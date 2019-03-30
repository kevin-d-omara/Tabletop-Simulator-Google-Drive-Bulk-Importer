using Google.Apis.Drive.v3;

namespace TTSBulkImporter
{
    public static class AppConfig
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        public static readonly string[] DriveScopes =
        {
            DriveService.Scope.DriveReadonly,
            DriveService.Scope.Drive,
            DriveService.Scope.DriveFile
        };

        public const string ApplicationName = "TTS Bulk Importer";
    }
}
