using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;

namespace TestTaskBars
{
    class SheetDataReader
    {
        private readonly string _range;

        private readonly SheetsService _service;

        private readonly string _sheetName;

        private readonly string _spreadsheetId;

        public SheetDataReader(SheetsService service, string spreadsheetId, string sheetName, string range)
        {
            this._range = range;
            this._service = service;
            this._spreadsheetId = spreadsheetId;
            this._sheetName = sheetName;
        }

        public async Task<IList<IList<object>>> ReadDataAsync()
        {
            try
            {
                var request = this._service.Spreadsheets.Values.Get(
                    this._spreadsheetId,
                    this._range);

                var response = await request.ExecuteAsync();
                return response.Values;
            }
            catch (Exception)
            {
                throw new Exception("Error in reading data");
            }
        }

        public string FindCell(IList<IList<object>> values, string targetString)
        {
            if (values == null || values.Count == 0)
            {
                return null;
            }

            var rowNumber = 0;
            foreach (var row in values)
            {
                for (var columnNumber = 0; columnNumber < row.Count; columnNumber++)
                {
                    if ((string)row[columnNumber] == targetString)
                    {
                        return this.CalculateCell(rowNumber, columnNumber);
                    }
                }
                rowNumber++;
            }

            return null;
        }

        private string CalculateCell(int rowNumber, int columnNumber)
        {
            var rowAndColumn = Helper.GetRowAndColumnNumber(this._range.Split(':').ToArray()[0]);

            var row = rowAndColumn.RowNumber;
            var column = rowAndColumn.ColumnNumber;

            return Helper.GetCellNumber(rowNumber + row, columnNumber + column);
        }
    }
}
