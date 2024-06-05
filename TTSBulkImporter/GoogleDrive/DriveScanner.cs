using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using File = Google.Apis.Drive.v3.Data.File;

namespace TTSBulkImporter.GoogleDrive
{
    /// <summary>
    /// This class offers methods to scan a user's Google Drive filesystem and enable file sharing.
    /// </summary>
    public class DriveScanner
    {
        private DriveService Service { get; }

        public DriveScanner(DriveService service)
        {
            Service = service;
        }

        /// <summary>
        /// Return the entire hierarchy of files and folders beginning at provided folder.
        /// Does not include trashed files and folders.
        /// </summary>
        public DriveFolder GetFilesystemFrom(string folderFileId)
        {
            var getRequest = Service.Files.Get(folderFileId);
            getRequest.Fields = "id, name, mimeType, trashed";

            // Validate root folder.
            var rootFile = getRequest.Execute();
            if (rootFile.Trashed.HasValue && (bool)rootFile.Trashed)
            {
                throw new ArgumentException("The provided folder is trashed.");
            }
            if (rootFile.MimeType != MimeTypes.Folder)
            {
                throw new ArgumentException("The provided folder is NOT a folder. It is a :" + rootFile.MimeType);
            }
            var rootDriveFolder = new DriveFolder(rootFile.Name, rootFile.Id);

            // Collect files and folders within root folder.
            var itemsInFolder = GetItemsInFolder(folderFileId);
            var files = itemsInFolder.Where(file => file.MimeType != MimeTypes.Folder);
            foreach (var file in files)
            {
                rootDriveFolder.Files.Add(new DriveFile(file.Name, file.Id));
            }
            var folders = itemsInFolder.Where(file => file.MimeType == MimeTypes.Folder);
            foreach (var folder in folders)
            {
                rootDriveFolder.Folders.Add(GetFilesystemFrom(folder.Id));
            }

            return rootDriveFolder;
        }

        /// <summary>
        /// Make all files and folders publicly shareable so that "Anyone with the link can view."
        /// This the same as clicking "Get shareable link" on a file in Google Drive.
        /// </summary>
        public void MakeFilesystemShareable(DriveFolder rootFolder)
        {
            GrantPermissionAnyoneWithLinkTo(rootFolder.Id);
            foreach (var file in rootFolder.Files)
            {
                GrantPermissionAnyoneWithLinkTo(file.Id);
            }
            foreach (var folder in rootFolder.Folders)
            {
                MakeFilesystemShareable(folder);
            }
        }

        /// <summary>
        /// Return the files and folders contained immediately within the provided folder.
        /// Does not include trashed files and folders.
        /// </summary>
        private IEnumerable<File> GetItemsInFolder(string folderFileId)
        {
            FilesResource.ListRequest listRequest = Service.Files.List();
            listRequest.Q = "parents in '" + folderFileId + "'";
            listRequest.Fields = "files(id, name, mimeType, trashed)";

            var files = listRequest.Execute().Files;
            var nonTrashedFiles = files.Where(file => file.Trashed.HasValue && !(bool)file.Trashed);

            return nonTrashedFiles;
        }

        /// <summary>
        /// Grants permission on the file so that "Anyone with the link can view." This the same as clicking "Get shareable link" on a file in Google Drive.
        /// </summary>
        /// <param name="fileId">The ID of the file.</param>
        private void GrantPermissionAnyoneWithLinkTo(string fileId)
        {
            var permission = new Permission();
            permission.Role = "reader";
            permission.Type = "anyone";

            Service.Permissions.Create(permission, fileId).Execute();

            Console.WriteLine("Created permission: 'Anyone with the link can view.' on file: " + fileId);
        }

        /// <summary>
        /// For testing only.
        /// </summary>
        public void PrettyPrintFileList(IEnumerable<Google.Apis.Drive.v3.Data.File> files)
        {
            Console.WriteLine("Files:");
            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    var fields = new List<string> { file.Name, file.MimeType };

                    var builder = new StringBuilder();
                    foreach (string item in fields)
                    {
                        builder.Append(item);
                        builder.Append("\t");
                    }
                    Console.WriteLine(builder.ToString());
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
        }
    }
}
