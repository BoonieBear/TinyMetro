using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
namespace TinyMetroWpfLibrary.Utility.ImageBuilder
{
    public class EllipseBuilder : ShapeBulder
    {
        public Point Position { get; set; }
        public double Radius { get; set; }
        
        public override void Drawing(System.Windows.Media.DrawingContext drawingContext, System.Windows.Rect area)
        {
            drawingContext.DrawEllipse(ShapeBrush, DrawPen, Position, Radius, Radius);
        }
    }
}
