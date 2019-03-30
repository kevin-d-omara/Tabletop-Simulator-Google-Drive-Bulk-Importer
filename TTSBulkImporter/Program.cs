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

        private const string USRiflePlatoon12th = "13dQn17K8w2xCvFFyE-KUiVimw0wGZWvx";
        private const string SteinerKampfgruppe = "1BPg-q2cmhfpPUV5h6rEfGSTZYBFD2QdC";
        private const string Gazette0           = "1ztV5SKQYCE5hQ7qV6B-GcpOO5XNyPepV";

        private const string CoreBox            = "1Aw4ggc4S5ixhDZXsfforSQZGIAKvWM5t";
        private const string CoreTerrain        = "1Ie0V5KTu5CaWpcdOaXrCYR3ikA-Ms0Xc";
        private const string CoreUS             = "1MFB9Af7iBfDSSpotWGUpysDpYOuEYWhW";
        private const string CoreGE             = "16FuXA_0ZDSEHsFRJpDYaFuO07bWj7NXa";
        private const string CoreTokens         = "104hH2JOSLGXOXLFPEMMQ3P1aGnxHf6Kx";

        // -- hacks
        private static readonly ColorDiffuse ColorTintUS      = new ColorDiffuse(0.411764681f, 0.517647f, 0.301960766f); // hex: 69844d
        private static readonly ColorDiffuse ColorTintGE      = new ColorDiffuse(0.266666667f, 0.294118f, 0.309803922f); // hex: 444b4f
        private static readonly ColorDiffuse ColorTintCW      = new ColorDiffuse(0.443137255f, 0.372549f, 0.278431373f); // hex: 715f47
        private static readonly ColorDiffuse ColorTintFFI     = new ColorDiffuse(0.576470588f, 0.545098f, 0.509803922f); // hex: 938b82
        private static readonly ColorDiffuse ColorTintTerrain = new ColorDiffuse(0.415686275f, 0.450980f, 0.196078431f); // hex: 6A7332
        private static readonly ColorDiffuse ColorTintDefault = new ColorDiffuse();
        // -- end hacks

        static void Main(string[] args)
        {
            // Args
            var folderId = TestFolder;
            var tintColor = ColorTintUS;

            // Make shareable.
//            var rootFolder = Demonstrate_ShareFilesystem(folderId);
//            Demonstrate_DownloadLinks(rootFolder);

            // Fetch Drive filesystem
            var service = GetDriveService();
            var modRootFolder = service.GetFilesystemFrom(folderId);

            // Convert filesystem to TTS Json
            var converter = new Converter();
            converter.TintColor = tintColor;
            var bag = converter.ConvertFolderToBag(modRootFolder);
            Demonstrate_JsonSerialization(bag, true);

            CloseProgram();
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


        #region Examples

        private static void Demonstrate_TTSJson()
        {
            var tile = new TileGamePiece();
            tile.CustomImage.ImageURL = @"http://i.imgur.com/YMAJp6R.jpg";
            tile.CustomImage.ImageSecondaryURL = @"http://i.imgur.com/jtZs1cV.jpg";
            //Demonstrate_JsonSerialization(tile);

            var token = new TokenGamePiece();
            token.CustomImage.ImageURL = @"https://drive.google.com/uc?export=download&id=1cXiByhyvglP9x-8gKPfVs3GvEColVYF1";
            //Demonstrate_JsonSerialization(token);

            var bag = new BagGamePiece();
            bag.ContainedObjects.Add(tile);
            bag.ContainedObjects.Add(token);
            Demonstrate_JsonSerialization(bag, true);
        }

        private static DriveFolder Demonstrate_ShareFilesystem(string folderId)
        {
            var service = GetDriveService();

            var modRootFolder = service.GetFilesystemFrom(folderId);

            service.MakeFilesystemShareable(modRootFolder);

            return modRootFolder;
        }

        private static void Demonstrate_DownloadLinks(DriveFolder driveFolder)
        {
            foreach (var file in driveFolder.Files)
            {
                Console.WriteLine(file.DownloadLink);
            }
        }

        private static void Demonstrate_JsonSerialization<T>(T obj, bool alsoShowPretty = false)
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

        #endregion
    }
}