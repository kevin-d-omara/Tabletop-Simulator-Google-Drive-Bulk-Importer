using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TTSBulkImporter.GoogleDrive;
using TTSBulkImporter.TabletopSimulator;
using TTSBulkImporter.TabletopSimulator.GamePiece;
using TTSBulkImporter.TabletopSimulator.GamePiece.Component;

namespace TTSBulkImporter.Importer
{
    /// <summary>
    /// Converts a Google Drive folder to a Tabletop Simulator bag.
    /// </summary>
    /// <seealso cref="DriveFolder"/>
    /// <seealso cref="BagGamePiece"/>
    public class Converter
    {
        /// <summary>
        /// The tint color that will be applied to all game objects created from Google Drive.
        /// </summary>
        public ColorDiffuse TintColor { get; } = ColorDiffuse.Colors.Default;

        private readonly List<string> allowableExtensions = new List<string>
        {
            ".png",
            ".jpg"
        };

        private Regex sideAPattern = new Regex(@"(\.A)");   // Filename contains '.A', ex: Recon.A.png
        private Regex sideBPattern = new Regex(@"(\.B)");   // Filename contains '.B', ex: Recon.B.png
        private Regex tokenPattern = new Regex(@"(\.token)", RegexOptions.IgnoreCase);  // Filename contains '.token', ex: Smoke.token.png, Smoke Marker.token.png

        private const float DefaultTableHeight = 0.77f;

        public Converter(ColorDiffuse tintColor)
        {
            TintColor = tintColor;
        }

        public BagGamePiece ConvertFolderToBag(DriveFolder driveFolder, bool isRoot = false, bool addLuaScript=false, bool matchSequentialFilesAsDoubleSided = false)
        {
            var bag = new BagGamePiece();
            bag.Nickname = driveFolder.Name;

            if (isRoot)
            {
                // Raise the bag height so it rests naturally on the table-top.
                // Otherwise it would "snap" up to the table when loading the mod.
                bag.Transform.posY = DefaultTableHeight;
            }

            // Filter down to image files.
            var validFiles = driveFolder.Files.Where(IsConvertableToGamePiece);
            var tokenFiles = validFiles.Where(file => IsToken(file));
            var tileFiles = validFiles.Where(file => !IsToken(file));

            // Create TTS objects.
            var tokens = tokenFiles.Select(ConvertToToken).ToList();
            var tiles = ConvertToTiles(tileFiles, matchSequentialFilesAsDoubleSided).ToList();
            var bags = driveFolder.Folders.Select(x => ConvertFolderToBag(x,
                addLuaScript: addLuaScript,
                matchSequentialFilesAsDoubleSided: matchSequentialFilesAsDoubleSided)).ToList();

            // Add objects to bag.
            bag.ContainedObjects = bag.ContainedObjects
                .Concat(tokens)
                .Concat(tiles)
                .Concat(bags)
                .ToList();

            // Add script w/ GUIDs to this bag.
            if (addLuaScript)
            {
                var script = new LuaScript();
                script.AddGuids(tokens);
                script.AddGuids(tiles);
                script.AddGuids(bags);
                bag.LuaScript = script.GetScript();
            }

            // Clear Nicknames (they were only used for generating LuaScript named GUIDs).
            tokens.ForEach(token => token.Nickname = "");
            tiles.ForEach(tile => tile.Nickname = "");

            return bag;
        }

        /// <summary>
        /// True if the filename has an allowed extension. Currently: ".png" or ".jpg".
        /// </summary>
        private bool IsConvertableToGamePiece(DriveFile driveFile)
        {
            var extension = Path.GetExtension(driveFile.Name).ToLower();

            return allowableExtensions.Contains(extension);
        }

        /// <summary>
        /// True if the filename contains the substring ".token."
        /// </summary>
        private bool IsToken(DriveFile driveFile)
        {
            return tokenPattern.IsMatch(driveFile.Name);
        }

        private TokenGamePiece ConvertToToken(DriveFile file)
        {
            var token = new TokenGamePiece();
            token.Nickname = AsValidNickname(file.Name);
            token.CustomImage.ImageURL = file.DownloadLink;
            token.ColorDiffuse = TintColor;

            return token;
        }

