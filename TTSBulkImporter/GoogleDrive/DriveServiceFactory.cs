using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace TTSBulkImporter.GoogleDrive
{
    /// <summary>
    /// Factory that creates instances of <see cref="DriveService"/>.
    /// </summary>
    public static class DriveServiceFactory
    {
        /// <summary>
        /// Return a new <see cref="DriveService"/> with the specified OAuth 2.0 scopes and application name.
        /// </summary>
        /// <param name="scopes">Valid OAuth 2.0 scopes for use with the Google Drive API. <see cref="DriveService.Scope"/></param>
        public static DriveService CreateDriveService(string[] scopes, string applicationName)
        {
            var credential = SignInUser(scopes);
            var service = CreateDriveService(credential, applicationName);

            return service;
        }

        private static UserCredential SignInUser(string[] scopes)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                Console.WriteLine("Please sign in to continue. Check your browser for the sign-in page.");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Sign in successful, credential file saved to: " + credPath);
            }

            return credential;
        }

        private static DriveService CreateDriveService(UserCredential credential, string applicationName)
        {
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });
        }
    }
}
