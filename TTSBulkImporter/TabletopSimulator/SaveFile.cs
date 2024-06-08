using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TTSBulkImporter.TabletopSimulator.GamePiece;

namespace TTSBulkImporter.TabletopSimulator
{
    /// <summary>
    /// A barebones save file for TTS. Only contains the necessary fields. Most fields are auto-populated by TTS when the file is loaded.
    /// </summary>
    public class SaveFile
    {
        public string SaveName;
        public string GameMode;
        public string VersionNumber = "v13.2.2";
        public string Table = "Table_RPG";
        public string Sky = "Sky_Cathedral";
        public BaseGamePiece[] ObjectStates => _gamePieces.ToArray();

        private readonly List<BaseGamePiece> _gamePieces = new List<BaseGamePiece>();

        public SaveFile(string saveName = "", string gameMode = "")
        {
            SaveName = saveName;
            GameMode = gameMode;
        }

        public void AddGamePiece(BaseGamePiece gamePiece)
        {
            _gamePieces.Add(gamePiece);
        }

        /// <summary>
        /// Write the SaveFile to disk as a JSON file.
        /// </summary>
        public void WriteToDisk(string filename, bool pretty=true)
        {
            var compactJson = JsonConvert.SerializeObject(this);
            var outputJson = pretty ? JToken.Parse(compactJson).ToString(Formatting.Indented) : compactJson;

            var outputDirectory = Utils.Filesystem.GetSolutionRootDirectory();
            var outputFilePath = Path.Combine(outputDirectory.FullName, filename);

            Console.WriteLine("");
            Console.WriteLine("Saving JSON to file: " + outputFilePath);
            File.WriteAllText(outputFilePath, outputJson);
        }
    }
}
