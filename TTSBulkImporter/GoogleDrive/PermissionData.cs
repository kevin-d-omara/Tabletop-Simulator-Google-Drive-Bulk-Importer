namespace TTSBulkImporter.GoogleDrive
{
    /// <summary>
    /// Magic strings needed for <see cref="Google.Apis.Drive.v3.Data.Permission"/>.
    /// 
    /// Strings taken from <a href="https://developers.google.com/drive/api/v3/reference/permissions">Google Drive API: Permissions page</a>.
    /// </summary>
    public static class PermissionData
    {
        /// <summary>
        /// Identifies what kind of resource this is.
        /// </summary>
        public const string Kind = "drive#permission";

        /// <summary>
        /// The role granted by this permission. While new values may be supported in the future, the following are currently allowed:
        /// </summary>
        public static class Role
        {
            public const string Owner = "owner";
            public const string Organizer = "organizer";
            public const string FileOrganizer = "fileOrganizer";
            public const string Writer = "writer";
            public const string Commenter = "commenter";
            public const string Reader = "reader";
        }

        /// <summary>
        /// The type of the grantee. Valid values are:
        /// </summary>
        public static class Type
        {
            public const string User = "user";
            public const string Group = "group";
            public const string Domain = "domain";
            public const string Anyone = "anyone";
        }
    }
}
