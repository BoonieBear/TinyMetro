using TinyMetroWpfLibrary.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TinyMetroWpfLibrary.Controls
{
    public class ColorLegend : Slider
    {
        static ColorLegend()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorLegend), new FrameworkPropertyMetadata(typeof(ColorLegend)));
        }

        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set
            {
                SetValue(UpperValueProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for UpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double), typeof(ColorLegend), new PropertyMetadata(0.0, UpperValueChanged));

        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LowerValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double), typeof(ColorLegend), new PropertyMetadata(0.0, LowerValueChanged));



        public double ColorItemHeight
        {
            get { return (double)GetValue(ColorItemHeightProperty); }
            set { SetValue(ColorItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorItemHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorItemHeightProperty =
            DependencyProperty.Register("ColorItemHeight", typeof(double), typeof(ColorLegend), new PropertyMetadata(0.0));

        public double TickItemHeight
        {
            get { return (double)GetValue(TickItemHeightProperty); }
            set { SetValue(TickItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickItemHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickItemHeightProperty =
            DependencyProperty.Register("TickItemHeight", typeof(double), typeof(ColorLegend), new PropertyMetadata(0.0));


        public bool IsSignalStyle
        {
            get { return (bool)GetValue(IsSignalStyleProperty); }
            set { SetValue(IsSignalStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSignalStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSignalStyleProperty =
            DependencyProperty.Register("IsSignalStyle",
                                        typeof(bool),
                                        typeof(ColorLegend),
                                         new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));



        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
            PrepareTicksItemsSource();
        }

        private Thumb _upperThumb = new Thumb();
        private Thumb _lowerThumb = new Thumb();
        private Canvas _trackCanvas = new Canvas();
        private Border _track = new Border();
        ItemsControl _itemsControl = new ItemsControl();
        ItemsControl _TicksitemsControl = new ItemsControl();
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _upperThumb = GetTemplateChild("UpperThumb") as Thumb;
            if (_upperThumb == null)
            {
                throw (new Exception("GetTemplateChild of UpperThumb failed"));
            }

            _lowerThumb = GetTemplateChild("LowerThumb") as Thumb;
            if (_lowerThumb == null)
            {
                throw (new Exception("GetTemplateChild of LowerThumb failed"));
            }

            _trackCanvas = GetTemplateChild("TrackCanvas") as Canvas;
            if (_trackCanvas == null)
            {
                throw (new Exception("GetTemplateChild of TrackCanvas failed"));
            }

            _track = GetTemplateChild("Track") as Border;
            if (_track == null)
            {
                throw (new Exception("GetTemplateChild of TrackCanvas failed"));
            }

            _itemsControl = GetTemplateChild("ColorItemsControl") as ItemsControl;
            if (_itemsControl == null)
            {
                throw (new Exception("GetTemplateChild of ColorItemsControl failed"));
            }

            _TicksitemsControl = GetTemplateChild("TicksItemsControl") as ItemsControl;
            if (_itemsControl == null)
            {
                throw (new Exception("GetTemplateChild of TicksItemsControl failed"));
            }

            _upperThumb.DragDelta += OnUpperThumb_DragDelta;
            _lowerThumb.DragDelta += OnLowerThumb_DragDelta;
            _track.SizeChanged += OnSizeChanged;
            PrepareColorItemSource();
            PrepareTicksItemsSource();
        }

        private void PrepareColorItemSource()
        {
            if (_itemsControl != null)
            {
                List<object> list = new List<object>();
                for (int i = 0; i < Maximum; i++)
                {
                    list.Add(new { Index = i });
                }
                _itemsControl.DataContext = list;
            }
        }

        private void PrepareTicksItemsSource()
        {
            List<object> ticks = new List<object>();
            if (IsSignalStyle)
            {
                for (int i = 0; i <= 10; i++)
                {
                    ticks.Add(new { Tick = -(i * 10) });
                }
            }
            else
            {
                ticks.Add(new { Tick = Constants.MAX_COLOR_LEGEND_THROUTHPUT_VALUE });
                ticks.Add(new { Tick = 200 });
                ticks.Add(new { Tick = 90 }); 
                ticks.Add(new { Tick = 70 });
                ticks.Add(new { Tick = 50 });
                ticks.Add(new { Tick = 30 });
                ticks.Add(new { Tick = 15 });
                ticks.Add(new { Tick = 5 });
                ticks.Add(new { Tick = 3 });
                ticks.Add(new { Tick = 1 });
                ticks.Add(new { Tick = Constants.MIN_COLOR_LEGEND_THROUGHPUT_VALUE });
            }
            if (_TicksitemsControl != null)
            {
                _TicksitemsControl.DataContext = ticks;
            }
        }


        private static void UpperValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorLegend slider = d as ColorLegend;
            slider.OnUpperValueChanged((double)e.NewValue);
        }

        private static void LowerValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorLegend slider = d as ColorLegend;
            slider.OnLowerValueChanged((double)e.NewValue);
        }


        public void OnUpperValueChanged(double upperValue)
        {
            OnValueChanged(_upperThumb, upperValue);
        }

        public void OnLowerValueChanged(double lowerValue)
        {
            OnValueChanged(_lowerThumb, lowerValue);
        }

        private void OnValueChanged(Thumb thumb, double value)
        {
            if (DoubleUtil.IsZero(_density)
                || thumb == null)
            {
                return;
            }

            Canvas.SetTop(thumb, value / _density);
        }

        private void OnLowerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double lowerValue = this.LowerValue + ValueFromDistance(e.VerticalChange);
            UpdateLowerValue(lowerValue);
        }

        private void OnUpperThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double upperValue = this.UpperValue + ValueFromDistance(e.VerticalChange);
            UpdateUppperValue(upperValue);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSelectionRangeElementPositionAndSize();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            UpdateSelectionRangeElementPositionAndSize();
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            UpdateSelectionRangeElementPositionAndSize();
        }

        private void CalculateDensity()
        {
            if (!DoubleUtil.AreEqual(_track.ActualHeight, _upperThumb.ActualHeight))
            {
                _density = (Maximum - Minimum) / (_track.ActualHeight - _upperThumb.ActualHeight);
            }
        }


        private double _density = 0;
        private double ValueFromDistance(double distance)
        {
            return distance * _density;
        }

        private double SnapToTick(double value)
        {
            double num;
            double minimum = base.Minimum;
            double maximum = base.Maximum;

            if (DoubleUtil.GreaterThan(this.TickFrequency, 0))
            {
                minimum = base.Minimum + Math.Round((value - base.Minimum) / this.TickFrequency) * this.TickFrequency;
                maximum = Math.Min(base.Maximum, minimum + this.TickFrequency);
            }
            num = (DoubleUtil.GreaterThanOrClose(value, (minimum + maximum) * 0.5) ? maximum : minimum);
            return num;
        }

        private void UpdateUppperValue(double value)
        {
            double tick = this.SnapToTick(value);
            UpperValue = Math.Max(LowerValue+5, Math.Min(base.Maximum, tick));
        }

        private void UpdateLowerValue(double value)
        {
            double tick = this.SnapToTick(value);
            LowerValue = Math.Max(base.Minimum, Math.Min(UpperValue-5, tick));
        }

        private void UpdateSelectionRangeElementPositionAndSize()
        {
            double num;
            Size size;
            Size renderSize = new Size(0, 0);
            if (_track != null)
            {
                renderSize = _track.RenderSize;
                size = _upperThumb.RenderSize;
                double maximum = base.Maximum - base.Minimum;
                FrameworkElement selectionRangeElement = _itemsControl;
                if (selectionRangeElement != null)
                {
                    num = Math.Max(0, (renderSize.Height - size.Height) / maximum);
                    selectionRangeElement.Height = Math.Max(0, renderSize.Height - size.Height);
                    ColorItemHeight = selectionRangeElement.Height / maximum;
                    TickItemHeight = selectionRangeElement.Height / 10;                    
                }
            }
            CalculateDensity();
            OnValueChanged(_upperThumb, UpperValue);
            OnValueChanged(_lowerThumb, LowerValue);
        }
    }
}
