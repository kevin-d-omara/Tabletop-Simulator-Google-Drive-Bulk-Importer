using System.Collections.Generic;

namespace TTSBulkImporter.GoogleDrive
{
    /// <summary>
    /// A Google Drive folder.
    /// </summary>
    public class DriveFolder
    {
        public string Name { get; }
        public string Id { get; }

        public ICollection<DriveFile> Files = new List<DriveFile>();
        public ICollection<DriveFolder> Folders = new List<DriveFolder>();

        public DriveFolder(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public override string ToString()
        {
            return typeof(DriveFolder) + ", Name: " + Name + ", Id: " + Id;
        }
    }
}
