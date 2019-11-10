using System;
using System.IO;
using Newtonsoft.Json;
using TTSBulkImporter.GoogleDrive;
using TTSBulkImporter.Importer;
using TTSBulkImporter.TabletopSimulator.GamePiece.Component;
using Newtonsoft.Json.Linq;

namespace TTSBulkImporter
{
    static class Program
    {
        // -- hacks
        private static readonly ColorDiffuse ColorTintUS            = new ColorDiffuse(0.411764681f, 0.517647f, 0.301960766f); // hex: 69844d
        private static readonly ColorDiffuse ColorTintGE            = new ColorDiffuse(0.266666667f, 0.294118f, 0.309803922f); // hex: 444b4f
        private static readonly ColorDiffuse ColorTintCW            = new ColorDiffuse(0.443137255f, 0.372549f, 0.278431373f); // hex: 715f47
        private static readonly ColorDiffuse ColorTintFFI           = new ColorDiffuse(0.576470588f, 0.545098f, 0.509803922f); // hex: 938b82
        private static readonly ColorDiffuse ColorTintCivilian      = new ColorDiffuse(0.752941176f, 0.733333f, 0.717647059f); // hex: c0bbb7
        private static readonly ColorDiffuse ColorTintTerrainBoard  = new ColorDiffuse(0.415686275f, 0.450980f, 0.196078431f); // hex: 6A7332
        private static readonly ColorDiffuse ColorTintDefault       = new ColorDiffuse();
        // -- end hacks

        static void Main(string[] args)
        {
            // Args
            var folderId = "1G_g0GFAJxaL9GkW4SqgNiQPJ2CGZuSbQ";
            var tintColor = ColorTintTerrainBoard;
            var outputFileName = "_battleground-4.json";

            // Fetch GoogleDrive filesystem
            var service = GetDriveService();
            var modRootFolder = service.GetFilesystemFrom(folderId);

            // Make shareable
            service.MakeFilesystemShareable(modRootFolder);

            // Convert filesystem to TTS Json
            var converter = new Converter();
            converter.TintColor = tintColor;
            var bag = converter.ConvertFolderToBag(modRootFolder);
            var compactJson = JsonConvert.SerializeObject(bag);
            var prettyJson = JToken.Parse(compactJson).ToString(Formatting.Indented);

            // Write JSON to disk and display on console
            WriteJsonToDisk(prettyJson, outputFileName);

            CloseProgram();
        }

        private static void WriteJsonToDisk(string json, string outputFileName)
        {
            var outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), outputFileName);
            Console.WriteLine("Saving JSON to file: " + outputFilePath);
            File.WriteAllText(outputFilePath, json);
        }

        private static DriveScanner GetDriveService()
        {
            var service = DriveServiceFactory.CreateDriveService(AppConfig.DriveScopes, AppConfig.ApplicationName);
            return new DriveScanner(service);
        }

        private static void CloseProgram()
        {
            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }
    }
}