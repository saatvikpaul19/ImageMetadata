using CharacterGrade.Models;
using CharacterGrade.Models.Enums;
using CharacterGrade.Models.XMLSerialized;
using CharacterGrade.Utils;
using CharacterGrade.Utils.Extensions;
using CharacterGrade.ViewModel;
using CharacterGrade.Views;
using CommunityToolkit.Mvvm.Input;
using MetadataExtractor;
using Microsoft.WindowsAPICodePack.Dialogs;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CharacterGrade.Pages
{
    public partial class ImagePage : System.Windows.Controls.Page
    {
        ExifUtils exif;
        private static readonly Logger logger = LogManager.GetLogger("MetadataLogger");
        private int currentImageIndex;
        private string _selectedDirectory;
        List<string> imageTypes = new() { "*.jpg", "*.png", "*.jpeg" };
        private Dictionary<string, string> fileMetadata;

        private string imagePath;
        MetadataSerializer serializer;

        public List<string> ImageFiles;
        private MetadataCategory selectedMetadataType;
        private string xmlPath;
        private bool dropdownValuesAdded;

        public Metadatas metadataFields { get; private set; }
        public bool IsImageExists { get; private set; }
        public bool isFileOpen { get; private set; }
        public string SelectedDirectory
        {
            get { return _selectedDirectory; }
            private set
            {
                _selectedDirectory = value;
                tbFolderDirectory.Text = "Selected folder: " + _selectedDirectory;
                logger.Info("Selected folder: " + _selectedDirectory);
                IsImageExists = File.Exists(_selectedDirectory);
            }
        }

        public ImagePage(FileReturnObject folderDetails, MetadataCategory metadataType, bool isFileOpen)
        {
            InitializeComponent();
            exif = new();
            selectedMetadataType = metadataType;
            xmlPath = MetadataUtils.getMetadataXMLPath(selectedMetadataType);

            serializer = new();
            metadataFields = serializer.ReadMetadataFields(xmlPath);

            SelectedDirectory = folderDetails.FilePath;
            this.isFileOpen = isFileOpen;

            if (this.isFileOpen)
            {
                LoadImages();
            }
            else
            {
                LoadMetadataForFolder();
            }

        }

        private void NumericOnlyInt(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string newText = ((System.Windows.Controls.TextBox)sender).Text + e.Text;
            e.Handled = !IsTextNumeric(newText);
        }

        private static bool IsTextNumeric(string str)
        {
            if (int.TryParse(str, out int result))
            {
                // Check if the parsed integer is within the desired range
                return result >= 0 && result < int.MaxValue;
            }
            return false;
        }

        private void NumericOnlyDouble(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string newText = ((System.Windows.Controls.TextBox)sender).Text + e.Text;
            e.Handled = !IsTextDouble(newText);
        }

        private static bool IsTextDouble(string str)
        {
            if (double.TryParse(str, out double result))
            {
                // Adjust the desired range for double values if needed
                return result >= 0 && result <= double.MaxValue;
            }
            return false;
        }

        private void LoadImages()
        {
            ImageFiles = new List<string>();
            foreach (var typ in imageTypes)
            {
                ImageFiles.AddRange(System.IO.Directory.GetFiles(SelectedDirectory, typ));
            }
            //DirectoryInfo dInfo = new(SelectedDirectory);
            //ImageFiles.AddRange(ExtensionMethods.GetFilesByExtensions(dInfo, FileUtils.AcceptedImageExtensionsArray).Select(e => e.FullName));

            //ImageFiles.AddRange(ExtensionMethods.GetFilesByExtensions2(SelectedDirectory, FileUtils.AcceptedImageExtensionsArray));

            logger.Info("Number of images found in the selected directory: " + ImageFiles.Count);

            if (ImageFiles.Count == 0)
            {
                System.Windows.MessageBox.Show("No images found in the selected directory. Only .jpg, .jpeg, .png images are supported by the tool", "No images found", MessageBoxButton.OK, MessageBoxImage.Information);
                //this.NavigationService.Navigate(new HomePage());
                //Navigator.GoBack();
            }
            else
            {
                currentImageIndex = 0;
                DisplayCurrentImage();
            }

            cbFilesInFolder.ItemsSource = ImageFiles;
            cbFilesInFolder.SelectedIndex = currentImageIndex;

        }

        private void LoadMetadataForFolder()
        {
            tbFolder.Text = "Selected folder: " + SelectedDirectory;
            imagePath = SelectedDirectory;
            ImagePageViewModel vm = new()
            {
                Path = SelectedDirectory,
                Metadatas = new TrulyObservableCollection<MetadataModel>(),
                IsFile = false,
                SelectedMetadataType = selectedMetadataType
                //Files = new System.Collections.ObjectModel.ObservableCollection<string>(ImageFiles),
                //SelectedFile = SelectedImageFile
            };

            foreach (var field in metadataFields.Metadata)
            {
                var dropdownValues = field.Values != null ? field.Values.Value : new List<string>();
                var metadataModel = new MetadataModel()
                {
                    Key = field.Key,
                    Descriptor = field.Descriptor,
                    Type = field.Type.ToEnum(MetadataType.text),
                    Value = string.Empty,
                    DropdownValues = new ObservableCollection<string>(dropdownValues),
                    SelectedValue = string.Empty,
                    IsRequired = Convert.ToBoolean(field.Required)
                };
                if (metadataModel.Type == MetadataType.dropdown)
                {
                    metadataModel.AllowNewValues = field.Values.AllowNewValues != null ? Convert.ToBoolean(field.Values.AllowNewValues) : false;
                }

                vm.Metadatas.Add(metadataModel);

            }

            this.DataContext = vm;
        }

        private void DisplayCurrentImage(bool isSelectionChanged = true)
        {
            if (currentImageIndex >= 0 && currentImageIndex < ImageFiles.Count)
            {
                if (isSelectionChanged)
                {
                    cbFilesInFolder.SelectedIndex = currentImageIndex;
                }

                if (dropdownValuesAdded)
                {
                    metadataFields = serializer.ReadMetadataFields(xmlPath);
                    dropdownValuesAdded = false;
                }

                imagePath = ImageFiles[currentImageIndex];
                if (File.Exists(imagePath))
                {

                    try
                    {
                        string output = exif.ReadMetadata(imagePath);
                        fileMetadata = exif.ExtractKeyValuePairs(output, MetadataUtils.getReplacementText(selectedMetadataType));

                        ImagePageViewModel vm = new()
                        {
                            Path = imagePath,
                            Metadatas = new TrulyObservableCollection<MetadataModel>(),
                            IsFile = true,
                            Files = new ObservableCollection<string>(ImageFiles),
                            SelectedFile = imagePath,
                            SelectedMetadataType = selectedMetadataType
                        };

                        foreach (var field in metadataFields.Metadata)
                        {
                            fileMetadata.TryGetValue(field.Key, out string metadata);
                            var dropdownValues = field.Values != null ? field.Values.Value : new List<string>();
                            var metadataModel = new MetadataModel()
                            {
                                Key = field.Key,
                                Descriptor = field.Descriptor,
                                Type = field.Type.ToEnum(MetadataType.text),
                                Value = String.IsNullOrEmpty(metadata) ? string.Empty : metadata,
                                DropdownValues = new ObservableCollection<string>(dropdownValues),
                                SelectedValue = string.Empty,
                                IsRequired = Convert.ToBoolean(field.Required)
                            };
                            if(metadataModel.Type == MetadataType.dropdown)
                            {
                                metadataModel.SelectedValue = metadataModel.Value;
                                metadataModel.AllowNewValues = field.Values.AllowNewValues != null ? Convert.ToBoolean(field.Values.AllowNewValues) : false;
                            }
                            else
                            {
                                metadataModel.SelectedValue = string.Empty;
                            }

                            vm.Metadatas.Add(metadataModel);

                        }

                        this.DataContext = vm;

                        BitmapImage bi = new();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.UriSource = new Uri(imagePath);
                        bi.EndInit();
                        imgDisplay.Source = bi;

                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }

                }
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                currentImageIndex = (currentImageIndex - 1 + ImageFiles.Count) % ImageFiles.Count;
                DisplayCurrentImage();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                currentImageIndex = (currentImageIndex + 1) % ImageFiles.Count;
                DisplayCurrentImage();
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var metadataToWrite = (DataContext as ImagePageViewModel).Metadatas;
            bool allRequiredMetadataHasNoValue = metadataToWrite.Any(m => m.IsRequired == true && (m.Type == MetadataType.dropdown ? string.IsNullOrEmpty(m.SelectedValue) : string.IsNullOrEmpty(m.Value)));
            if (allRequiredMetadataHasNoValue)
            {
                System.Windows.MessageBox.Show("All fields marked as required (red highlight) must have a value", "Missing data in mandatory fields", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                try
                {
                    string output = exif.WriteMetadata(imagePath, metadataToWrite.ToList());
                    System.Windows.MessageBox.Show(output, "Metadata writing complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    System.Windows.MessageBox.Show("Error writing metadata", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }

        }

        private void cbFilesInFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentImageIndex = cbFilesInFolder.SelectedIndex;

            DisplayCurrentImage(false);
        }

        private void Save()
        {
            // Your logic for saving the object goes here
            var parentDataContext = this.DataContext as MetadataModel;
            var viewModel = new MetadataDialogVM(parentDataContext.Descriptor);
            var customDialog = new MetadataDialog(viewModel) { DataContext = viewModel };
            bool? result = customDialog.ShowDialog();

            if (result == true)
            {
                serializer.UpdateDropdownXMLElement(xmlPath, parentDataContext.Key, viewModel.UserInput);

                parentDataContext.DropdownValues.Append(viewModel.UserInput);
            }
        }

        private void btnAutoFill_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMetadataType == MetadataCategory.Character)
            {
                if (isFileOpen)
                {
                    string imageFileName = System.IO.Path.GetFileNameWithoutExtension(imagePath);

                    var fileDetails = getDetailsFromFile(imageFileName);
                    if (fileDetails.FileDetailsExtracted)
                    {
                        var metadataToWrite = (DataContext as ImagePageViewModel).Metadatas;

                        metadataToWrite.First(m => m.Key == "Line_Number").Value = fileDetails.LineNumber;
                        metadataToWrite.First(m => m.Key == "Character_Number").Value = fileDetails.PosNumber;
                        metadataToWrite.First(m => m.Key == "ISO15919_Alphabet").Value = fileDetails.Character;
                    }
                    //string output = exif.WriteMetadata(imagePath, metadataToWrite.ToList());
                    //logger.Info("File name: " + imageFileName + "\n Exif response: " + output);
                    //System.Windows.MessageBox.Show(output, "Metadata writing complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ImageFiles = new List<string>();
                    foreach (var typ in imageTypes)
                    {
                        ImageFiles.AddRange(System.IO.Directory.GetFiles(SelectedDirectory, typ));
                    }

                    try
                    {
                        var metadataToWrite = (DataContext as ImagePageViewModel).Metadatas;
                        int fileDetailsWritten = 0;
                        foreach (string imageFileName in ImageFiles)
                        {
                            string fileName = System.IO.Path.GetFileNameWithoutExtension(imageFileName);

                            var fileDetails = getDetailsFromFile(fileName);
                            if (fileDetails.FileDetailsExtracted)
                            {
                                metadataToWrite.First(m => m.Key == "Line_Number").Value = fileDetails.LineNumber;
                                metadataToWrite.First(m => m.Key == "Character_Number").Value = fileDetails.PosNumber;
                                metadataToWrite.First(m => m.Key == "ISO15919_Alphabet").Value = fileDetails.Character;

                                string output = exif.WriteMetadata(imageFileName, metadataToWrite.ToList());
                                logger.Info("File name: " + imageFileName + "\n Exif response: " + output);
                                fileDetailsWritten++;
                            }
                        }

                        metadataToWrite.First(m => m.Key == "Line_Number").Value = string.Empty;
                        metadataToWrite.First(m => m.Key == "Character_Number").Value = string.Empty;
                        metadataToWrite.First(m => m.Key == "ISO15919_Alphabet").Value = string.Empty;

                        System.Windows.MessageBox.Show(fileDetailsWritten + " files updated", "Metadata writing complete", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        System.Windows.MessageBox.Show("Error writing metadata", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
            }
            else if (selectedMetadataType == MetadataCategory.Image)
            {
                if (isFileOpen)
                {
                    var imgPixels = GetPixelsFromFile(imagePath);

                    var metadataToWrite = (DataContext as ImagePageViewModel).Metadatas;

                    metadataToWrite.First(m => m.Key == "Pixels_X").Value = imgPixels.Width.ToString();
                    metadataToWrite.First(m => m.Key == "Pixels_Y").Value = imgPixels.Height.ToString();
                }
                else
                {
                    ImageFiles = new List<string>();
                    foreach (var typ in imageTypes)
                    {
                        ImageFiles.AddRange(System.IO.Directory.GetFiles(SelectedDirectory, typ));
                    }

                    try
                    {
                        var metadataToWrite = (DataContext as ImagePageViewModel).Metadatas;

                        foreach (string imageFileName in ImageFiles)
                        {
                            var imgPixels = GetPixelsFromFile(imageFileName);
                            metadataToWrite.First(m => m.Key == "Pixels_X").Value = imgPixels.Width.ToString();
                            metadataToWrite.First(m => m.Key == "Pixels_Y").Value = imgPixels.Height.ToString();

                            string output = exif.WriteMetadata(imageFileName, metadataToWrite.ToList());
                            logger.Info("File name: " + imageFileName + "\n Exif response: " + output);
                        }

                        metadataToWrite.First(m => m.Key == "Pixels_X").Value = string.Empty;
                        metadataToWrite.First(m => m.Key == "Pixels_Y").Value = string.Empty;

                        System.Windows.MessageBox.Show(ImageFiles.Count + " files updated", "Metadata writing complete", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        System.Windows.MessageBox.Show("Error writing metadata", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void AutofillAndSubmit()
        {

        }

        private ImagePixels GetPixelsFromFile2(string fileName)
        {
            BitmapImage bi = new();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(fileName);
            int wid = (int)bi.Width;
            int hig = (int)bi.Height;
            bi.EndInit();

            return new ImagePixels(wid, hig);
        }

        private ImagePixels GetPixelsFromFile(string fileName)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(fileName);

            int wid = img.Width;
            int hig = img.Height;

            return new ImagePixels(wid, hig);
        }

        private FileNameDetails getDetailsFromFile(string fileName)
        {
            logger.Info("Extracting metadata from file name: " + fileName);
            char delimiter = '_';

            try
            {
                string[] elements = fileName.Split(delimiter);
                return new FileNameDetails(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5], true);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                System.Windows.MessageBox.Show("Character metadata could not be extracted from file name", "Metadata extraction failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return new FileNameDetails(false);
            }
        }

        private void AddNewDropdownValue_Click(object sender, RoutedEventArgs e)
        {
            var textBox = (System.Windows.Controls.Button)sender;
            var parentContentControl = FindParent<ContentControl>(textBox);

            //if (parentContentControl != null)
            var parentDataContext = parentContentControl.DataContext as MetadataModel;

            var viewModel = new MetadataDialogVM(parentDataContext.Descriptor);
            var customDialog = new MetadataDialog(viewModel) { DataContext = viewModel };
            bool? result = customDialog.ShowDialog();

            if (result == true)
            {
                serializer.UpdateDropdownXMLElement(xmlPath, parentDataContext.Key, viewModel.UserInput);
                if (!string.IsNullOrEmpty(viewModel.UserInput))
                {
                    var newList = parentDataContext.DropdownValues.Append(viewModel.UserInput);
                    parentDataContext.DropdownValues = new ObservableCollection<string>(newList);
                    dropdownValuesAdded = true;
                }

            }
        }


        // Helper method to find the parent of a specific type
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }


    }
}
