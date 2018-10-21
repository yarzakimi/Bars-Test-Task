using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace TestTaskBars
{
    class SheetDataWriter
    {
        private readonly SheetsService _service;

        private readonly string _sheetId;

        private readonly string _sheetName;

        public SheetDataWriter(SheetsService service, string sheetId, string sheetName)
        {
            this._service = service;
            this._sheetId = sheetId;
            this._sheetName = sheetName;
        }

        public async Task<UpdateValuesResponse> WriteToSheetAsync(string sheetCellNumber, IList<IList<Object>> valueToWrite)
        {
            try
            {
                var range = sheetCellNumber; // "Basic!B111";
                var valueRange = new ValueRange() { Values = valueToWrite };
                
                //valueRange.Values = valueToWrite;

                var update = this._service.Spreadsheets.Values.Update(valueRange, this._sheetId, range);
                update.ValueInputOption =
                    SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                return await update.ExecuteAsync();
            }
            catch (Exception)
            {
                throw new Exception("Error in writing data");
            }
        }
    }
}
