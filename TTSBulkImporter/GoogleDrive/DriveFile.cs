namespace TTSBulkImporter.GoogleDrive
{
    /// <summary>
    /// A Google Drive file.
    /// </summary>
    public class DriveFile : DriveItem
    {
        /// <summary>
        /// The link to download this file directly. Note: must enable public permissions separately.
        /// </summary>
        public string DownloadLink => @"https://drive.google.com/uc?export=download&id=" + Id;

        public DriveFile(string name, string id) : base(name, id) { }
    }
}
