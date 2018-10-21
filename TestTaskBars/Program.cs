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
            List<string> serverList = Helper.GetServersList();
            foreach (var srv in serverList)
            {
                var serverName = srv;
                var connectionParams = ConfigurationManager.ConnectionStrings[serverName].ConnectionString;
                ConnectPostgres server = new ConnectPostgres(serverName, connectionParams);
                IList<IList<Object>> server_databases = new List<IList<Object>>();
                server_databases = server.GetTable();
                foreach (var sublist in server_databases)
                {
                    foreach (var cell in sublist)
                    {
                        Console.Write(cell + "\t");
                    }
                    Console.WriteLine();
                }
                Console.Read();

                var spreadsheetId = ConfigurationManager.AppSettings["SheetId"];
                //var sheetName = ConfigurationManager.AppSettings["SheetName"];
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

                List<string> sheetList = Helper.GetSheetsList(service, spreadsheetId);

                var sheetName = sheetList.Find(x => x == serverName);
                if (sheetName == null)
                {
                    sheetName = serverName;
                    Helper.AddSheet(service, spreadsheetId, sheetName);
                }

                IList<IList<Object>> whatTowrite = Helper.GenerateData(server_databases);

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
}
