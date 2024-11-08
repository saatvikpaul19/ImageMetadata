using CharacterGrade.Pages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CharacterGrade.Converters
{
    public class ImageSourceConverter : IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is string)
            //{
            //    return new BitmapImage(new Uri((string)value));
            //}
            //return new BitmapImage(new Uri("Resources//no-image-found.jpg"));

            

            var path = parameter as string;
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(path + (string)value, UriKind.Relative);
            img.EndInit();
            return img;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri((string)values[1] + (string)values[0], UriKind.Relative);
            img.EndInit();
            return img;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
