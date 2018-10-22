using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace TestTaskBars
{
    class Program
    {
        static void Main(string[] args)
        {
            Timer timer = new Timer(new TimerCallback(Timer_Task), null, 1000, 60000*ConfigurationManager.ConnectionStrings.Count);
            
            Console.WriteLine(DateTime.Now.ToString() + ": To stop programm, press 'q'");

            ConsoleKeyInfo keypress;
            do
            {
                Task.Delay(1000).Wait();
                keypress = Console.ReadKey();

            } while (keypress.KeyChar != 'q');
            timer.Dispose();
            Console.WriteLine(DateTime.Now.ToString() + ": The program has completed");
        }

        public static void Timer_Task(object source)
        {
            List<string> serverList = Helper.GetServersList();

            foreach (var srv in serverList)
            {
                var serverName = srv;
                var connectionParams = ConfigurationManager.ConnectionStrings[serverName].ConnectionString;

                ConnectPostgres server = new ConnectPostgres(serverName, connectionParams);
                IList<IList<Object>> server_databases = new List<IList<Object>>();

                server_databases = server.GetTable();
                Console.WriteLine(DateTime.Now.ToString() + ": Received the following data:");
                foreach (var sublist in server_databases)
                {
                    foreach (var cell in sublist)
                    {
                        Console.Write(cell + "\t");
                    }
                    Console.WriteLine();
                }
                                
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

                var spreadsheetId = ConfigurationManager.AppSettings["SheetId"];
                if(spreadsheetId == "")
                {
                    Console.WriteLine(DateTime.Now.ToString() + ": Unable to get access to the Google Spreadsheets. Missind spreadsheetID");
                    spreadsheetId = Helper.CreateSpreadsheet(service, "TestTaskBars");
                    Helper.AddOrUpdateAppSettings("SheetId", spreadsheetId);
                }

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

                var sheetCellNumber = Helper.GetCellNumber(1, 1);

                var rc = Helper.GetRowAndColumnNumber(sheetCellNumber);
                var cellToWrite = Helper.GetCellNumber(rc.RowNumber, rc.ColumnNumber);

                var dataWriter = new SheetDataWriter(service, spreadsheetId, sheetName);
                var result = dataWriter.WriteToSheetAsync(cellToWrite, whatTowrite);

                Console.WriteLine(DateTime.Now.ToString() + ": Data successfully written to sheet {0}", sheetName);
            }
        }
    }
}
