using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
namespace TinyMetroWpfLibrary.Utility.ImageBuilder
{
    public enum ShapeBuilderType
    {
        Rectangle,
        ArrowLine,
        StartEllipse,
        RangeEllipse,
    }

    public class ShapeData
    {
        public Point Position { get; set; }
        public ShapeBuilderType ShapType { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public double Radius { get; set; }
        public Brush Brush { get; set; }
    }
}
