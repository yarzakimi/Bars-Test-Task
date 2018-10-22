using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace TestTaskBars
{
    public class Attendance
    {
        public string AttendanceId { get; set; }
    }
    public class SheetSimpleWriter
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "TestTask";
        static string SheetId = "1lG8Jad9hhhPZCrSe13abW-g3clKbRirP2Jvn0Cx_sFs";

        static void SimpleWrite()
        {
            var service = AuthorizeGoogleApp();

            string newRange = GetRange(service);

            IList<IList<Object>> objNeRecords = GenerateData();

            UpdatGoogleSheetinBatch(objNeRecords, SheetId, newRange, service);

            Console.WriteLine("Inserted");
            Console.Read();
        }
        private static SheetsService AuthorizeGoogleApp()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return service;
        }

        protected static string GetRange(SheetsService service)
        {
            // Define request parameters.
            String spreadsheetId = SheetId;
            String range = "A:A";

            SpreadsheetsResource.ValuesResource.GetRequest getRequest =
                       service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange getResponse = getRequest.Execute();
            IList<IList<Object>> getValues = getResponse.Values;

            //int currentCount = getValues.Count() + 2;

            //String newRange = "A" + currentCount + ":A";
            String newRange = "A:A";

            return newRange;
        }

        private static IList<IList<Object>> GenerateData()
        {
            List<IList<Object>> objNewRecords = new List<IList<Object>>();

            IList<Object> obj = new List<Object>();

            obj.Add("Column - 1");
            obj.Add("Column - 2");
            obj.Add("Column - 3");

            objNewRecords.Add(obj);

            return objNewRecords;
        }

        private static void UpdatGoogleSheetinBatch(IList<IList<Object>> values, string spreadsheetId, string newRange, SheetsService service)
        {
            SpreadsheetsResource.ValuesResource.AppendRequest request =
               service.Spreadsheets.Values.Append(new ValueRange() { Values = values }, spreadsheetId, newRange);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var response = request.Execute();
        }
    }
}