using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CharacterGrade.ViewModel
{
    public class MetadataDialogVM : INotifyPropertyChanged
    {
        private string metadataKey;

        public string MetadataKey
        {
            get { return metadataKey; }
            set { metadataKey = value; }
        }

        private string _userInput;
        public MetadataDialogVM(string key)
        {
            MetadataKey = key;
            //OkCommand = new relaycommand();
        }

        public string UserInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                //OnPropertyChanged(nameof(UserInput));
            }
        }

        public ICommand OkCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