        private TileGamePiece ConvertToTileSingleSided(DriveFile file)
        {
            var tile = new TileGamePiece();
            tile.Nickname = AsValidNickname(file.Name);
            tile.CustomImage.ImageURL = file.DownloadLink;
            tile.ColorDiffuse = TintColor;
            
            return tile;
        }

        private TileGamePiece ConvertToTileDoubleSided(DriveFile fileA, DriveFile fileB)
        {
            var tile = new TileGamePiece();
            tile.Nickname = AsValidNickname(fileA.Name);
            tile.CustomImage.ImageURL = fileA.DownloadLink;
            tile.CustomImage.ImageSecondaryURL = fileB.DownloadLink;
            tile.ColorDiffuse = TintColor;
            
            return tile;
        }

        /// <summary>
        /// Transform the files (in one folder) into Tiles.
        /// </summary>
        private IEnumerable<TileGamePiece> ConvertToTiles(IEnumerable<DriveFile> files, bool matchSequentialFilesAsDoubleSided)
        {
            return matchSequentialFilesAsDoubleSided
                ? ConvertToTilesMatchingSequential(files)
                : ConvertToTilesMatchingAB(files);
        }

        private IEnumerable<TileGamePiece> ConvertToTilesMatchingSequential(IEnumerable<DriveFile> files)
        {
            var filesList = files.ToList();
            if (!IsEvenNumber(filesList.Count))
            {
                throw new ArgumentException($"There is an odd number of files ({filesList.Count}). Cannot match sequential images as front/back because there is an odd number.");
            }

            filesList.Sort(new PropertyComparer<DriveFile, string>(
                comparer: new NaturalComparer(),
                accessor: (DriveFile driveFile) => driveFile.Name));

            var tiles = new List<TileGamePiece>();
            for (int i = 0; i < filesList.Count - 1; i+=2)
            {
                var fileA = filesList[i];
                var fileB = filesList[i+1];
                tiles.Add(ConvertToTileDoubleSided(fileA, fileB));
            }

            return tiles;
        }

        private IEnumerable<TileGamePiece> ConvertToTilesMatchingAB(IEnumerable<DriveFile> files)
        {
            var tiles = new List<TileGamePiece>();

            var aSides = files.Where(file => sideAPattern.IsMatch(file.Name));
            var bSides = files.Where(file => sideBPattern.IsMatch(file.Name));
            var singleSides = files.Where(file => !sideAPattern.IsMatch(file.Name) && !sideBPattern.IsMatch(file.Name));

            if (aSides.Count() != bSides.Count())
            {
                throw new ArgumentException("Number of side A and side B files does not match: # A is " + aSides.Count() + ", # B is " + bSides.Count());
            }

            // Map between stripped filename and DriveFile.
            var sideBMap = new Dictionary<string, DriveFile>();
            foreach (var file in bSides)
            {
                sideBMap.Add(sideBPattern.Replace(file.Name, ""), file);
            }

            // Create double-sided Tiles.
            foreach (var fileA in aSides)
            {
                DriveFile fileB;
                if (sideBMap.TryGetValue(sideAPattern.Replace(fileA.Name, ""), out fileB))
                {
                    tiles.Add(ConvertToTileDoubleSided(fileA, fileB));
                }
                else
                {
                    throw new ArgumentException("No side B to match with file: " + fileA);
                }
            }

            // Create single-sided Tiles.
            foreach (var file in singleSides)
            {
                tiles.Add(ConvertToTileSingleSided(file));
            }

            return tiles;
        }

        /// <summary>
        /// Return the string as a valid Tabletop Simulator object nickname. Removes the following:
        ///     file extension
        ///     ".A" from end of filename
        ///     ".B" from end of filename
        ///     ".token" from end of filename
        /// </summary>
        private string AsValidNickname(string filename)
        {
            filename = Path.GetFileNameWithoutExtension(filename);
            filename = sideAPattern.Replace(filename, "");
            filename = sideBPattern.Replace(filename, "");
            filename = tokenPattern.Replace(filename, "");

            return filename;
        }

        /// <summary>
        /// Clear the Nickname of all game the game pieces. 
        /// </summary>
        /// <param name="pieces"></param>
        private void ClearNicknames(IEnumerable<BaseGamePiece> pieces)
        {
            foreach (var piece in pieces)
            {
                piece.Nickname = "";
            }
        }

        private bool IsEvenNumber(int x)
        {
            return x % 2 == 0;
        }
    }
}