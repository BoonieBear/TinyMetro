using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TinyMetroWpfLibrary.Controls
{
    public class ToggleSwitch : Control
    {
        #region Constants

        private const string CommonStates = "CommonStates";
        private const string NormalState = "Normal";
        private const string DisabledState = "Disabled";
        private const string MouseOverState = "MouseOver";

        private const string CheckStates = "CheckStates";
        private const string CheckedState = "Checked";
        private const string DraggingState = "Dragging";
        private const string UncheckedState = "Unchecked";

        private const string FocusStates = "FocusStates";
        private const string FocusedState = "Focused";
        private const string UnfocusedState = "Unfocused";

        private const string SwitchRootPart = "SwitchRoot";
        private const string SwitchCheckedPart = "SwitchChecked";
        private const string SwitchUncheckedPart = "SwitchUnchecked";
        private const string SwitchThumbPart = "SwitchThumb";
        //private const string SwitchTrackPart = "SwitchTrack";

        private const string CommonPropertiesCategory = "Common Properties";
        private const string AppearanceCategory = "Appearance";

        #endregion

        static ToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
        }

        #region Fields

        /// <summary>
        /// True if the mouse has been captured by this control, false otherwise.
        /// </summary> 
        private bool _isMouseCaptured;

        /// <summary> 
        /// True if the SPACE key is currently pressed, false otherwise. 
        /// </summary>
        private bool _isSpaceKeyDown;

        /// <summary>
        /// True if the mouse's left button is currently down, false otherwise. 
        /// </summary>
        private bool _isMouseLeftButtonDown;

        /// <summary> 
        /// Last known position of the mouse with respect to this Button.
        /// </summary> 
        private Point _mousePosition;

        /// <summary>
        /// True if visual state changes are suspended; false otherwise. 
        /// </summary>
        private bool _suspendStateChanges;

        #endregion

        #region Properties

        protected Thumb SwitchThumb { get; set; }
        protected FrameworkElement SwitchChecked { get; set; }
        protected FrameworkElement SwitchUnchecked { get; set; }
        protected FrameworkElement SwitchRoot { get; set; }
        //protected FrameworkElement SwitchTrack { get; set; }

        /// <summary>
        /// The current offset of the Thumb.
        /// </summary>
        private double Offset
        {
            get
            {
                return Canvas.GetLeft(SwitchThumb);
            }
            set
            {
                SwitchChecked.BeginAnimation(WidthProperty, null);
                SwitchThumb.BeginAnimation(Canvas.LeftProperty, null);
                Canvas.SetLeft(SwitchThumb, value);
                SwitchChecked.Width = value;
            }
        }

        /// <summary>
        /// The current offset of the Thumb when it's in the Checked state.
        /// </summary>
        protected double CheckedOffset { get; set; }

        /// <summary>
        /// The current offset of the Thumb when it's in the Unchecked state.
        /// </summary>
        protected double UncheckedOffset { get; set; }

        /// <summary>
        /// The offset of the thumb while it's being dragged.
        /// </summary>
        protected double DragOffset { get; set; }

        /// <summary>
        /// Gets or sets whether the thumb position is being manipulated.
        /// </summary>
        protected bool IsDragging { get; set; }

        /// <summary> 
        /// Gets a value that indicates whether a ToggleSwitch is currently pressed.
        /// </summary> 
        public bool IsPressed { get; protected set; }


        private PropertyPath CanvasLeftPath
        {
            get { return new PropertyPath("(Canvas.Left)"); }
        }

        private PropertyPath WidthPath
        {
            get
            {
                return new PropertyPath("Width");
            }
        }

        #endregion


        #region Elasticity (Dependency Property)
        public double Elasticity
        {
            get
            {
                return 0.5;
            }
        }

        #endregion


        #region ThumbSize (DependencyProperty)

        ///<summary>
        /// DependencyProperty for the <see cref="ThumbSize">ThumbSize</see> property.
        ///</summary>
        public static readonly DependencyProperty ThumbSizeProperty =
            DependencyProperty.Register("ThumbSize", typeof(double), typeof(ToggleSwitch),
            new FrameworkPropertyMetadata(40.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, OnLayoutDependancyPropertyChanged));

        ///<summary>
        /// The size of the toggfle switch's <see cref="Thumb">thumb</see>.
        ///</summary>
        [Category(AppearanceCategory)]
        [Description("The size of the toggle switch's thumb.")]
        public double ThumbSize
        {
            get { return (double)GetValue(ThumbSizeProperty); }
            set { SetValue(ThumbSizeProperty, value); }
        }

        #endregion

        #region IsChecked (DependencyProperty)

        ///<summary>
        /// DependencyProperty for the <see cref="IsChecked">IsChecked</see> property.
        ///</summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleSwitch),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange, OnIsCheckedChanged));


        ///<summary>
        /// Gets or sets whether the control is in the checked state.
        ///</summary>
        [Category(CommonPropertiesCategory)]
        [Description("Gets or sets whether the control is in the checked state.")]
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ToggleSwitch)d;

            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue)
                {
                    control.InvokeChecked(new RoutedEventArgs());
                }
                else
                {
                    control.InvokeUnchecked(new RoutedEventArgs());
                }
            }

            control.ChangeCheckStates(true);
        }

        #endregion


        #region Events

        ///<summary>
        /// Event raised when the toggle switch is unchecked.
        ///</summary>
        public event RoutedEventHandler Unchecked;

        protected void InvokeUnchecked(RoutedEventArgs e)
        {
            RoutedEventHandler handler = Unchecked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        ///<summary>
        /// Event raised when the toggle switch is checked.
        ///</summary>
        public event RoutedEventHandler Checked;

        protected void InvokeChecked(RoutedEventArgs e)
        {
            RoutedEventHandler handler = Checked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        /// <summary> 
        /// Initializes a new instance of the ToggleSwitch class.
        /// </summary>
        public ToggleSwitch()
        {
            Loaded += delegate { UpdateVisualState(false); };
            IsEnabledChanged += OnIsEnabledChanged;
        }

        /// <summary>
        /// Raised when a drag has started on the <see cref="Thumb">Thumb</see>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            IsDragging = true;
            DragOffset = Offset;
            ChangeCheckStates(false);
        }

        /// <summary>
        /// Raised while dragging the <see cref="Thumb">Thumb</see>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            DragOffset += e.HorizontalChange;
            Offset = Math.Max(UncheckedOffset, Math.Min(CheckedOffset, DragOffset));
        }

        /// <summary>
        /// Raised when the dragging of the <see cref="Thumb">Thumb</see> has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            IsDragging = false;
            bool click = false;
            double fullThumbWidth = SwitchThumb.ActualWidth + SwitchThumb.BorderThickness.Left + SwitchThumb.BorderThickness.Right;

            if ((!IsChecked && DragOffset > CheckedOffset / 2)
                 || (IsChecked && DragOffset < CheckedOffset / 2))
            {
                double edge = IsChecked ? CheckedOffset : UncheckedOffset;
                if (Offset != edge)
                {
                    click = true;
                }
            }
            else if (DragOffset == CheckedOffset || DragOffset == UncheckedOffset)
            {
                click = true;
            }
            else
            {
                ChangeCheckStates(true);
            }

            if (click)
            {
                OnClick();
            }

            DragOffset = 0;
        }

        /// <summary>
        /// Recalculated the layout of the control.
        /// </summary>
        private void LayoutControls()
        {
            if (SwitchThumb == null || SwitchRoot == null)
            {
                return;
            }

            CheckedOffset = SwitchRoot.ActualWidth - SwitchThumb.ActualWidth;
            UncheckedOffset = 0;
            if (!IsDragging)
            {
                Offset = IsChecked ? CheckedOffset : UncheckedOffset;
                ChangeCheckStates(false);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RemoveEventHandlers();
            GetTemplateChildren();
            AddEventHandlers();

            LayoutControls();

            VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, false);
            ChangeCheckStates(false);
        }

        /// <summary>
        /// Initializes the control's template parts.
        /// </summary>
        private void GetTemplateChildren()
        {
            SwitchRoot = GetTemplateChild(SwitchRootPart) as FrameworkElement;
            SwitchThumb = GetTemplateChild(SwitchThumbPart) as Thumb;
            SwitchChecked = GetTemplateChild(SwitchCheckedPart) as FrameworkElement;
            SwitchUnchecked = GetTemplateChild(SwitchUncheckedPart) as FrameworkElement;
            //SwitchTrack = GetTemplateChild(SwitchTrackPart) as FrameworkElement;
        }

        /// <summary>
        /// Subscribe event listeners.
        /// </summary>
        private void AddEventHandlers()
        {
            if (SwitchThumb != null)
            {
                SwitchThumb.DragStarted += OnDragStarted;
                SwitchThumb.DragDelta += OnDragDelta;
                SwitchThumb.DragCompleted += OnDragCompleted;
            }
            SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// Unsubscribe event listeners.
        /// </summary>
        private void RemoveEventHandlers()
        {
            if (SwitchThumb != null)
            {
                SwitchThumb.DragStarted -= OnDragStarted;
                SwitchThumb.DragDelta -= OnDragDelta;
                SwitchThumb.DragCompleted -= OnDragCompleted;
            }
            SizeChanged -= OnSizeChanged;
        }


        /// <summary>
        /// Called when the control is clicked.
        /// </summary>
        private void OnClick()
        {
            IsChecked = !IsChecked;
        }

        /// <summary>
        /// Raised when the size of the control has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            LayoutControls();
        }

        /// <summary> 
        /// Capture the mouse. 
        /// </summary>
        internal void CaptureMouseInternal()
        {
            if (!_isMouseCaptured)
            {
                _isMouseCaptured = CaptureMouse();
            }
        }

        /// <summary>
        /// Release mouse capture if we already had it. 
        /// </summary>
        private void ReleaseMouseCaptureInternal()
        {
            ReleaseMouseCapture();
            _isMouseCaptured = false;
        }

        /// <summary>
        /// Raised when a dependency property that affects the control's layout has changed.
        /// </summary>
        /// <param name="d">The ToggleSwitch control</param>
        /// <param name="e"></param>
        private static void OnLayoutDependancyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((ToggleSwitch)d).LayoutControls();
            }
        }

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _suspendStateChanges = true;
            if (!IsEnabled)
            {
                IsPressed = false;
                _isMouseCaptured = false;
                _isSpaceKeyDown = false;
                _isMouseLeftButtonDown = false;
            }

            _suspendStateChanges = false;
            UpdateVisualState();
        }


        /// <summary> 
        /// Responds to the LostFocus event.
        /// </summary> 
        /// <param name="e">The event data for the LostFocus event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            IsPressed = false;
            ReleaseMouseCaptureInternal();
            _isSpaceKeyDown = false;

            _suspendStateChanges = false;
            UpdateVisualState();
        }


        /// <summary> 
        /// Responds to the MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e"> 
        /// The event data for the MouseLeftButtonDown event.
        /// </param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
            {
                return;
            }

            _isMouseLeftButtonDown = true;

            if (!IsEnabled)
            {
                return;
            }

            e.Handled = true;
            _suspendStateChanges = true;
            Focus();

            CaptureMouseInternal();
            if (_isMouseCaptured)
            {
                IsPressed = true;
            }

            _suspendStateChanges = false;
            UpdateVisualState();
        }

        /// <summary> 
        /// Responds to the MouseLeftButtonUp event.
        /// </summary>
        /// <param name="e"> 
        /// The event data for the MouseLeftButtonUp event.
        /// </param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (e.Handled)
            {
                return;
            }

            _isMouseLeftButtonDown = false;

            if (!IsEnabled)
            {
                return;
            }

            e.Handled = true;
            if (!_isSpaceKeyDown && IsPressed)
            {
                OnClick();
            }

            if (!_isSpaceKeyDown)
            {
                ReleaseMouseCaptureInternal();
                IsPressed = false;
            }
        }

        /// <summary> 
        /// Responds to the MouseMove event.
        /// </summary> 
        /// <param name="e">The event data for the MouseMove event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _mousePosition = e.GetPosition(this);

            if (_isMouseLeftButtonDown &&
                 IsEnabled &&
                 _isMouseCaptured &&
                 !_isSpaceKeyDown)
            {
                IsPressed = IsValidMousePosition();
            }
        }

        /// <summary>
        /// Determine if the mouse is above the button based on its last known 
        /// position.
        /// </summary>
        /// <returns> 
        /// True if the mouse is considered above the button, false otherwise. 
        /// </returns>
        private bool IsValidMousePosition()
        {
            return (_mousePosition.X >= 0.0) &&
                     (_mousePosition.X <= ActualWidth) &&
                     (_mousePosition.Y >= 0.0) &&
                     (_mousePosition.Y <= ActualHeight);
        }

        protected bool GoToState(bool useTransitions, string stateName)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        protected virtual void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                GoToState(useTransitions, DisabledState);
            }
            else
            {
                GoToState(useTransitions, IsMouseOver ? MouseOverState : NormalState);
            }

            if (IsFocused && IsEnabled)
            {
                GoToState(useTransitions, FocusedState);
            }
            else
            {
                GoToState(useTransitions, UnfocusedState);
            }
        }

        protected void UpdateVisualState(bool useTransitions = true)
        {
            if (!_suspendStateChanges)
            {
                ChangeVisualState(useTransitions);
            }
        }

        /// <summary>
        /// Updates the control's layout to reflect the current <see cref="IsChecked">IsChecked</see> state.
        /// </summary>
        /// <param name="useTransitions">Whether to use transitions during the layout change.</param>
        protected virtual void ChangeCheckStates(bool useTransitions)
        {
            var state = IsChecked ? CheckedState : UncheckedState;

            if (IsDragging)
            {
                VisualStateManager.GoToState(this, DraggingState + state, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, state, useTransitions);
                if (SwitchThumb != null)
                {
                    VisualStateManager.GoToState(SwitchThumb, state, useTransitions);
                }
            }

            if (SwitchThumb == null)
            {
                return;
            }

            var storyboard = new Storyboard();
            var duration = new Duration(useTransitions ? TimeSpan.FromMilliseconds(100) : TimeSpan.Zero);
            var backgroundAnimation = new DoubleAnimation();
            var thumbAnimation = new DoubleAnimation();

            backgroundAnimation.Duration = duration;
            thumbAnimation.Duration = duration;

            double offset = IsChecked ? CheckedOffset : UncheckedOffset;
            backgroundAnimation.To = offset;
            thumbAnimation.To = offset;
            storyboard.Children.Add(backgroundAnimation);
            storyboard.Children.Add(thumbAnimation);

            Storyboard.SetTarget(backgroundAnimation, SwitchChecked);
            Storyboard.SetTarget(thumbAnimation, SwitchThumb);

            Storyboard.SetTargetProperty(backgroundAnimation, WidthPath);
            Storyboard.SetTargetProperty(thumbAnimation, CanvasLeftPath);

            storyboard.Begin();
        }
    }
}
