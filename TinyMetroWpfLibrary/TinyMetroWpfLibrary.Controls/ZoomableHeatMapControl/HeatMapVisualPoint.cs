using System.Windows;
using System.Windows.Media;

namespace TinyMetroWpfLibrary.Controls.ZoomableHeatMapControl
{
    public class HeatMapVisualPoint
    {
        public Point VisualPoint { get; set; }
        public bool IsDashArrowPoint { get; set; }
        public bool IsNeedRangeIndicator { get; set; }
        public Brush Brush { get; set; }
    }
}
