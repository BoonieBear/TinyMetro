using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TinyMetroWpfLibrary.Controls.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isTrue = (bool)value;
            string para = parameter as string;

            if (para == "TrueToVisible")
            {
                return isTrue ? Visibility.Visible : Visibility.Collapsed;   
            }
            else
            {
                return isTrue ? Visibility.Collapsed : Visibility.Visible;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
