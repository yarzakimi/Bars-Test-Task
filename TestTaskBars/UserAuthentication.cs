using System;
using System.Text;
using System.Configuration;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

namespace TestTaskBars
{
    public static class UserAuthentication
    {
        public static UserCredential Authenticate(string[] scopes)
        {
            UserCredential credential;

            //using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["credentials"])))
            {
                var credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");
                credential =
                    GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                Console.WriteLine(DateTime.Now.ToString() + ": Credential file saved to: " + credPath);
            }

            return credential;
        }
    }
}
