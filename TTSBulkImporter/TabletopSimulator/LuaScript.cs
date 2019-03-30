using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TTSBulkImporter.TabletopSimulator.GamePiece;

namespace TTSBulkImporter.TabletopSimulator
{
    public class LuaScript
    {
        /// <summary>
        /// Naming style for GUID entries.
        /// </summary>
        public enum GuidNameStyle
        {
            /// <summary>
            /// "monsters = '123abc',
            /// </summary>
            NameAsKey,

            /// <summary>
            /// '123abc'    -- monsters,
            /// </summary>
            NameAsComment,

            /// <summary>
            /// '123abc',
            /// </summary>
            NoNameEver
        }

        private StringBuilder script = new StringBuilder();
        private StringBuilder guids = new StringBuilder();

        private const string comma = ",";
        private Func<string, string> WrapInSingleQuotes = str => "'" + str + "'";

        /// <summary>
        /// Return the script as a string.
        /// </summary>
        public string GetScript()
        {
            // Write GUIDs to script.
            script.AppendLine("GUIDs = {");
            script.Append(guids);
            script.AppendLine("}");

            return script.ToString();
        }

        /// <summary>
        /// Add a list of GUIDs to the script.
        /// </summary>
        public void AddGuids(IEnumerable<BaseGamePiece> gamePieces, GuidNameStyle nameStyle = GuidNameStyle.NameAsKey)
        {
            foreach (var piece in gamePieces)
            {
                AddGuid(piece, nameStyle);
            }
        }


        /// <summary>
        /// Add a single GUID to the script.
        /// </summary>
        private void AddGuid(BaseGamePiece gamePiece, GuidNameStyle nameStyle = GuidNameStyle.NameAsKey)
        {
            var line = "    ";
            var guid = WrapInSingleQuotes(gamePiece.GUID);
            var name = AsValidLuaVariableName(gamePiece.Nickname);

            // TODO: prepare name (strip whitespace and force first letter lower case)

            if (nameStyle == GuidNameStyle.NoNameEver || String.IsNullOrWhiteSpace(gamePiece.Nickname))
                line += guid;
            else
                if (nameStyle == GuidNameStyle.NameAsKey)
                    line += name + " = " + guid;
                else // GuidNameStyle.NameAsComment
                    line += guid + "   " + "-- " + name;

            guids.AppendLine(line + comma);
        }

        /// <summary>
        /// Return the string as a valid Lua variable name. Removes the following:
        ///     file extension
        ///     whitespace
        ///     non-alphanumeric characters (except "_")
        /// and
        ///     adds "_" if there is a leading number
        /// </summary>
        private string AsValidLuaVariableName(string name)
        {
            name = Path.GetFileNameWithoutExtension(name);
            name = name.Trim().Replace(" ", "");                    // Strip whitespace.
            name = Regex.Replace(name, @"^\d+", m => "_" + m);      // Pad leading number with an underscore.
            name = Regex.Replace(name, @"[^0-9|^a-z|^A-Z|^_]", ""); // Remove non-alphanumeric characters (except "_").

            return name;
        }
    }
}