using CharacterGrade.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CharacterGrade
{
    internal class DBLoader
    {
        private string SelectedDirectory = string.Empty;

        public bool RemoveFolderFromDB(int folderId)
        {
            bool returnSuccess = false;

            using (var context = new FileContext())
            {
                var filesToDelete = context.Files.Where(f => f.FolderMetadataId == folderId);
                context.Files.RemoveRange(filesToDelete);

                var folderToDelete = context.Folders.FirstOrDefault(f => f.Id == folderId);
                context.Folders.Remove(folderToDelete);

                context.SaveChanges();
                returnSuccess = context.SaveChanges() > 0;
            }
            return returnSuccess;
        }

        public List<FileMetadata> GetFilesFromDB(int folderId)
        {
            using (var context = new FileContext())
            {
                var files = context.Files.Where(f => f.FolderMetadataId == folderId).ToList();
                return files;
            }
        }

        public bool RemoveFileFromDB(int fileId)
        {
            bool returnSuccess = false;
            using (var context = new FileContext())
            {
                var filesToDelete = context.Files.FirstOrDefault(f => f.FolderMetadataId == fileId);
                context.Files.Remove(filesToDelete);

                returnSuccess = context.SaveChanges() > 0;
            }
            return returnSuccess;
        }

        public bool UpdateFileMetadata(int fileId, string metadata)
        {
            bool returnSuccess = false;
            using (var context = new FileContext())
            {
                var fileMetadata = context.Files.FirstOrDefault(f => f.Id == fileId);

                if (fileMetadata != null)
                {
                    fileMetadata.Metadata = metadata;
                    returnSuccess = context.SaveChanges() > 0;
                }
            }
            return returnSuccess;
        }

        public FileReturnObject LoadFromDirectory()
        {
            FileReturnObject returnObject = new FileReturnObject();
            CommonOpenFileDialog dialog = new()
            {
                InitialDirectory = "C:\\Users",
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SelectedDirectory = dialog.FileName;
                //string selectedDirectory = Path.GetDirectoryName(selectedFilePath);


                using (var context = new FileContext())
                {
                    // Create the database if it doesn't exist
                    context.Database.EnsureCreated();

                    if (!context.Folders.Any(f => f.Path == SelectedDirectory))
                    {
                        var dbFolder = new FolderMetadata { Guid = Guid.NewGuid(), Path = SelectedDirectory, Metadata = "some metadata" };

                        context.Folders.Add(dbFolder);
                        var addedFolder = context.SaveChanges();

                        WriteNewFiles(context, dbFolder.Id);
                        //LoadImages(SelectedDirectory);
                        returnObject = new FileReturnObject() { FilePath = SelectedDirectory, FolderMetadataId = dbFolder.Id };

                        MessageBox.Show("folder saved to db");

                    }
                    else
                    {
                        var result = MessageBox.Show("folder already exists to db", "This is the caption text.", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                        if (result == MessageBoxResult.Yes)
                        {
                            var existingFolder = context.Folders.First(f => f.Path == SelectedDirectory);

                            var filesToDelete = context.Files.Where(f => f.Path == existingFolder.Path);
                            // Remove the files from the table
                            context.Files.RemoveRange(filesToDelete);
                            context.SaveChanges();

                            returnObject = new FileReturnObject() { FilePath = SelectedDirectory, FolderMetadataId = existingFolder.Id };

                            WriteNewFiles(context, existingFolder.Id);
                        }


                    }

                }
            }
            return returnObject;
        }

        private int WriteNewFiles(FileContext context, int folderMetadata)
        {
            // Get all files in the folder and its subfolders
            var files = Directory.GetFiles(SelectedDirectory, "*", SearchOption.AllDirectories);

            // Add each file to the database
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var dbFile = new FileMetadata
                {
                    Guid = Guid.NewGuid(),
                    Name = fileInfo.Name,
                    Path = fileInfo.FullName,
                    Metadata = "some metadata",
                    FolderMetadataId = folderMetadata
                };
                context.Files.Add(dbFile);
            }

            return context.SaveChanges();
        }
    }
}
