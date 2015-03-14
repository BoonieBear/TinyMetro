using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
namespace TinyMetroWpfLibrary.Utility.ImageBuilder
{
    public class RectangleBuilder : ShapeBulder
    {
        public override void Drawing(System.Windows.Media.DrawingContext drawingContext, Rect area)
        {
            drawingContext.DrawRectangle(ShapeBrush, DrawPen, area);
        }
    }
}
