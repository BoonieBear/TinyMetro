using System;
using System.Globalization;
using System.Windows;

namespace TinyMetroWpfLibrary.Controls.Converters
{
    public sealed class MessageBoxButtonVisibilityConverter : MarkupConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var buttons = (MessageBoxButton)value;
            var para = (string)parameter;
            Visibility visibility = Visibility.Collapsed;
            switch (para)
            {
                case "YES":
                case "NO":
                    if ((buttons == MessageBoxButton.YesNo)
                        || (buttons == MessageBoxButton.YesNoCancel))
                    {
                        visibility = Visibility.Visible;
                    }
                    break;

                case "OK":
                    if (buttons == MessageBoxButton.OK || buttons == MessageBoxButton.OKCancel)
                    {
                        visibility = Visibility.Visible;
                    }
                    break;
                case "CANCEL":
                    if (buttons == MessageBoxButton.OKCancel)
                    {
                        visibility = Visibility.Visible;
                    }
                    break;
                default:
                    break;
            }

            return visibility;
        }

        /// <summary>
        ///     converts from visibility to boolean
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility)value;
            return (visibility == Visibility.Visible);
        }
    }
}
