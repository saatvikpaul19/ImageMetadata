using CharacterGrade.Models;
using CharacterGrade.Models.Enums;
using CharacterGrade.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterGrade.ViewModel
{
    public class ImagePageViewModel : INotifyPropertyChanged
    {
        public string Path { get; set; }
        public bool IsFile { get; set; }
        public ObservableCollection<string> Files { get; set; }

        private string selectedFile;

        public string SelectedFile
        {
            get { return selectedFile; }
            set
            {
                selectedFile = value;
            }
        }

        private TrulyObservableCollection<MetadataModel> _metadatas;
        public TrulyObservableCollection<MetadataModel> Metadatas
        {
            get => _metadatas;
            set
            {
                _metadatas = value;
                OnPropertyChanged(nameof(Metadatas));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MetadataCategory selectedMetadataType;

        public MetadataCategory SelectedMetadataType
        {
            get { return selectedMetadataType; }
            set
            {
                if (selectedMetadataType != value)
                {
                    selectedMetadataType = value;
                    OnPropertyChanged(nameof(SelectedMetadataType));
                }
            }
        }

    }
}
