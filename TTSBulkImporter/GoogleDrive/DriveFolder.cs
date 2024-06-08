using System.Collections.Generic;

namespace TTSBulkImporter.GoogleDrive
{
    /// <summary>
    /// A Google Drive folder.
    /// </summary>
    public class DriveFolder : DriveItem
    {
        public ICollection<DriveFile> Files = new List<DriveFile>();
        public ICollection<DriveFolder> Folders = new List<DriveFolder>();

        public DriveFolder(string name, string id) : base(name, id) { }
    }
}
