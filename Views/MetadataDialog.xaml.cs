using CharacterGrade.ViewModel;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace CharacterGrade.Views
{
    /// <summary>
    /// Interaction logic for MetadataDialog.xaml
    /// </summary>
    public partial class MetadataDialog : Window
    {
        public MetadataDialog(MetadataDialogVM dialogVM)
        {
            InitializeComponent();
            DataContext = dialogVM;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
