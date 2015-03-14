using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
namespace AirMagnet.AircheckWifiTester.Controls
{
    public class HeatMapVisualPoint
    {
        public Point VisualPoint { get; set; }
        public bool IsDashArrowPoint { get; set; }
        public bool IsNeedRangeIndicator { get; set; }
        public Brush Brush { get; set; }
    }
}
