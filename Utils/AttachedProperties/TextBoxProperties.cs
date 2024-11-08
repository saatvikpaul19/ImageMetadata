using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CharacterGrade.Utils.AttachedProperties
{
    public static class TextBoxProperties
    {
        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.RegisterAttached(
                "IsRequired", typeof(bool), typeof(TextBoxProperties),
                new PropertyMetadata(false));

        public static bool GetIsRequired(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRequiredProperty);
        }

        public static void SetIsRequired(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRequiredProperty, value);
        }
    }

}
