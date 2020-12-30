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
        private static readonly ColorDiffuse ColorTintUS = new ColorDiffuse(0.39607808f, 0.5058824f, 0.286274165f); // hex: 648148
        private static readonly ColorDiffuse ColorTintGE = new ColorDiffuse(0.2666619f, 0.294113219f, 0.3097992f); // hex: 434A4E
        private static readonly ColorDiffuse ColorTintCW = new ColorDiffuse(0.443134367f, 0.3725461f, 0.2784285f); // hex: 705E46
        private static readonly ColorDiffuse ColorTintFFI = new ColorDiffuse(0.576470554f, 0.545097947f, 0.509800732f); // hex: 928A81
        private static readonly ColorDiffuse ColorTintCustomization = new ColorDiffuse(0.2941174f, 0.450980365f, 0.407842875f); // hex: 4A7267
        private static readonly ColorDiffuse ColorTintTerrainBoard = new ColorDiffuse(0.407842726f, 0.443136871f, 0.1882349f); // hex: 67702F
        private static readonly ColorDiffuse ColorTintCivilian = ColorTintFFI;
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