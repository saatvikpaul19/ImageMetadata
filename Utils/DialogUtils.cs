using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterGrade.Utils
{
    internal class DialogUtils
    {
        public string GetSelectedDirectory()
        {
            string SelectedDirectory = string.Empty;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SelectedDirectory = dialog.FileName;
            }
            return SelectedDirectory;
        }
    }
}
