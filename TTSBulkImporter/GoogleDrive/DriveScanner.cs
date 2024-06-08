using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Requests;
using File = Google.Apis.Drive.v3.Data.File;

namespace TTSBulkImporter.GoogleDrive
{
    /// <summary>
    /// This class offers methods to scan a user's Google Drive filesystem and enable file sharing.
    /// </summary>
    public class DriveScanner
    {
        // Source: https://developers.google.com/drive/api/guides/performance#batch-requests
        private const int MaxRequestsPerBatch = 100;

        private DriveService DriveService { get; }

        public DriveScanner(DriveService service)
        {
            DriveService = service;
        }

        /// <summary>
        /// Return the entire hierarchy of files and folders beginning at provided folder.
        /// Does not include trashed files and folders.
        /// </summary>
        public DriveFolder GetFilesystemFrom(string folderFileId)
        {
            var getRequest = DriveService.Files.Get(folderFileId);
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
        /// Make all files and folders (including nested ones) publicly shareable so that "Anyone with the link can view."
        /// This the same as clicking "Get shareable link" on each file and fodler in Google Drive.
        /// </summary>
        public void MakeFilesystemShareable(DriveFolder rootFolder)
        {
            List<DriveObject> filesAndFolders = new List<DriveObject>();
            filesAndFolders.Add(rootFolder);
            filesAndFolders.AddRange(rootFolder.Files);

            BatchGrantPermissionAnyoneWithLinkTo(filesAndFolders);

            foreach (var folder in rootFolder.Folders)
            {
                MakeFilesystemShareable(folder);
            }
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

        /// <summary>
        /// Return the files and folders contained immediately within the provided folder (i.e. non-recursively).
        /// Does not include trashed files.
        /// </summary>
        private IEnumerable<File> GetItemsInFolder(string folderFileId)
        {
            FilesResource.ListRequest request = DriveService.Files.List();
            request.PageSize = 1000;
            request.Q = $"'{folderFileId}' in parents and trashed=false";
            request.Fields = "files(id, name, mimeType)";

            var response = request.Execute();

            return response.Files;
        }

        /// <summary>
        /// Grants permission to all the provided files so that "Anyone with the link can view." This the same as clicking "Get shareable link" on each of those files in Google Drive.
        /// </summary>
        /// <param name="files">The files and folders to change permission for.</param>
        private void BatchGrantPermissionAnyoneWithLinkTo(ICollection<DriveObject> files)
        {
            // TODO: Observe limits:
            // Max 8,000 character limit on length of URL for each "inner request" (from https://developers.google.com/drive/api/guides/performance#batch-requests)

            foreach (var chunk in files.Chunk(MaxRequestsPerBatch))
            {
                // Queue Requests:
                var batchRequest = new BatchRequest(DriveService);
                BatchRequest.OnResponse<Permission> callback = delegate (
                    Permission permission,
                    RequestError error,
                    int index,
                    HttpResponseMessage message)
                {
                    if (error != null)
                    {
                        throw new InvalidOperationException(error.Message);
                    }
                };

                var filePermission = CreatePublicReadonlyPermission();

                foreach (var file in chunk)
                {
                    var request = DriveService.Permissions.Create(filePermission, file.Id);
                    batchRequest.Queue(request, callback);
                }

                // Execute Requests Batch:
                var task = batchRequest.ExecuteAsync();
                task.Wait();

                Console.WriteLine($"Changed permission of {files.Count} files to: 'Anyone with the link can view.'");
                Console.WriteLine($"Files/Folders:");
                foreach (var file in files)
                {
                    Console.WriteLine($"Name: {file.Name}, ID: {file.Id}");
                }
            }
        }

        /// <summary>
        /// Grants permission on the file so that "Anyone with the link can view." This the same as clicking "Get shareable link" on the file in Google Drive.
        /// </summary>
        /// <param name="fileId">The ID of the file.</param>
        private void GrantPermissionAnyoneWithLinkTo(string fileId)
        {
            var permission = CreatePublicReadonlyPermission();

            var request = DriveService.Permissions.Create(permission, fileId);
            request.Execute();

            Console.WriteLine("Created permission: 'Anyone with the link can view.' on file: " + fileId);
        }

        private Permission CreatePublicReadonlyPermission()
        {
            return new Permission()
            {
                Type = "anyone",
                Role = "reader"
            };
        }
    }
}
