using System.IO;

namespace TTSBulkImporter.TabletopSimulator.Utils
{
    public static class Filesystem
    {
        /// <summary>
        /// Get the root directory of the .NET solution. This is the folder containing the ".sln" file.
        /// </summary>
        public static DirectoryInfo GetSolutionRootDirectory()
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            return currentDirectory.Parent.Parent.Parent.Parent;
        }
    }
}
