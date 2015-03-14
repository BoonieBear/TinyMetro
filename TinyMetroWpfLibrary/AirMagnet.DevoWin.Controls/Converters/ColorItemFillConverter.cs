using AirMagnet.AircheckWifiTester.Utils;
using AirMagnet.LogUtil;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace AirMagnet.AircheckWifiTester.Controls
{
    public class ColorItemFillConverter : IMultiValueConverter
    {
        private static ILogService _logger = new FileLogService(typeof(ColorItemFillConverter));
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                int index = (int)values[0];
                double upperValue = (double)(values[1]);
                double SignalLowerValue = (double)values[2];
                bool isSignalStyle = (bool)values[3];
                if ((index >= upperValue) || (index < SignalLowerValue))
                {
                    return Brushes.Gray;
                }
                if (isSignalStyle)
                {
                    return new SolidColorBrush(SignalColorMap.Instance.GetColorByColorLegendIndex(index));
                }
                else
                {
                    return new SolidColorBrush(ThroughputColorMap.Instance.GetColorByColorLegendIndex(index));
                }
              
            }
            catch(Exception ex)
            {
                _logger.Error("ColorLegend", ex);
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
