using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Configuration;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;

namespace TestTaskBars
{
    class Program
    {
        static void Main(string[] args)
        {
            /*DataTable size_server = new DataTable();
            size_server.Columns.Add("Server", typeof(string));
            size_server.Columns.Add("Database", typeof(string));
            size_server.Columns.Add("Size in bytes", typeof(long));
            size_server.Columns.Add("Size", typeof(string));
            size_server.Columns.Add("Update date", typeof(DateTime));*/
            ConnectPostgres server = new ConnectPostgres();
            IList<IList<Object>> server_databases = new List<IList<Object>>();
            server_databases = server.GetTable();
            foreach(var sublist in server_databases)
            {
                foreach(var cell in sublist)
                {
                    Console.Write(cell + "\t");
                }
                Console.WriteLine();
            }
            Console.Read();
            //ConnectGoogle sheet = new ConnectGoogle();
            //sheet.TestFunction();

            var spreadsheetId = ConfigurationManager.AppSettings["SheetId"];
            var sheetName = ConfigurationManager.AppSettings["SheetName"];
            var range = ConfigurationManager.AppSettings["ReaderRange"];

            string[] scopes = { SheetsService.Scope.Spreadsheets };
            var ApplicationName = "Google Sheet Test Task";

            var credential = UserAuthentication.Authenticate(scopes);
            // Create Google Sheets API service.
            var service =
                new SheetsService(
                    new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = ApplicationName
                    });

            /*List<IList<Object>> test_table = new List<IList<Object>>();
            Random rnd = new Random();
            for(int i=0; i<4; i++)
            {
                IList<Object> obj = new List<Object>();

                obj.Add("Server1");
                obj.Add("postres");
                obj.Add(rnd.Next());
                obj.Add(rnd.Next());
                obj.Add(DateTime.Now.ToString("d.M.yyyy"));

                test_table.Add(obj);
            }*/

            IList<IList<Object>> whatTowrite = Helper.GenerateData(server_databases);
            //string searchValue = "Server1";

            var dataReader = new SheetDataReader(service, spreadsheetId, sheetName, range);
            var sheetDataTask = dataReader.ReadDataAsync();
            var sheetData = sheetDataTask.Result;

            //if (sheetData != null)
            //{
                //var sheetCellNumber = dataReader.FindCell(sheetData, searchValue);
                var sheetCellNumber = Helper.GetCellNumber(1, 1);

                if (sheetCellNumber != null)
                {
                    var rc = Helper.GetRowAndColumnNumber(sheetCellNumber);
                    var cellToWrite = Helper.GetCellNumber(rc.RowNumber, rc.ColumnNumber);

                    var dataWriter = new SheetDataWriter(service, spreadsheetId, sheetName);
                    var result = dataWriter.WriteToSheetAsync(cellToWrite, whatTowrite);
                    Console.WriteLine($"{result.Result} is updated");
                }
                else
                {
                    Console.WriteLine("The target data not found in the sheet");
                }
            //}
            //else
            //{
                //Console.WriteLine("Sheet is empty");
            //}

            Console.WriteLine("Data written successfully");
        }
    }
}
