using System.Windows;
using System.Windows.Controls;
using TinyMetroWpfLibrary.Controls.Utils;

namespace TinyMetroWpfLibrary.Controls.ZoomableHeatMapControl
{
    public class FixSizeImage : Image
    {
        public double DeminsionWidth
        {
            get { return (double)GetValue(DeminsionWidthProperty); }
            set { SetValue(DeminsionWidthProperty, value); }
        }

        public FixSizeImage()
        {
            this.Loaded += OnImageLoaded;
        }

      

        // Using a DependencyProperty as the backing store for DeminsionWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeminsionWidthProperty =
            DependencyProperty.Register("DeminsionWidth",
                                        typeof(double),
                                        typeof(FixSizeImage),
                                        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double DeminsionHeight
        {
            get { return (double)GetValue(DeminsionHeightProperty); }
            set { SetValue(DeminsionHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeminsionHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeminsionHeightProperty =
            DependencyProperty.Register("DeminsionHeight",
                                        typeof(double),
                                        typeof(FixSizeImage),
                                        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        private void OnImageLoaded(object sender, RoutedEventArgs e)
        {
            do
            {
                if (double.IsNaN(this.DeminsionWidth)
                  || double.IsNaN(this.DeminsionHeight)
                  || Source == null)
                {
                    break;
                }

                double imageWidth = Source.Width;
                double imageHeight = Source.Height;
                if (imageHeight == 0)
                {
                    break;
                }

                double ImageRatio = imageWidth / imageHeight;
                double ControlRatio = DeminsionWidth / DeminsionHeight;
                if (ImageRatio > ControlRatio)
                {
                    this.Width = DeminsionWidth;
                    this.Height = this.DeminsionWidth / ImageRatio;
                    SetZoomableCanvasOffset(0, (this.DeminsionHeight - this.Height) / 2);
                }
                else
                {
                    this.Width = this.DeminsionHeight * ImageRatio;
                    this.Height = DeminsionHeight;
                    SetZoomableCanvasOffset((this.DeminsionWidth - this.Width) / 2, 0);
                }
            } while (false);
           
        }
        

        private void SetZoomableCanvasOffset(double x, double y)
        {
            if (ContainerZoomableCanvas == null)
            {
                return;
            }

            if (ContainerZoomableCanvas.Offset != new Point(0,0))
            {
                return;
            }
            ContainerZoomableCanvas.Offset = new Point(-x, -y);
        }
        private ZoomableCanvas _containerZoomableCanvas;
        private ZoomableCanvas ContainerZoomableCanvas
        {
            get
            {
                if (_containerZoomableCanvas == null)
                {
                    _containerZoomableCanvas = TreeHelper.TryFindParent<ZoomableCanvas>(this);
                }
                return _containerZoomableCanvas;
            }
        }
    }
}
