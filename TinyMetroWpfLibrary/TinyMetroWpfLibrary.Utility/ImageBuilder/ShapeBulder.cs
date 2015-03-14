using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TinyMetroWpfLibrary.Utility.ImageBuilder
{
    public abstract class ShapeBulder
    {
        private Brush _shapeBrush;
        public Brush ShapeBrush
        {
            get { return _shapeBrush; }
            set { _shapeBrush = value; }
        }

        private Pen _drawPen;
        public Pen DrawPen
        {
            get { return _drawPen; }
            set { _drawPen = value; }
        }

        public abstract void Drawing(DrawingContext drawingContext, Rect area);
        
    }
}
