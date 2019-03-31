using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            /// Puts the key in brackets and double quotes so that any characters can be used, including an invalid Lua variable name.
            /// ["recon-team"] = '123abc',
            /// </summary>
            NameAsKeyWithBrackets,

            /// <summary>
            /// monsters = '123abc',
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
        public void AddGuids(IEnumerable<BaseGamePiece> gamePieces, GuidNameStyle nameStyle = GuidNameStyle.NameAsKeyWithBrackets)
        {
            foreach (var piece in gamePieces)
            {
                AddGuid(piece, nameStyle);
            }
        }


        /// <summary>
        /// Add a single GUID to the script.
        /// </summary>
        private void AddGuid(BaseGamePiece gamePiece, GuidNameStyle nameStyle = GuidNameStyle.NameAsKeyWithBrackets)
        {
            var line = "    ";
            var guid = WrapInSingleQuotes(gamePiece.GUID);
            var name = AsValidLuaVariableName(gamePiece.Nickname);

            if (String.IsNullOrWhiteSpace(gamePiece.Nickname))
                line += guid;
            else
                switch (nameStyle)
                {
                    case GuidNameStyle.NoNameEver:
                        line += guid;
                        break;
                    case GuidNameStyle.NameAsKey:
                        line += name + " = " + guid;
                        break;
                    case GuidNameStyle.NameAsComment:
                        line += guid + "   " + "-- " + name;
                        break;
                    case GuidNameStyle.NameAsKeyWithBrackets:
                        line += "[\"" + gamePiece.Nickname + "\"]" + " = " + guid;
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }

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