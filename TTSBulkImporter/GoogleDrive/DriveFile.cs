namespace TTSBulkImporter.GoogleDrive
{
    /// <summary>
    /// A Google Drive folder.
    /// </summary>
    public class DriveFile
    {
        public string Name { get; }
        public string Id { get; }

        /// <summary>
        /// The link to download this file directly. Note: must enable public permissions separately.
        /// </summary>
        public string DownloadLink => @"https://drive.google.com/uc?export=download&id=" + Id;

        public DriveFile(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public override string ToString()
        {
            return typeof(DriveFile) + ", Name: " + Name + ", Id: " + Id;
        }
    }
}
