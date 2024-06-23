using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TTSBulkImporter.Importer
{
    /// <summary>
    /// An <see cref="IComparer{T}"/> for performing a "natural sort", as is used in Windows Explorer for listing filenames.
    /// </summary>
    /// <seealso cref="https://stackoverflow.com/a/73658647/5886427"/>
    public sealed class NaturalComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (x is null)
                return -1;
            if (y is null)
                return +1;

            var itemsX = Regex
              .Split(x, "([0-9]+)")
              .Where(item => !string.IsNullOrEmpty(item))
              .ToList();

            var itemsY = Regex
              .Split(y, "([0-9]+)")
              .Where(item => !string.IsNullOrEmpty(item))
              .ToList();

            for (int i = 0; i < Math.Min(itemsX.Count, itemsY.Count); ++i)
            {
                int result = CompareChunks(itemsX[i], itemsY[i]);

                if (result != 0)
                    return result;
            }

            return itemsX.Count.CompareTo(itemsY.Count);
        }

        private static int CompareChunks(string x, string y)
        {
            if (x[0] >= '0' && x[0] <= '9' && y[0] >= '0' && y[0] <= '9')
            {
                string tx = x.TrimStart('0');
                string ty = y.TrimStart('0');

                int result = tx.Length.CompareTo(ty.Length);

                if (result != 0)
                    return result;

                result = tx.CompareTo(ty);

                if (result != 0)
                    return result;
            }

            return string.Compare(x, y);
        }
    }
}
