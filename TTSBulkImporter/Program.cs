using Google.Apis.Drive.v3;
using System;
using TTSBulkImporter.GoogleDrive;
using TTSBulkImporter.Importer;
using TTSBulkImporter.TabletopSimulator;
using TTSBulkImporter.TabletopSimulator.GamePiece.Component;

namespace TTSBulkImporter
{
    // INSTRUCTIONS:
    // 1. Change the "Settings" below.
    // 2. Click "Start" (with the green arrow) at the top toolbar above.
    // 3. Follow the instructions on the Command Line Terminal that pops up.
    //      - The first time you run this program each day, it will ask you to sign in to Google Drive.
    //        It should automatically open the sign-in page in your browser.
    //        If this doesn't happen, try closing the programming and re-running it.
    //        Sometimes the program crashes on this step, but then always succeeds when I re-run it.
    //
    //      - After signing in, it will gradually update permissions for all the files.
    //
    //      - Finally, it will create the TTS Save file and tell you where the file was created.

    static class Program
    {
        private static class Settings
        {
            /// <summary>
            /// The ID of the target Google Drive folder.
            ///
            /// You can get the ID by right clicking the folder > Share > Copy link
            ///
            /// The link will look like this:
            /// https://drive.google.com/drive/folders/1ldNLgdT4Qei3mEEUve8a4BVA_LM3bxAM?usp=drive_link
            ///                                        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            /// Then copy the part with ^^^^ and paste it below inside the quotes:
            /// </summary>
            public static string GoogleDriveFolderId = "1HgOgKPgUV0j-7Z8eCWJAfryy6YkLpMSb";

            /// <summary>
            /// If 'true':
            ///     Sequential files (in "natural sort" order, similar to alphabetical order) are matched together as
            ///     double-sided tiles.
            ///
            ///     For example:
            ///         PZL B1_ART_V0.1.png  -> Tile 1, Front Side
            ///         PZL B1_ART_V0.2.png  -> Tile 1, Back Side
            ///         PZL B1_ART_V0.3.png  -> Tile 2, Front Side
            ///         PZL B1_ART_V0.4.png  -> Tile 2, Back Side
            ///
            ///     The program will crash if there is an odd number of files in any of the folders, because
            ///     that indicates the user may have not uploaded a full set of matching front/back images.
            ///
            /// If 'false':
            ///     Double sided tiles are matched when the two images have identical names except for having an .A/.B
            ///     suffix.
            ///
            ///     For example:
            ///         PZL B1_ART_V0.1.A.png  -> Tile 1, Front Side
            ///         PZL B1_ART_V0.1.B.png  -> Tile 1, Back Side
            ///         PZL B1_ART_V0.2.A.png  -> Tile 2, Front Side
            ///         PZL B1_ART_V0.2.B.png  -> Tile 2, Back Side
            /// </summary>
            public static bool MatchSequentialFilesAsDoubleSided = true;

            /// <summary>
            /// The name of the TTS Save File to create.
            /// The file will be created in the root folder ("Tabletop-Simulator-Google-Drive-Bulk-Importer", the same
            /// folder containing the ".sln" file).
            /// </summary>
            public static string TabletopSimulatorSaveFileName = "TTS Bulk Import Save File.json";

            /// <summary>
            /// The color tint that all the game pieces will have.
            /// </summary>
            public static ColorDiffuse TintColor = HeroesSystemConfig.Colors.Pink;

            /// <summary>
            /// TODO: Add support for this.
            /// </summary>
            public static float TokenThickness = 0.16f;
        }

        static void Main(string[] args)
        {
            // Get files from Google Drive
            var service = GetDriveService();
            var modRootFolder = service.GetFilesystemFrom(Settings.GoogleDriveFolderId);

            // Convert files to TTS game pieces
            var converter = new Converter(Settings.TintColor);
            var bag = converter.ConvertFolderToBag(modRootFolder, isRoot: true,
                matchSequentialFilesAsDoubleSided: Settings.MatchSequentialFilesAsDoubleSided);

            // Make files shareable with public
            service.MakeFilesystemShareable(modRootFolder);

            // Write TTS save file to disk
            var ingameSaveName = "TTS Bulk Import";
            var saveFile = new SaveFile(saveName: ingameSaveName, gameMode: ingameSaveName);
            saveFile.AddGamePiece(bag);
            saveFile.WriteToDisk(Settings.TabletopSimulatorSaveFileName);

            // Done
            CloseProgram();
        }

        private static DriveScanner GetDriveService()
        {
            var service = DriveServiceFactory.CreateDriveService(AppConfig.DriveScopes, AppConfig.ApplicationName);
            return new DriveScanner(service);
        }

        private static void CloseProgram()
        {
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }
    }
}