using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTSBulkImporter.GoogleDrive;
using TTSBulkImporter.Importer;
using TTSBulkImporter.TabletopSimulator.GamePiece;
using TTSBulkImporter.TabletopSimulator.GamePiece.Component;

namespace TTSBulkImporter
{
    static class Program
    {
        private const string TestFolder         = "1K8i84c5EUcjYy8vRy8LFMjnVTvuSlY6S";

        private const string BattlegroundSet1   = "1p6RHV9lbNZJaTfSmaOVAR1kcYRAeT17n";

        private const string CoreBox            = "1Aw4ggc4S5ixhDZXsfforSQZGIAKvWM5t";
        private const string CoreTerrain        = "1Ie0V5KTu5CaWpcdOaXrCYR3ikA-Ms0Xc";
        private const string CoreUS             = "1MFB9Af7iBfDSSpotWGUpysDpYOuEYWhW";
        private const string CoreGE             = "16FuXA_0ZDSEHsFRJpDYaFuO07bWj7NXa";
        private const string CoreTokens         = "104hH2JOSLGXOXLFPEMMQ3P1aGnxHf6Kx";

        // -- hacks
        private static readonly ColorDiffuse ColorTintUS            = new ColorDiffuse(0.411764681f, 0.517647f, 0.301960766f); // hex: 69844d
        private static readonly ColorDiffuse ColorTintGE            = new ColorDiffuse(0.266666667f, 0.294118f, 0.309803922f); // hex: 444b4f
        private static readonly ColorDiffuse ColorTintCW            = new ColorDiffuse(0.443137255f, 0.372549f, 0.278431373f); // hex: 715f47
        private static readonly ColorDiffuse ColorTintFFI           = new ColorDiffuse(0.576470588f, 0.545098f, 0.509803922f); // hex: 938b82
        private static readonly ColorDiffuse ColorTintTerrainBoard  = new ColorDiffuse(0.415686275f, 0.450980f, 0.196078431f); // hex: 6A7332
        private static readonly ColorDiffuse ColorTintDefault       = new ColorDiffuse();
        // -- end hacks

        static void Main(string[] args)
        {
            // Args
            var folderId = BattlegroundSet1;
            var tintColor = ColorTintTerrainBoard;


            // Fetch GoogleDrive filesystem
            var service = GetDriveService();
            var modRootFolder = service.GetFilesystemFrom(folderId);

            // Make shareable
            service.MakeFilesystemShareable(modRootFolder);

            // Convert filesystem to TTS Json
            var converter = new Converter();
            converter.TintColor = tintColor;
            var bag = converter.ConvertFolderToBag(modRootFolder);
            SerializeToJsonAndPrint(bag);


            CloseProgram();
        }

        private static DriveScanner GetDriveService()
        {
            var service = DriveServiceFactory.CreateDriveService(AppConfig.DriveScopes, AppConfig.ApplicationName);
            return new DriveScanner(service);
        }

        private static void SerializeToJsonAndPrint<T>(T obj, bool alsoShowPretty = false)
        {
            var compactJson = JsonConvert.SerializeObject(obj);
            Console.WriteLine(compactJson);

            if (alsoShowPretty)
            {
                var prettyJson = JToken.Parse(compactJson).ToString(Formatting.Indented);
                Console.WriteLine(prettyJson);
            }

//            var deserialized = JsonConvert.DeserializeObject<T>(compactJson);
        }

        private static void CloseProgram()
        {
            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }
    }
}