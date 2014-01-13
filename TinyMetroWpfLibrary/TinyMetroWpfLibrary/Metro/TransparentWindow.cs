// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.Events;

namespace BoonieBear.TinyMetro.WPF.Metro
{
    /// <summary>
    /// The TransparentWindow is based on a sample of William Han that he published on MSDN.
    /// For more information see <see cref="http://archive.msdn.microsoft.com/getwpfcodebywillhan/Release/ProjectReleases.aspx?ReleaseId=1578"/>
    /// </summary>
    public partial class TransparentWindow : Window
    {
        public const double BORDER_SIZE = 1;

        public static readonly DependencyProperty ResizeDirectionsProperty = 
            DependencyProperty.Register("ResizeDirections", typeof (ResizeDirectionFlags?), typeof (TransparentWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty DragableProperty =
            DependencyProperty.Register("Dragable", typeof (bool), typeof (TransparentWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ResizeableProperty =
            DependencyProperty.Register("Resizeable", typeof (bool), typeof (TransparentWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsMainWindowProperty =
            DependencyProperty.Register("IsMainWindow", typeof(bool), typeof(TransparentWindow), new PropertyMetadata(true));

        public static RoutedUICommand MinimizeCommand = new RoutedUICommand();
        public static RoutedUICommand MaximizeCommand = new RoutedUICommand();
        public static RoutedUICommand NormalizeCommand = new RoutedUICommand();

        /// <summary>
        /// Initializes the <see cref="TransparentWindow"/> class.
        /// </summary>
        static TransparentWindow()
        {
// ReSharper disable ObjectCreationAsStatement
            new FrameworkElement();
// ReSharper restore ObjectCreationAsStatement

            // Establish a new Style Property that uses the default Style "TransparentWindow", if no style has been set
            StyleProperty.OverrideMetadata(typeof(TransparentWindow), new FrameworkPropertyMetadata(null, OnCoerceStyle));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransparentWindow), new FrameworkPropertyMetadata(typeof(TransparentWindow)));

            EventManager.RegisterClassHandler(typeof(TransparentWindow), Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta));
            EventManager.RegisterClassHandler(typeof(TransparentWindow), PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
        }

        /// <summary>
        /// Initializes a new instance of the TransparentWindow class.
        /// </summary>
        public TransparentWindow()
        {
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;
            Background = new SolidColorBrush {Color = Colors.White};
            Loaded += InitializeTransparentWindow;
            Closing += FadeOutOnClosing;
            StateChanged += OnWindowStateChanged;
        }

        /// <summary>
        /// Reacts on the window state changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    if (storedResizeDirections != null)
                    {
                        ResizeDirections = storedResizeDirections.Value;
                        storedResizeDirections = null;
                    }
                    break;
                case WindowState.Minimized:
                    break;
                case WindowState.Maximized:
                    storedResizeDirections = ResizeDirections;
                    ResizeDirections = ResizeDirectionFlags.None;
                    break;
            }
        }

        /// <summary>
        /// Initialize the Transparent Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitializeTransparentWindow(object sender, EventArgs e)
        {
            // First Time initialization
            if (GetValue(ResizeDirectionsProperty) == null)
                ResizeDirections =
                    ResizeDirectionFlags.SizeN |
                    ResizeDirectionFlags.SizeS |
                    ResizeDirectionFlags.SizeW |
                    ResizeDirectionFlags.SizeE |
                    ResizeDirectionFlags.SizeSE |
                    ResizeDirectionFlags.SizeNW |
                    ResizeDirectionFlags.SizeSW |
                    ResizeDirectionFlags.SizeNE;
        }

        /// <summary>
        /// Evaluates the default style "TransparentWindow", if no style has been set
        /// </summary>
        /// <param name="o"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object OnCoerceStyle(DependencyObject o, object value)
        {
            return value ?? ((TransparentWindow)o).TryFindResource("TransparentWindow");
        }

        /// <summary>
        /// Gets or sets a value indicating whether the windows is dragable or not
        /// </summary>
        public bool Dragable
        {
            get { return (bool) GetValue(DragableProperty); }
            set { SetValue(DragableProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the windows is Resizeable or not
        /// </summary>
        public bool Resizeable
        {
            get { return (bool) GetValue(ResizeableProperty); }
            set
            {
                if (value == Resizeable) 
                    return;

                SetValue(ResizeableProperty, value);
                UpdateBorders(value ? ResizeDirections : ResizeDirectionFlags.None);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window is the main window 
        /// </summary>
        public bool IsMainWindow
        {
            get { return (bool) GetValue(IsMainWindowProperty); }
            set { SetValue(IsMainWindowProperty, value); }
        }

        /// <summary>
        /// stored resize directions, that are the prior maximize Window Settings.
        /// </summary>
        private ResizeDirectionFlags? storedResizeDirections;

        /// <summary>
        /// Set the Resize Directions
        /// </summary>
        public ResizeDirectionFlags ResizeDirections
        {
            get { return (ResizeDirectionFlags) GetValue(ResizeDirectionsProperty); }
            set
            {
                var oldValue = (ResizeDirectionFlags?)GetValue(ResizeDirectionsProperty);

                // Compare it to the previous value
                if (oldValue != value)
                {
                    // Set the property
                    SetValue(ResizeDirectionsProperty, value);

                    // ReSharper disable PossibleNullReferenceException
                    UpdateBorders(value);

                    // Adjust the outer border
                    var border = (Border) GetTemplateChild("m_edgeBorder");
                    var thickness = new Thickness(
                        (value & ResizeDirectionFlags.SizeW) == ResizeDirectionFlags.SizeW ? BORDER_SIZE : 0.0,
                        (value & ResizeDirectionFlags.SizeN) == ResizeDirectionFlags.SizeN ? BORDER_SIZE : 0.0,
                        (value & ResizeDirectionFlags.SizeE) == ResizeDirectionFlags.SizeE ? BORDER_SIZE : 0.0,
                        (value & ResizeDirectionFlags.SizeS) == ResizeDirectionFlags.SizeS ? BORDER_SIZE : 0.0);
                    border.Margin = thickness;

                    // Now invert the inner border to 
                    border = (Border)GetTemplateChild("INNER_BORDER");
                    border.Margin = thickness;

                    // ReSharper restore PossibleNullReferenceException
                }
            }
        }

        private void UpdateBorders(ResizeDirectionFlags value)
        {
            // ReSharper disable PossibleNullReferenceException
            ((Thumb)GetTemplateChild("PART_SizeN")).IsEnabled = (value & ResizeDirectionFlags.SizeN) == ResizeDirectionFlags.SizeN;
            ((Thumb)GetTemplateChild("PART_SizeS")).IsEnabled = (value & ResizeDirectionFlags.SizeS) == ResizeDirectionFlags.SizeS; 
            ((Thumb)GetTemplateChild("PART_SizeW")).IsEnabled = (value & ResizeDirectionFlags.SizeW) == ResizeDirectionFlags.SizeW;
            ((Thumb)GetTemplateChild("PART_SizeE")).IsEnabled = (value & ResizeDirectionFlags.SizeE) == ResizeDirectionFlags.SizeE;
            ((Thumb)GetTemplateChild("PART_SizeSE")).IsEnabled = (value & ResizeDirectionFlags.SizeSE) == ResizeDirectionFlags.SizeSE;
            ((Thumb)GetTemplateChild("PART_SizeNW")).IsEnabled = (value & ResizeDirectionFlags.SizeNW) == ResizeDirectionFlags.SizeNW;
            ((Thumb)GetTemplateChild("PART_SizeSW")).IsEnabled = (value & ResizeDirectionFlags.SizeSW) == ResizeDirectionFlags.SizeSW;
            ((Thumb)GetTemplateChild("PART_SizeNE")).IsEnabled = (value & ResizeDirectionFlags.SizeNE) == ResizeDirectionFlags.SizeNE;
            // ReSharper restore PossibleNullReferenceException
        }        
        
        //private void UpdateBordersVisibility(ResizeDirectionFlags value)
        //{
        //    // ReSharper disable PossibleNullReferenceException
        //    ((Thumb)GetTemplateChild("PART_SizeN")).IsEnabled = (value & ResizeDirectionFlags.SizeN) == ResizeDirectionFlags.SizeN;
        //    ((Thumb)GetTemplateChild("PART_SizeS")).IsEnabled = (value & ResizeDirectionFlags.SizeS) == ResizeDirectionFlags.SizeS; 
        //    ((Thumb)GetTemplateChild("PART_SizeW")).IsEnabled = (value & ResizeDirectionFlags.SizeW) == ResizeDirectionFlags.SizeW;
        //    ((Thumb)GetTemplateChild("PART_SizeE")).IsEnabled = (value & ResizeDirectionFlags.SizeE) == ResizeDirectionFlags.SizeE;
        //    ((Thumb)GetTemplateChild("PART_SizeSE")).IsEnabled = (value & ResizeDirectionFlags.SizeSE) == ResizeDirectionFlags.SizeSE;
        //    ((Thumb)GetTemplateChild("PART_SizeNW")).IsEnabled = (value & ResizeDirectionFlags.SizeNW) == ResizeDirectionFlags.SizeNW;
        //    ((Thumb)GetTemplateChild("PART_SizeSW")).IsEnabled = (value & ResizeDirectionFlags.SizeSW) == ResizeDirectionFlags.SizeSW;
        //    ((Thumb)GetTemplateChild("PART_SizeNE")).IsEnabled = (value & ResizeDirectionFlags.SizeNE) == ResizeDirectionFlags.SizeNE;
        //    // ReSharper restore PossibleNullReferenceException
        //}

        private static void OnPreviewMouseDown(object sender,  MouseButtonEventArgs eventArgs)
        {
            var transparentWindow = (TransparentWindow)sender;
            if (transparentWindow.WindowState != WindowState.Normal || !transparentWindow.Dragable)
                return;

            var thumb = eventArgs.OriginalSource as Rectangle;
            if (thumb == null)
                return;

            if (thumb.Name.Equals("PART_HEADER") && eventArgs.LeftButton == MouseButtonState.Pressed)
                transparentWindow.DragMove();
        }

        /// <summary>
        /// Called when [drag delta].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private static void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var transparentWindow = (TransparentWindow)sender;
            var thumb = e.OriginalSource as Thumb;
            if (thumb != null && transparentWindow.WindowState == WindowState.Normal)
            {
                var windowRect = new Rect(transparentWindow.Left, transparentWindow.Top, transparentWindow.ActualWidth, transparentWindow.ActualHeight);
                double maxReducedHeight = Math.Max(0, transparentWindow.ActualHeight - transparentWindow.MinHeight);
                double maxReducedWidth = Math.Max(0, transparentWindow.ActualWidth - transparentWindow.MinWidth);
                double reducedHeight = e.VerticalChange;
                double reducedWidth = e.HorizontalChange;
                if (thumb.Name.Equals("PART_SizeSE") && (transparentWindow.ResizeDirections & ResizeDirectionFlags.SizeSE) == ResizeDirectionFlags.SizeSE)
                {
                    reducedHeight = Math.Max(reducedHeight, -maxReducedHeight);
                    reducedWidth = Math.Max(reducedWidth, -maxReducedWidth);
                    transparentWindow.Width = Math.Max(transparentWindow.ActualWidth + reducedWidth, transparentWindow.MinWidth);
                    transparentWindow.Height = Math.Max(transparentWindow.ActualHeight + reducedHeight, transparentWindow.MinHeight);
                }
                else if (thumb.Name.Equals("PART_SizeNW") && (transparentWindow.ResizeDirections & ResizeDirectionFlags.SizeNW) == ResizeDirectionFlags.SizeNW)
                {
                    reducedWidth = Math.Min(reducedWidth, maxReducedWidth);
                    reducedHeight = Math.Min(reducedHeight, maxReducedHeight);
                    windowRect.Y += reducedHeight;
                    windowRect.X += reducedWidth;
                    windowRect.Width -= reducedWidth;
                    windowRect.Height -= reducedHeight;
                    transparentWindow.SetWindowVisualRect(windowRect);
                }
                else if (thumb.Name.Equals("PART_SizeSW") && (transparentWindow.ResizeDirections & ResizeDirectionFlags.SizeSW) == ResizeDirectionFlags.SizeSW)
                {
                    reducedHeight = Math.Max(reducedHeight, -maxReducedHeight);
                    reducedWidth = Math.Min(reducedWidth, maxReducedWidth);
                    windowRect.X += reducedWidth;
                    windowRect.Width -= reducedWidth;
                    windowRect.Height += reducedHeight;
                    transparentWindow.SetWindowVisualRect(windowRect);
                }
                else if (thumb.Name.Equals("PART_SizeNE") && (transparentWindow.ResizeDirections & ResizeDirectionFlags.SizeNE) == ResizeDirectionFlags.SizeNE)
                {
                    reducedHeight = Math.Min(reducedHeight, maxReducedHeight);
                    reducedWidth = Math.Max(reducedWidth, -maxReducedWidth);
                    windowRect.Y += reducedHeight;
                    windowRect.Height = transparentWindow.ActualHeight - reducedHeight;
                    windowRect.Width = transparentWindow.ActualWidth + reducedWidth;
                    transparentWindow.SetWindowVisualRect(windowRect);
                }
                else if (thumb.Name.Equals("PART_SizeN") && (transparentWindow.ResizeDirections & ResizeDirectionFlags.SizeN) == ResizeDirectionFlags.SizeN)
                {
                    reducedHeight = Math.Min(reducedHeight, maxReducedHeight);
                    windowRect.Y += reducedHeight;
                    windowRect.Height = transparentWindow.ActualHeight - reducedHeight;
                    transparentWindow.SetWindowVisualRect(windowRect);
                }
                else if (thumb.Name.Equals("PART_SizeS") && (transparentWindow.ResizeDirections & ResizeDirectionFlags.SizeS) == ResizeDirectionFlags.SizeS)
                {
                    reducedHeight = Math.Max(reducedHeight, -maxReducedHeight);
                    transparentWindow.Height = transparentWindow.ActualHeight + reducedHeight;
                }
                else if (thumb.Name.Equals("PART_SizeW") && (transparentWindow.ResizeDirections & ResizeDirectionFlags.SizeW) == ResizeDirectionFlags.SizeW)
                {
                    reducedWidth = Math.Min(reducedWidth, maxReducedWidth);
                    windowRect.X += reducedWidth;
                    windowRect.Width = transparentWindow.ActualWidth - reducedWidth;
                    transparentWindow.SetWindowVisualRect(windowRect);
                }
                else if (thumb.Name.Equals("PART_SizeE") && (transparentWindow.ResizeDirections & ResizeDirectionFlags.SizeE) == ResizeDirectionFlags.SizeE)
                {
                    reducedWidth = Math.Max(reducedWidth, -maxReducedWidth);
                    transparentWindow.Width = transparentWindow.ActualWidth + reducedWidth;
                }
            }
        }

        /// <summary>
        /// Sets the window visual rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        protected virtual void SetWindowVisualRect(Rect rect)
        {
            var mainWindowPtr = new WindowInteropHelper(this).Handle;
            var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            if (mainWindowSrc == null || mainWindowSrc.CompositionTarget == null) 
                return;
            var deviceTopLeft = mainWindowSrc.CompositionTarget.TransformToDevice.Transform(rect.TopLeft);
            var deviceBottomRight = mainWindowSrc.CompositionTarget.TransformToDevice.Transform(rect.BottomRight);
            SetWindowPos(mainWindowSrc.Handle,
                         IntPtr.Zero,
                         (int)(deviceTopLeft.X),
                         (int)(deviceTopLeft.Y),
                         (int)(Math.Abs(deviceBottomRight.X - deviceTopLeft.X)),
                         (int)(Math.Abs(deviceBottomRight.Y - deviceTopLeft.Y)),
                         0);
        }

        private int accessTimes;

        /// <summary>
        /// Fade out the window on closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FadeOutOnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = ++accessTimes % 2 == 1;
            if (!e.Cancel)
                return;

            // Fadeout
            var anim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300));
            anim.Completed += (s, _) => Close();
            BeginAnimation(OpacityProperty, anim);
        }

        private bool firstOnClosing = true;

        /// <summary>
        /// Send the Application is closing event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            // Only visit the OnClosing Method once
            if (firstOnClosing && IsMainWindow)
            {
                var closingEvent = Kernel.Instance.EventAggregator.PublishMessage(new ApplicationIsClosingEvent());
                e.Cancel = !Kernel.Instance.Controller.ForceClosing && closingEvent.Cancel;
                
                if (!e.Cancel)
                    firstOnClosing = false;
            }

            // Only visit on closing, if it's not prior canceled
            if (!e.Cancel)
                base.OnClosing(e);
        }

        /// <summary>
        /// On closing, do an unsubscribe of the event aggregator
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            if (IsMainWindow)
            {
                Kernel.Instance.EventAggregator.PublishMessage(new ApplicationClosedEvent());
                Kernel.Instance.EventAggregator.Unsubscribe(this);
            }

            base.OnClosed(e);
        }
    }
}
