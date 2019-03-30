using System;
using System.Collections.Generic;
using System.Linq;

namespace TTSBulkImporter.TabletopSimulator
{
    /// <summary>
    /// Provides valid Tabletop Simulator GUIDs.
    /// <remarks>
    /// A TTS GUID is a string with six alphanumeric characters. However, it appears a GUID can be of any length.
    /// Therefore, it is possible to use the Nickname of a piece as the GUID.
    /// </remarks>
    /// </summary>
    public static class GuidProvider
    {
        public const int GuidLength = 6;

        private static readonly HashSet<string> usedGuids = new HashSet<string>();
        private static readonly Random random = new Random();

        public static string NewGuid()
        {
            string guid = "";

            var alreadyUsed = true;
            while (alreadyUsed)
            {
                guid = GenerateGuid();
                if (!usedGuids.Contains(guid))
                {
                    alreadyUsed = false;
                }
            }

            return guid;
        }

        private static string GenerateGuid()
        {
            return RandomAlphanumericString(GuidLength);
        }

        private static string RandomAlphanumericString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
