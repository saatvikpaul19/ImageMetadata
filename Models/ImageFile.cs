using CharacterGrade.Models.Enums;
using CharacterGrade.Models.XMLSerialized;
using CharacterGrade.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterGrade.Models
{
    public class MetadataModel : INotifyPropertyChanged
    {
        public string Key { get; set; }
        public MetadataType Type { get; set; }
        public string Descriptor { get; set; }

        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(_value));
            }
        }


        public bool IsRequired { get; set; }
        public bool AllowNewValues { get; set; }


        private ObservableCollection<string> _dropdownValues;
        public ObservableCollection<string> DropdownValues
        {
            get => _dropdownValues;
            set
            {
                _dropdownValues = value;
                OnPropertyChanged(nameof(_dropdownValues));
            }
        }


        public string SelectedValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
