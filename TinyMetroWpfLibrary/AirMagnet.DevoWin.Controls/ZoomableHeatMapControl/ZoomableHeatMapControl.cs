using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using AirMagnet.AircheckWifiTester.Utils;
using System.ComponentModel;
using AirMagnet.AircheckWifiTester.Utils.ImageBuilder;
namespace AirMagnet.AircheckWifiTester.Controls
{
    public class ZoomableHeatMapControl : ContentControl
    {
        static ZoomableHeatMapControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomableHeatMapControl), new FrameworkPropertyMetadata(typeof(ZoomableHeatMapControl)));
        }

        private const int ThrottleIntervalMilliseconds = 200;
        private readonly DispatcherTimer _levelChangeThrottle;
        public ZoomableHeatMapControl()
        {
            this.Loaded += OmLoaded;
            this.Unloaded += OnUnloaded;
            _levelChangeThrottle = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(ThrottleIntervalMilliseconds), IsEnabled = false };
            _levelChangeThrottle.Tick += (s, e) =>
            {
                PathShapFacade = new ShapBuildFacade(_floorPlanImage.ActualWidth, _floorPlanImage.ActualHeight, Math.Max(0, Math.Min(3, Scale)));
                RangeIndicatorShapFacade = new ShapBuildFacade(_floorPlanImage.ActualWidth, _floorPlanImage.ActualHeight, Math.Max(0, Math.Min(3, Scale)));
                DrawAllPoints(PointCollection);
                _levelChangeThrottle.IsEnabled = false;
            };

        }

        private void OmLoaded(object sender, RoutedEventArgs e)
        {

        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (IsInitModel)
            {
                this.Scale = 1;
                this.Offset = new Point(0, 0);
            }
        }

 
        #region const
        private const int MIN_SCALE = 1;
        private const int MAX_SCALE = 1000;
        private const int ScaleAnimationRelativeDuration = 400;
        private const double MinScaleRelativeToMinSize = 0.5;
        private const double MaxScaleRelativeToMaxSize = 5;
        #endregion

        #region event


        public Point PickerPoint
        {
            get { return (Point)GetValue(PickerPointProperty); }
            set { SetValue(PickerPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PickerPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PickerPointProperty =
            DependencyProperty.Register("PickerPoint", typeof(Point), typeof(ZoomableHeatMapControl), new PropertyMetadata(new Point(0,0)));

        
        #endregion




        public bool IsInitModel
        {
            get { return (bool)GetValue(IsInitModelProperty); }
            set { SetValue(IsInitModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInitModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInitModelProperty =
            DependencyProperty.Register("IsInitModel", typeof(bool), typeof(ZoomableHeatMapControl), new PropertyMetadata(true));




        public string SaveImageFilePath
        {
            get { return (string)GetValue(SaveImageFilePathProperty); }
            set { SetValue(SaveImageFilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveImageFilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveImageFilePathProperty =
            DependencyProperty.Register("SaveImageFilePath", typeof(string), typeof(ZoomableHeatMapControl), new PropertyMetadata(null));

        
        

        

        // for zoom in / zoom out
        public double ZoomScale
        {
            get { return (double)GetValue(ZoomScaleProperty); }
            set { SetValue(ZoomScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomScaleProperty =
            DependencyProperty.Register("ZoomScale", typeof(double), typeof(ZoomableHeatMapControl), new PropertyMetadata(1.0, ZoomScaleChanged));

        public static void ZoomScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomableHeatMapControl heatmap = d as ZoomableHeatMapControl;
            if (heatmap != null)
            {
                heatmap.OnZoomScaleChanged((double)e.NewValue - (double)e.OldValue);
            }
        }

        public void OnZoomScaleChanged(double scale)
        {
            if (scale > 0)
            {
                ScaleCanvas(1.2, new Point(this.ActualWidth / 2, this.ActualHeight / 2));
                
            }
            else
            {
                ScaleCanvas(0.8, new Point(this.ActualWidth / 2, this.ActualHeight / 2));
            } 
        }

        
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ZoomableHeatMapControl), new PropertyMetadata(1.0));


        public Point Offset
        {
            get { return (Point)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(Point), typeof(ZoomableHeatMapControl), new PropertyMetadata(new Point(0,0)));



        public Point? ToolTipLocation
        {
            get { return (Point?)GetValue(ToolTipLocationProperty); }
            set { SetValue(ToolTipLocationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTipLocation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipLocationProperty =
            DependencyProperty.Register("ToolTipLocation", typeof(Point?), typeof(ZoomableHeatMapControl), new PropertyMetadata(null, ToolTipLocationChanged));

        public static void ToolTipLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomableHeatMapControl heatmapControl = d as ZoomableHeatMapControl;
            if (heatmapControl != null)
            {
                heatmapControl.OnToolTipLocationChanged(e.NewValue as Point?);
            }
        }


        public Style StartPointControlStyle
        {
            get { return (Style)GetValue(StartPointControlStyleProperty); }
            set { SetValue(StartPointControlStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartPointControlStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartPointControlStyleProperty =
            DependencyProperty.Register("StartPointControlStyle", typeof(Style), typeof(ZoomableHeatMapControl), new PropertyMetadata(null));

        
        public void OnToolTipLocationChanged(Point? location)
        {
            if (_thumbCanvas == null)
            {
                return;
            }

            if (_thumb == null)
            {
                return;
            }
            if (location == null)
            {
                _thumbCanvas.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                _thumbCanvas.Visibility = System.Windows.Visibility.Visible;
                Canvas.SetLeft(_thumb, location.Value.X);
                Canvas.SetTop(_thumb, location.Value.Y);
            }
        }

        public Style ToolTipThumbStyle
        {
            get { return (Style)GetValue(ToolTipThumbStyleProperty); }
            set { SetValue(ToolTipThumbStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTipThumbStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipThumbStyleProperty =
            DependencyProperty.Register("ToolTipThumbStyle", typeof(Style), typeof(ZoomableHeatMapControl), new PropertyMetadata(null));



        public Point? StartPoint
        {
            get { return (Point?)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point?), typeof(ZoomableHeatMapControl), new PropertyMetadata(null));

        
        
        private IScrollInfo ScrollInfo
        {
            get
            {
                return _zoomableCanvas as IScrollInfo;
            }
        }


        public Point ZoomableCanvasOffset
        {
            get 
            {
                return _zoomableCanvas.Offset;
            }
            set 
            {
                _zoomableCanvas.Offset = GetValideOffset(value);
            }
        }
        
        private Point GetValideOffset(Point offset)
        {
            double MaxHorizontalOffset = 0, MinHorizontalOffset = 0, MaxVerticalOffset = 0, MinVertercalOffset = 0;
            if (_zoomableCanvas.Scale <= 1)
            {
                MaxHorizontalOffset = ActualImageWidth * this.Scale / 2;
                MinHorizontalOffset = -(CurrentScrollInfo.ViewportWidth - ActualImageWidth * this.Scale / 2);
                MaxVerticalOffset = ActualImageHeight * this.Scale / 2;
                MinVertercalOffset = -(CurrentScrollInfo.ViewportHeight - ActualImageHeight * this.Scale / 2);

            }
            else
            {
                MaxHorizontalOffset = ActualImageWidth * this.Scale - ActualImageWidth / 2;
                MinHorizontalOffset = ActualImageWidth / 2 - CurrentScrollInfo.ViewportWidth;
                MaxVerticalOffset = ActualImageHeight * this.Scale - ActualImageHeight / 2;
                MinVertercalOffset = ActualImageHeight / 2 - CurrentScrollInfo.ViewportHeight;
            }

            double x = Math.Max(MinHorizontalOffset, Math.Min(offset.X, MaxHorizontalOffset));
            double y = Math.Max(MinVertercalOffset, Math.Min(offset.Y, MaxVerticalOffset));
            return new Point(x,y);
        }

        private ZoomableCanvas _zoomableCanvas = null;

        private Image _floorPlanImage = null;
        public Image FloorPlanImage
        {
            get
            {
                return _floorPlanImage;
            }
        }

        private Thumb _thumb;
        private ZoomableCanvas _thumbCanvas = null;
        public ShapBuildFacade PathShapFacade
        {
            get { return (ShapBuildFacade)GetValue(PathShapFacadeProperty); }
            set { SetValue(PathShapFacadeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for PathShapFacade.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathShapFacadeProperty =
            DependencyProperty.Register("PathShapFacade", typeof(ShapBuildFacade), typeof(ZoomableHeatMapControl), new PropertyMetadata(null));



        public ShapBuildFacade RangeIndicatorShapFacade
        {
            get { return (ShapBuildFacade)GetValue(RangeIndicatorShapFacadeProperty); }
            set { SetValue(RangeIndicatorShapFacadeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeIndicatorShapFacade.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeIndicatorShapFacadeProperty =
            DependencyProperty.Register("RangeIndicatorShapFacade", typeof(ShapBuildFacade), typeof(ZoomableHeatMapControl), new PropertyMetadata(null));

        
        
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            IsManipulationEnabled = true;
            _zoomableCanvas = this.GetTemplateChild("PATH_ZoomableCanvas") as ZoomableCanvas;
            MouseTouchDevice.RegisterEvents(_zoomableCanvas);
            _floorPlanImage = this.GetTemplateChild("FloorPlanImage") as Image;
            if (_floorPlanImage != null)
            {
                _floorPlanImage.SizeChanged += Image_SizeChanged;
            }
            _thumb = this.GetTemplateChild("HoverThumb") as Thumb;
            if (_thumb != null)
            {
                _thumb.DragDelta += OnThumbDragDelta;
            }

            _thumbCanvas = this.GetTemplateChild("ToolTipLocationPicker") as ZoomableCanvas;
            if (ToolTipLocation == null && _thumbCanvas != null)
            {
                _thumbCanvas.Visibility = System.Windows.Visibility.Collapsed;
            }

            DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(ZoomableHeatMapControl.ScaleProperty, typeof(ZoomableHeatMapControl));
            descriptor.AddValueChanged(this, new EventHandler(OnScaleChanged));

            descriptor = DependencyPropertyDescriptor.FromProperty(ZoomableHeatMapControl.SaveImageFilePathProperty, typeof(ZoomableHeatMapControl));
            descriptor.AddValueChanged(this, new EventHandler(OnImageFilePathChanged));
        }

        private void OnScaleChanged(object sender, EventArgs e)
        {
            ThrottleChangeLevel();
        }

        private void OnImageFilePathChanged(object sender, EventArgs e)
        {
            if (SaveImageFilePath == null)
            {
                return;
            }
            PathShapFacade.SaveImageFile(SaveImageFilePath);
            
        }

        private void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ThrottleChangeLevel();
        }

        private IScrollInfo CurrentScrollInfo
        {
            get
            {
                return _zoomableCanvas;
            }
        }
        private void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            if (thumb == null)
            {
                return;
            }
            double left = Canvas.GetLeft(thumb);
            double top = Canvas.GetTop(thumb);
            left = left + e.HorizontalChange / this.Scale;
            top = top + e.VerticalChange / this.Scale;
            left = Math.Max(0, Math.Min(left, ActualImageWidth));
            top = Math.Max(0, Math.Min(top, this.ActualImageHeight));
            Canvas.SetLeft(thumb, left);
            Canvas.SetTop(thumb, top);
            ToolTipLocation = new Point(left, top);
        }


        #region Overriden Input Event Handlers
        private bool _hasManipulationDelta = false;
        private const double Max_Drag_Delta_Picker = 10;
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);

            var oldScale = _zoomableCanvas.Scale;
            _zoomableCanvas.ApplyAnimationClock(ZoomableCanvas.ScaleProperty, null);
            _zoomableCanvas.Scale = oldScale;

            var oldOffset = ZoomableCanvasOffset;
            _zoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            ZoomableCanvasOffset = oldOffset;

            var scale = e.DeltaManipulation.Scale.X;
            ScaleCanvas(scale, e.ManipulationOrigin);

            ZoomableCanvasOffset -= e.DeltaManipulation.Translation;
            e.Handled = true;
            if (e.DeltaManipulation.Translation.X * e.DeltaManipulation.Translation.X 
                + e.DeltaManipulation.Translation.Y * e.DeltaManipulation.Translation.Y 
                > Max_Drag_Delta_Picker * Max_Drag_Delta_Picker)
            {
                _hasManipulationDelta = true;
            }
            
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
            base.OnManipulationInertiaStarting(e);
            e.TranslationBehavior = new InertiaTranslationBehavior { DesiredDeceleration = 0.0096 };
            e.ExpansionBehavior = new InertiaExpansionBehavior { DesiredDeceleration = 0.000096 };
            if (!_hasManipulationDelta)
            {
                PickerPoint = new Point((e.ManipulationOrigin.X + ZoomableCanvasOffset.X) / Scale, (e.ManipulationOrigin.Y + ZoomableCanvasOffset.Y) / Scale);
            }
            _hasManipulationDelta = false;
            e.Handled = true;
        }

        static int count = 0;
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            count++;
            var relativeScale = Math.Pow(2, (double)e.Delta /5/ Mouse.MouseWheelDeltaForOneLine);
            var position = e.GetPosition(this);
            ScaleCanvas(relativeScale, position);
             
            e.Handled = true;
        }

        #endregion

        private double _originalScale = 0.95;
        private void ScaleCanvas(double relativeScale, Point center, bool animate = false)
        {
            var scale = _zoomableCanvas.Scale;

            if (scale <= 0)
            {
                return;
            }
            relativeScale = relativeScale.Clamp(
                MinScaleRelativeToMinSize * _originalScale / scale,
                Math.Max(MaxScaleRelativeToMaxSize, MaxScaleRelativeToMaxSize * _originalScale) / scale);

            var targetScale = scale * relativeScale;

            if (targetScale != scale)
            {
               
                var position = (Vector)center;
                var targetOffset = (Point)((Vector)(ZoomableCanvasOffset + position) * relativeScale - position);
                targetOffset = GetValideOffset(targetOffset);
                if (animate)
                {
                    if (relativeScale < 1)
                        relativeScale = 1 / relativeScale;
                    var duration = TimeSpan.FromMilliseconds(relativeScale * ScaleAnimationRelativeDuration);
                    var easing = new CubicEase();
                    _zoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(targetScale, duration) { EasingFunction = easing }, HandoffBehavior.Compose);
                    _zoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(targetOffset, duration) { EasingFunction = easing }, HandoffBehavior.Compose);
                }
                else
                {
                    _zoomableCanvas.Scale = targetScale;
                    ZoomableCanvasOffset = targetOffset;
                }
            }
        }

        #region PointCollection

        public ObservableCollection<HeatMapVisualPoint> PointCollection
        {
            get { return (ObservableCollection<HeatMapVisualPoint>)GetValue(PointCollectionProperty); }
            set { SetValue(PointCollectionProperty, value); }
        }
        // Using a DependencyProperty as the backing store for PointCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointCollectionProperty =
            DependencyProperty.Register("PointCollection", typeof(ObservableCollection<HeatMapVisualPoint>), typeof(ZoomableHeatMapControl), new PropertyMetadata(null, PointCollectionChanged));

        private static void PointCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomableHeatMapControl control = d as ZoomableHeatMapControl;
            if (control == null)
            {
                return;
            }
            control.OnPointCollectionChanged(e.OldValue as ObservableCollection<HeatMapVisualPoint>, e.NewValue as ObservableCollection<HeatMapVisualPoint>);
        }

        private ShapeBuilderType TransLatePointType(HeatMapVisualPoint point)
        {
            if (point.IsDashArrowPoint)
            {
                return ShapeBuilderType.ArrowLine;
            }
            else
            {
                return ShapeBuilderType.Rectangle;
            }
        }

        public void OnPointCollectionChanged(ObservableCollection<HeatMapVisualPoint> OldpointCollection, ObservableCollection<HeatMapVisualPoint> pointCollection)
        {
            if (OldpointCollection  != null)
            {
                OldpointCollection.CollectionChanged -= PointCollection_CollectionChanged;
            }
            if (pointCollection != null)
            {
                DrawAllPoints(pointCollection);
                pointCollection.CollectionChanged += PointCollection_CollectionChanged;
            }
            else
            {
                _prePenDownPoint = null;
                if (PathShapFacade != null)
                {
                    PathShapFacade.Dispose();
                    PathShapFacade = null;
                }

                if (RangeIndicatorShapFacade != null)
                {
                    RangeIndicatorShapFacade.Dispose();
                    RangeIndicatorShapFacade = null;
                }
            }
        }

        private void DrawAllPoints(ObservableCollection<HeatMapVisualPoint> pointCollection)
        {
            _prePenDownPoint = null;
            if (PathShapFacade == null)
            {
                return;
            }

            if (RangeIndicatorShapFacade == null && RangeRadius == null)
            {
                return;
            }

            if (pointCollection == null || pointCollection.Count == 0)
            {
                PathShapFacade.Dispose();
                RangeIndicatorShapFacade.Dispose();
            }
            else
            {
                List<ShapeData> pathShapeList = new List<ShapeData>();
                List<ShapeData> rangeIndicatorShapList = new List<ShapeData>();
                foreach (var point in pointCollection)
                {
                    ShapeData pathShapData = TranslateVisualPointToPathShapData(point);
                    if (pathShapData != null)
                    {
                        pathShapeList.Add(pathShapData);
                    }

                    ShapeData RangeIndicatorShapData = TranslateVisualPointToRangeIndicatorShapData(point);
                    if (RangeIndicatorShapData != null)
                    {
                        rangeIndicatorShapList.Add(RangeIndicatorShapData);
                    }
                }
                PathShapFacade.ReDrawShaps(pathShapeList);
                RangeIndicatorShapFacade.ReDrawShaps(rangeIndicatorShapList);
            }
        }

        private Point? _prePenDownPoint = null;
        //private Point? PrePenDownPoint
        //{
        //    get
        //    {
        //        return _prePenDownPoint;
        //    }
        //    set
        //    {
        //        _prePenDownPoint = value;
        //    }
        //}
        private ShapeData TranslateVisualPointToPathShapData(HeatMapVisualPoint point)
        {
            ShapeData shape = new ShapeData();
            if (point.IsDashArrowPoint && _prePenDownPoint == null)
            {
                shape.ShapType = ShapeBuilderType.StartEllipse;
                shape.Position = point.VisualPoint;
                _prePenDownPoint = point.VisualPoint;
            }
            else if (point.IsDashArrowPoint && _prePenDownPoint != null)
            {
                shape.ShapType = ShapeBuilderType.ArrowLine;
                shape.Start = _prePenDownPoint.Value;
                shape.End = point.VisualPoint;
                _prePenDownPoint = point.VisualPoint;
            }
            else
            {
                shape.ShapType = ShapeBuilderType.Rectangle;
                shape.Position = point.VisualPoint;
            }
            return shape;
        }

        private ShapeData TranslateVisualPointToRangeIndicatorShapData(HeatMapVisualPoint point)
        {
            if (RangeRadius == null)
            {
                return null;
            }

            if (!point.IsNeedRangeIndicator)
            {
                return null;
            }

            ShapeData RangeShap = new ShapeData();
            RangeShap.Position = point.VisualPoint;
            //for DE5970
            RangeShap.Radius = RangeRadius.Value/4;
            RangeShap.ShapType = ShapeBuilderType.RangeEllipse;
            RangeShap.Brush = point.Brush;
            return RangeShap;
        }

          
        private void PointCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        HeatMapVisualPoint point = (HeatMapVisualPoint)item;
                        ShapeData PathShap = TranslateVisualPointToPathShapData(point);
                        if (PathShap != null)
                        {
                            PathShapFacade.AddShap(PathShap);
                        }
                        ShapeData rangeIndicatorShap = TranslateVisualPointToRangeIndicatorShapData(point);
                        if (rangeIndicatorShap != null)
                        {
                            RangeIndicatorShapFacade.AddShap(rangeIndicatorShap);
                        }
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        DrawAllPoints(PointCollection);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                default:
                    break;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                
            }
        }

        private Point Converter2ImagePoint(Point point)
        {
            double canvasLeft = Canvas.GetLeft(_floorPlanImage);
            double canvasTop = Canvas.GetTop(_floorPlanImage);
            return new Point(point.X - canvasLeft, point.Y - canvasTop);
        }

        private Point Converter2OriginalPoint(Point point)
        {
            double canvasLeft = Canvas.GetLeft(_floorPlanImage);
            double canvasTop = Canvas.GetTop(_floorPlanImage);
            return new Point(point.X + canvasLeft, point.Y + canvasTop);
        }
                 
     
        private void ThrottleChangeLevel()
        {

            if (_levelChangeThrottle.IsEnabled)
                _levelChangeThrottle.Stop();

            _levelChangeThrottle.Start();
        }
        #endregion

        #region Image
        public ImageSource BackgroundImageSource
        {
            get { return (ImageSource)GetValue(BackgroundImageSourceProperty); }
            set { SetValue(BackgroundImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundImageSourceProperty =
            DependencyProperty.Register("BackgroundImageSource", typeof(ImageSource), typeof(ZoomableHeatMapControl), new PropertyMetadata(null));


        public ImageSource HeatMapImageSource
        {
            get { return (ImageSource)GetValue(HeatMapImageSourceProperty); }
            set { SetValue(HeatMapImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeatMapImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeatMapImageSourceProperty =
            DependencyProperty.Register("HeatMapImageSource", typeof(ImageSource), typeof(ZoomableHeatMapControl), new PropertyMetadata(null));


        public double ActualImageWidth
        {
            get { return (double)GetValue(ActualImageWidthProperty); }
            set { SetValue(ActualImageWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualImageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualImageWidthProperty =
            DependencyProperty.Register("ActualImageWidth", typeof(double), typeof(ZoomableHeatMapControl), new PropertyMetadata(0.0));


        public double ActualImageHeight
        {
            get { return (double)GetValue(ActualImageHeightProperty); }
            set { SetValue(ActualImageHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualImageHeightProperty =
            DependencyProperty.Register("ActualImageHeight", typeof(double), typeof(ZoomableHeatMapControl), new PropertyMetadata(0.0));




        public double? RangeRadius
        {
            get { return (double?)GetValue(RangeRadiusProperty); }
            set { SetValue(RangeRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeRadiusProperty =
            DependencyProperty.Register("RangeRadius", typeof(double?), typeof(ZoomableHeatMapControl), new PropertyMetadata(null));

        
        
        
        #endregion
    }
    
    
}
