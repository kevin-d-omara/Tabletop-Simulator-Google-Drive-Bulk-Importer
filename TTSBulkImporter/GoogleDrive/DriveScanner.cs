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
        /// Get the entire hierarchy of files and folders nested within the provided folder.
        /// Excludes trashed files and folders.
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
        /// This the same as clicking "Get shareable link" on each file and folder in Google Drive.
        /// </summary>
        public void MakeFilesystemShareable(DriveFolder rootFolder)
        {
            BatchGrantPermissionAnyoneWithLinkTo(rootFolder);

            foreach (var folder in rootFolder.Folders)
            {
                MakeFilesystemShareable(folder);
            }
        }

        /// <summary>
        /// Get the files and folders contained immediately within the provided folder (i.e. not including nested items).
        /// Excludes trashed items.
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

        // TODO: Observe limits:
        // Max 8,000 character limit on length of URL for each "inner request" (from https://developers.google.com/drive/api/guides/performance#batch-requests)

        /// <summary>
        /// Grant permission to the provided folder and it's contained files (excluding contained folders).
        /// Permission is for "Anyone with the link can view." This the same as clicking "Get shareable link" on each of those files in Google Drive.
        /// </summary>
        private void BatchGrantPermissionAnyoneWithLinkTo(DriveFolder folder)
        {
            Console.WriteLine("");
            Console.WriteLine($"Changing permission of folder \"{folder.Name}\" (ID: {folder.Id}) and it's {folder.Files.Count} contained files.");
            Console.WriteLine($"(This may take up to 30 seconds. Batch Size: {MaxRequestsPerBatch} items.)");

            List<DriveItem> files = new List<DriveItem>();
            files.Add(folder);
            files.AddRange(folder.Files);

            bool isFirstChunk = true;
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

                Console.WriteLine($"Success!");
                if (isFirstChunk)
                {
                    Console.WriteLine($"Changed permission of 1 folder and {chunk.Length - 1} files to: 'Anyone with the link can view.' =>");
                    Console.WriteLine($"Folder: {chunk[0].Name}, ID: {chunk[0].Id}");
                    foreach (var file in chunk.Skip(1))
                    {
                        Console.WriteLine($"File: {file.Name}, ID: {file.Id}");
                    }

                    isFirstChunk = false;
                }
                else
                {
                    Console.WriteLine($"Changed permission of {chunk.Length} files to: 'Anyone with the link can view.' =>");
                    foreach (var file in chunk)
                    {
                        Console.WriteLine($"File: {file.Name}, ID: {file.Id}");
                    }
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
