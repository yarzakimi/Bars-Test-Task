using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace TestTaskBars
{
    public static class Helper
    {
        public static string GetCellNumber(int row, int column)
        {
            var columnSheet = new StringBuilder();

            while (column > 0)
            {
                var cm = column % 26;

                if (cm == 0)
                {
                    column--;
                    columnSheet.Insert(0, 'Z');
                }
                else
                {
                    columnSheet.Insert(0, (char)(cm + 'A' - 1));
                }

                column /= 26;
            }

            return $"{columnSheet}{row}";
        }

        public static SheetCellNumeric GetRowAndColumnNumber(string sheetCellNumber)
        {
            var i = 0;

            for (; i < sheetCellNumber.Length; i++)
            {
                if (char.IsDigit(sheetCellNumber[i]))
                {
                    break;
                }
            }

            var column = 0;
            var row = sheetCellNumber.Substring(i);

            var cs = sheetCellNumber.Substring(0, i);
            var csl = cs.Length - 1;

            for (var j = 0; j < cs.Length; j++)
            {
                var cc = cs[j] - 'A' + 1;

                var multi = (int)Math.Pow(26, csl--);

                column += cc * multi;
            }

            return new SheetCellNumeric { RowNumber = int.Parse(row), ColumnNumber = column };
        }

        public static IList<IList<Object>> GenerateData(IList<IList<Object>> table)
        {
            List<IList<Object>> objNewRecords = new List<IList<Object>>();

            IList<Object> obj = new List<Object>();

            obj.Add("Сервер");
            obj.Add("База данных");
            obj.Add("Размер в байтах");
            obj.Add("Размер");
            obj.Add("Дата обновления");

            objNewRecords.Add(obj);

            objNewRecords.AddRange(table);

            return objNewRecords;
        }

        public static string CreateSpreadsheet(SheetsService service, string spreadsheetName)
        {
            Spreadsheet new_spreadsheet = new Spreadsheet();
            new_spreadsheet.Properties = new SpreadsheetProperties();
            new_spreadsheet.Properties.Title = spreadsheetName;
            var createRequest = service.Spreadsheets.Create(new_spreadsheet).Execute();
            return createRequest.SpreadsheetId;
        }

        public static List<string> GetSheetsList(SheetsService service, string spreadsheetId)
        {
            var ssRequest = service.Spreadsheets.Get(spreadsheetId);
            Spreadsheet ss = ssRequest.Execute();

            List<string> sheetList = new List<string>();

            foreach (Sheet sheet in ss.Sheets)
            {
                sheetList.Add(sheet.Properties.Title);
            }
            return sheetList;
        }

        public static void AddSheet(SheetsService service, string spreadsheetId, string sheetName)
        {
            string new_sheetName = sheetName;
            var addSheetRequest = new AddSheetRequest();
            addSheetRequest.Properties = new SheetProperties();
            addSheetRequest.Properties.Title = new_sheetName;
            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            batchUpdateSpreadsheetRequest.Requests.Add(new Request
            {
                AddSheet = addSheetRequest
            });

            var batchUpdateRequest =
                service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId);

            batchUpdateRequest.Execute();
        }

        public static List<string> GetServersList()
        {
            List<string> serverList = new List<string>();

            for(int i=0; i<ConfigurationManager.ConnectionStrings.Count; i++)
            {
                if (ConfigurationManager.ConnectionStrings[i].ProviderName == "Npgsql")
                    serverList.Add(ConfigurationManager.ConnectionStrings[i].Name);
            }

            return serverList;
        }

        public static void AddOrUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        public class SheetCellNumeric
        {
            public int ColumnNumber;

            public int RowNumber;
        }
    }
}
