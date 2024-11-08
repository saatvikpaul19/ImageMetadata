using CharacterGrade.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterGrade.Utils
{
    internal class FileUtils
    {
        public static string AcceptedImageExtensions = "*.jpg | *.jpeg | *.png";

        public static string[] AcceptedImageExtensionsArray = { "*.jpg ", "*.jpeg", "*.png" };
        public static FileReturnObject LoadFromDirectory()
        {
            string SelectedDirectory = string.Empty;

            CommonOpenFileDialog dialog = new()
            {
                InitialDirectory = "C:\\Users",
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SelectedDirectory = dialog.FileName;
            }
            FileReturnObject returnObject = new() { FilePath = SelectedDirectory, FolderMetadataId = -1 };

            return returnObject;

        }
    }
}
