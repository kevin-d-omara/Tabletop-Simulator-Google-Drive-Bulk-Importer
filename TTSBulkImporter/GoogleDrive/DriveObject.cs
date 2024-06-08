namespace TTSBulkImporter.GoogleDrive
{
    /// <summary>
    /// An object stored in Google Drive (typically a File or Folder).
    /// </summary>
    public abstract class DriveObject
    {
        public string Name { get; }
        public string Id { get; }

        public DriveObject(string name, string id)
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
