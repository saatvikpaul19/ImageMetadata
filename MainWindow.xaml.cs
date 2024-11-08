using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.ComponentModel;
using System.Reflection;
using CharacterGrade.Utils;

namespace CharacterGrade
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public bool IsHomePage { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        // Navigate to Page1.xaml
        private void GetStarted(object sender, RoutedEventArgs e)
        {
            DBLoader dBLoader = new();
            dBLoader.LoadFromDirectory();
            frame.Source = new Uri("Pages\\HomePage.xaml", UriKind.Relative);
        }

        //private string _selectedDirectory;
        //public string SelectedDirectory
        //{
        //    get { return _selectedDirectory; }
        //    set
        //    {
        //        if (_selectedDirectory != value)
        //        {
        //            _selectedDirectory = value;
        //            OnPropertyChanged(nameof(SelectedDirectory));
        //        }
        //    }
        //}

        public MainWindow()
        {
            InitializeComponent();
            frame.Source = new Uri("Pages\\HomePage.xaml", UriKind.Relative);
            DataContext = this;
            IsHomePage = true;
        }

        

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
