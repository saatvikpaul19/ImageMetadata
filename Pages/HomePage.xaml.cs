using CharacterGrade.Models;
using CharacterGrade.Models.Enums;
using CharacterGrade.Pages;
using CharacterGrade.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CharacterGrade
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MetadataCategory _selectedCategoryType;

        public HomePage()
        {
            InitializeComponent();
            DataContext = this;
            SelectedCategory = MetadataCategory.Inscription;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            NavigateToImageScreen(true);
        }

        private void btnFolderSelect_Click(object sender, RoutedEventArgs e)
        {
            NavigateToImageScreen(false);
        }
        private void OpenLogFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string startupPath = AppDomain.CurrentDomain.BaseDirectory;

                string textFilePath = System.IO.Path.Combine(startupPath, "metadata.log");

                if (File.Exists(textFilePath))
                {
                    Process.Start("notepad.exe", textFilePath);
                }
                else
                {
                    MessageBox.Show("The text file does not exist.", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening the text file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToImageScreen(bool isFolderSelected)
        {
            var folderDetails = FileUtils.LoadFromDirectory();

            if (!String.IsNullOrEmpty(folderDetails.FilePath))
            {
                //if (ExtensionMethods.GetFilesByExtensions2(folderDetails.FilePath, FileUtils.AcceptedImageExtensionsArray).Any())
                this.NavigationService.Navigate(new ImagePage(folderDetails, SelectedCategory, isFolderSelected));
                //else
                //    MessageBox.Show("No images found in the selected directory. Only .jpg, .jpeg, .png images are supported by the tool", "No images found", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private string metadataDescription;

        public string MetadataDescription
        {
            get { return metadataDescription; }
            set
            {
                metadataDescription = value;
                OnPropertyChanged(nameof(MetadataDescription));
            }
        }


        public MetadataCategory SelectedCategory
        {
            get { return _selectedCategoryType; }
            set
            {
                _selectedCategoryType = value;
                MetadataDescription = _selectedCategoryType switch
                {
                    MetadataCategory.Inscription => "Apply metadata related to an inscription such as script, language, material, date and measurements etc.",
                    MetadataCategory.Character => "Apply metadata related to characters in an inscription such as alphabet, line number and character position etc.",
                    MetadataCategory.Image => "Apply metadata related to an image such as pixels dimensions",
                    _ => "",
                };
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public IEnumerable<MetadataCategory> MetadataCategories
        {
            get
            {
                return Enum.GetValues(typeof(MetadataCategory))
                    .Cast<MetadataCategory>();
            }
        }
    }
}
