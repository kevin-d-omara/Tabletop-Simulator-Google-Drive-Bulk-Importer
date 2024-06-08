namespace TTSBulkImporter.GoogleDrive
{
    /// <summary>
    /// An item stored in Google Drive (typically a File or Folder).
    /// </summary>
    public abstract class DriveItem
    {
        public string Name { get; }
        public string Id { get; }

        public DriveItem(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public override string ToString()
        {
            return $"Type: {this.GetType()}, Name: {Name}, Id: {Id}";
        }
    }
}
