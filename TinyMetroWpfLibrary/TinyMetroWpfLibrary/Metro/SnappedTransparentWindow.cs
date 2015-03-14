// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TinyMetroWpfLibrary.Docking;
using TinyMetroWpfLibrary.Helper;
using TinyMetroWpfLibrary.Hooks;
using Binding = System.Windows.Data.Binding;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace TinyMetroWpfLibrary.Metro
{

    /// <summary>
    /// The Pin Style defines how the window behaves at the desktop screen
    /// </summary>
    public enum PinStyle
    {
        /// <summary>
        /// Window is not pinned to the desktop
        /// </summary>
        Undefined,

        /// <summary>
        /// Window stays visible, but hides from the user
        /// </summary>
        AlwaysOn,

        /// <summary>
        /// Window is never visible, but shows on demand
        /// </summary>
        AlwaysOff,

        /// <summary>
        /// Window stays always visible
        /// </summary>
        Fixed,

        /// <summary>
        /// Windows will handled like Always off, but more Touch friendly
        /// The display won't disappear, when the mouse is moved out, but disappears
        /// when the user clicks
        /// </summary>
        TouchFriendly,

        /// <summary>
        /// The Window will not be opened automatically by any event, but cloes if the user clis the screen
        /// </summary>
        Manual
    }

    /// <summary>
    /// The snapped transparent window class is used to build transparent, dockable windows
    /// </summary>
    public class SnappedTransparentWindow : TransparentWindow, INotifyPropertyChanged
    {
        #region Private Fields

        /// <summary>
        /// Docking Instance that is used for manipulating the window
        /// </summary>
        private readonly TaskBarDocker docker;

        /// <summary>
        /// True, if the window is hidden and waits for the user to show up
        /// </summary>
        private bool hidden;

        /// <summary>
        /// True, if the user pressed the magic key
        /// </summary>
        private bool isMagicKeyPressed;

        /// <summary>
        /// Latency Timer that will be started the time the user moves the mouse to the hidden window
        /// </summary>
        private readonly Timer latencyTimer = new Timer();

        /// <summary>
        /// Flag indicating whether the window shall slide in on activation or not
        /// </summary>
        private bool showOnActivate = true;

        /// <summary>
        /// Used to get the DPI of the screen
        /// </summary>
        private readonly ScreenResolution screenResolution = new ScreenResolution();

        #endregion

        #region Private Constants

        /// <summary>
        /// The constant that defines the amout of pixel that stays visible, if the window is hidden
        /// </summary>
        private const double HIDE_SIZE = BORDER_SIZE;

        #endregion

        #region Private Storyboards

        private readonly TranslateTransform windowTranslate;

        private readonly Storyboard reduceOpacity;

        private readonly Storyboard fadeIn;
        private readonly Storyboard fadeOut;

        private readonly Storyboard slideIn;
        private readonly Storyboard slideOut;

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty SlideOutWidthProperty =
            DependencyProperty.Register("SlideOutWidth", typeof(double), typeof(SnappedTransparentWindow));

        private static readonly DependencyProperty SlideOutHeightProperty =
            DependencyProperty.Register("SlideOutHeight", typeof(double), typeof(SnappedTransparentWindow));

        public static readonly DependencyProperty SlidingStyleProperty =
            DependencyProperty.Register("PinStyle", typeof(PinStyle), typeof(SnappedTransparentWindow));

        public static readonly DependencyProperty OpacityOnHoldProperty =
            DependencyProperty.Register("OpacityOnHold", typeof(double), typeof(SnappedTransparentWindow), new PropertyMetadata(1.0));

        public static readonly DependencyProperty MagicKeyCodeProperty =
            DependencyProperty.Register("MagicKeyCode", typeof(Keys), typeof(SnappedTransparentWindow), new PropertyMetadata(Keys.LControlKey));

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(SnappedTransparentWindow), new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(300))));

        public static readonly DependencyProperty SlideInLatencyProperty =
            DependencyProperty.Register("SlideInLatency", typeof(TimeSpan), typeof(SnappedTransparentWindow), new PropertyMetadata(TimeSpan.FromMilliseconds(600)));

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the snapped transparent window 
        /// </summary>
        public SnappedTransparentWindow()
        {
            // Window Translation
            windowTranslate = new TranslateTransform();
            RenderTransform = windowTranslate;

            // Reduce Opacity Animation
            reduceOpacity = new Storyboard();

            var animation = new DoubleAnimation();
            BindingOperations.SetBinding(animation, Timeline.DurationProperty, new Binding("AnimationDuration") { Source = this, Mode = BindingMode.OneWay });
            BindingOperations.SetBinding(animation, DoubleAnimation.ToProperty, new Binding("OpacityOnHold") { Source = this, Mode = BindingMode.OneWay });
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            reduceOpacity.Children.Add(animation);

            // FadeIn Animation
            fadeIn = new Storyboard();

            animation = new DoubleAnimation { To = 1.0 };
            BindingOperations.SetBinding(animation, Timeline.DurationProperty, new Binding("AnimationDuration") { Source = this, Mode = BindingMode.OneWay });
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            fadeIn.Children.Add(animation);

            // FadeOut Animation
            fadeOut = new Storyboard();

            animation = new DoubleAnimation { To = 0.0 };
            BindingOperations.SetBinding(animation, Timeline.DurationProperty, new Binding("AnimationDuration") { Source = this, Mode = BindingMode.OneWay });
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            fadeOut.Children.Add(animation);

            // SlideOut Animation
            slideOut = new Storyboard();

            animation = new DoubleAnimation {EasingFunction = new ExponentialEase {EasingMode = EasingMode.EaseIn}};
            BindingOperations.SetBinding(animation, Timeline.DurationProperty, new Binding("AnimationDuration") { Source = this, Mode = BindingMode.OneWay });
            BindingOperations.SetBinding(animation, DoubleAnimation.ToProperty, new Binding("SlideOutWidth") { Source = this, Mode = BindingMode.OneWay });
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            slideOut.Children.Add(animation);

            animation = new DoubleAnimation { EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseIn } };
            BindingOperations.SetBinding(animation, Timeline.DurationProperty, new Binding("AnimationDuration") { Source = this, Mode = BindingMode.OneWay });
            BindingOperations.SetBinding(animation, DoubleAnimation.ToProperty, new Binding("SlideOutHeight") { Source = this, Mode = BindingMode.OneWay });
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            slideOut.Children.Add(animation);

            // SlideIn Animation
            slideIn = new Storyboard();

            animation = new DoubleAnimation { To = 0.0, EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut } };
            BindingOperations.SetBinding(animation, Timeline.DurationProperty, new Binding("AnimationDuration") { Source = this, Mode = BindingMode.OneWay });
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            slideIn.Children.Add(animation);

            animation = new DoubleAnimation { To = 0.0, EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut } };
            BindingOperations.SetBinding(animation, Timeline.DurationProperty, new Binding("AnimationDuration") { Source = this, Mode = BindingMode.OneWay });
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            slideIn.Children.Add(animation);

            Activated += OnWindowActivated;

            // Add the docker
            docker = new TaskBarDocker(this, true);
            docker.OnDockingChanged += OnDockingChanged;

            Loaded += Initialize;
            MouseEnter += OnWindowEnterArea;
            MouseLeave += OnWindowLeaveArea;

            TouchEnter += OnWindowEnterTouchArea;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        /// <summary>
        /// Show the window, if it gets activated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowActivated(object sender, EventArgs e)
        {
            switch (PinStyle)
            {
                case PinStyle.Undefined:
                    break;
                case PinStyle.Manual:
                    break;
                case PinStyle.AlwaysOn:
                    break;
                case PinStyle.AlwaysOff:
                    if (ShowOnActivate)
                        OnSlideIn(sender, e);
                    break;
                case PinStyle.Fixed:
                    break;
                case PinStyle.TouchFriendly:
                    if (ShowOnActivate)
                        OnSlideIn(sender, e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Do Some initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Initialize(object sender, RoutedEventArgs e)
        {
            Dragable = false;
            PinStyle = PinStyle.TouchFriendly;
            Topmost = true;
            Dock(TaskBar.TaskBarEdge.Right, docker.ActiveScreenIndex);
        }

        #endregion

        #region Private Properties

        private DateTime lastAnimation = DateTime.Now;

        /// <summary>
        /// Gets or sets a value indicating whether an animation has been started
        /// </summary>
        protected bool IsAnimating
        {
            get { return (DateTime.Now - lastAnimation) < AnimationDuration.TimeSpan; }
        }

        /// <summary>
        /// Resets the IsAnimating value
        /// </summary>
        private void ResetIsAnimating()
        {
            lastAnimation = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the sliding height
        /// </summary>
        private double SlideOutHeight
        {
            get { return (double)GetValue(SlideOutHeightProperty); }
            set
            {
                if (SlideOutHeight != value)
                {
                    SetValue(SlideOutHeightProperty, value);
                    NotifyPropertyChanged("SlideOutHeight");
                }
            }
        }

        /// <summary>
        /// Gets or sets the sliding width
        /// </summary>
        private double SlideOutWidth
        {
            get { return (double)GetValue(SlideOutWidthProperty); }
            set
            {
                if (SlideOutWidth != value)
                {
                    SetValue(SlideOutWidthProperty, value);
                    NotifyPropertyChanged("SlideOutWidth");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window is currently hidden
        /// </summary>
        protected bool Hidden
        {
            get { return hidden; }
            set
            {
                hidden = value;
                Resizeable = !hidden;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user pressed the magic key
        /// </summary>
        private bool IsMagicKeyPressed
        {
            get
            {
                return isMagicKeyPressed;
            }
            set
            {
                if (isMagicKeyPressed == value)
                    return;

                isMagicKeyPressed = value;
                if (isMagicKeyPressed)
                    OnStopSliding();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Animation Duration. 
        /// </summary>
        /// <remarks>
        /// The Animation Duration defines the time span that the window takes in order to slide in and out, or to fade in and out.
        /// </remarks>
        public Duration AnimationDuration
        {
            get { return (Duration)GetValue(AnimationDurationProperty); }
            set
            {
                if (AnimationDuration != value)
                {
                    SetValue(AnimationDurationProperty, value);
                    NotifyPropertyChanged("AnimationDuration");
                    NotifyPropertyChanged("AnimationDurationMS");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Sliding Style, or better PinningStyle
        /// </summary>
        /// <remarks>
        /// This style defines how the window behaves if the user rushes into it or leave the window alone.
        /// </remarks>
        public PinStyle PinStyle
        {
            get { return (PinStyle)GetValue(SlidingStyleProperty); }
            set
            {
                if (PinStyle != value)
                {
                    switch (PinStyle)
                    {
                        case PinStyle.Undefined:
                            break;
                        case PinStyle.AlwaysOn:
                            HookManager.KeyDown -= OnKeyPressed;
                            HookManager.KeyUp -= OnKeyUp;
                            HookManager.MouseMove -= OnMouseMoveOnScreen;
                            isMagicKeyPressed = false;
                            break;
                        case PinStyle.AlwaysOff:
                            HookManager.MouseClick -= OnMouseClickOnScreen;
                            break;
                        case PinStyle.Manual:
                            HookManager.MouseClick -= OnMouseClickOnScreen;
                            break;
                        case PinStyle.Fixed:
                            break;
                        case PinStyle.TouchFriendly:
                            HookManager.MouseClick -= OnMouseClickOnScreen;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("value");
                    }

                    SetValue(SlidingStyleProperty, value);
                    NotifyPropertyChanged("SlidingStyle");

                    switch (value)
                    {
                        case PinStyle.Undefined:
                            break;
                        case PinStyle.AlwaysOn:
                            HookManager.KeyDown += OnKeyPressed;
                            HookManager.KeyUp += OnKeyUp;
                            HookManager.MouseMove += OnMouseMoveOnScreen;
                            break;
                        case PinStyle.AlwaysOff:
                            HookManager.MouseClick += OnMouseClickOnScreen;
                            AdjustSlidingBehaviour(Width, Height, DockedTo);
                            BeginAnimation(fadeIn);
                            break;
                        case PinStyle.Manual:
                            HookManager.MouseClick += OnMouseClickOnScreen;
                            AdjustSlidingBehaviour(Width, Height, DockedTo);
                            BeginAnimation(fadeIn);
                            break;
                        case PinStyle.Fixed:
                            break;
                        case PinStyle.TouchFriendly:
                            HookManager.MouseClick += OnMouseClickOnScreen;
                            AdjustSlidingBehaviour(Width, Height, DockedTo);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("value");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Slide In Latency
        /// </summary>
        /// <remarks>
        /// The Slide In Latency is important when the user moves the mouse cursor to the hidden window. 
        /// After the amount of time (that is defined in the SlideInLatency) the window shows again.
        /// </remarks>
        public TimeSpan SlideInLatency
        {
            get { return (TimeSpan)GetValue(SlideInLatencyProperty); }
            set
            {
                if (SlideInLatency != value)
                {
                    SetValue(SlideInLatencyProperty, value);
                    NotifyPropertyChanged("SlideInLatency");
                    NotifyPropertyChanged("SlideInLatencyMS");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that defines the opacity of the window the time the user is not working with the window.
        /// </summary>
        public double OpacityOnHold
        {
            get { return (double)GetValue(OpacityOnHoldProperty); }
            set
            {
                if (OpacityOnHold != value)
                {
                    SetValue(OpacityOnHoldProperty, value);
                    NotifyPropertyChanged("OpacityOnHold");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Magic Key. 
        /// </summary>
        /// <remarks>
        /// If the user presses the Magic Key, the window will pinned and does not animate further 
        /// so that the user can change the size or step into it.
        /// </remarks>
        public Keys MagicKeyCode
        {
            get { return (Keys)GetValue(MagicKeyCodeProperty); }
            set
            {
                if (MagicKeyCode != value)
                {
                    SetValue(MagicKeyCodeProperty, value);
                    NotifyPropertyChanged("MagicKeyCode");
                    NotifyPropertyChanged("MagicKey");
                }
            }
        }

        /// <summary>
        /// Gets the Active Screen Index
        /// </summary>
        public int ActiveScreenIndex
        {
            get { return docker.ActiveScreenIndex; }
        }

        /// <summary>
        /// Gets the current docking value
        /// </summary>
        public TaskBar.TaskBarEdge DockedTo
        {
            get { return docker.DockedTo; }
        }

        /// <summary>
        /// Flag indicating whether the window shall slide in on activation or not
        /// </summary>
        public bool ShowOnActivate
        {
            get { return showOnActivate; }
            protected set { showOnActivate = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method docks the window to the defined edge
        /// </summary>
        /// <param name="taskBarEdge">Edge to use</param>
        /// <param name="screen">Active Screen</param>
        public void Dock(TaskBar.TaskBarEdge taskBarEdge, int screen)
        {
            switch (taskBarEdge)
            {
                case TaskBar.TaskBarEdge.NotDocked:
                    ResizeDirections = (ResizeDirectionFlags) 255;
                    break;
                case TaskBar.TaskBarEdge.Left:
                    ResizeDirections = ResizeDirectionFlags.SizeE;
                    break;
                case TaskBar.TaskBarEdge.Top:
                    ResizeDirections = ResizeDirectionFlags.SizeS;
                    break;
                case TaskBar.TaskBarEdge.Right:
                    ResizeDirections = ResizeDirectionFlags.SizeW;
                    break;
                case TaskBar.TaskBarEdge.Bottom:
                    ResizeDirections = ResizeDirectionFlags.SizeN;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("taskBarEdge");
            }

            // Dock the window
            docker.Dock(taskBarEdge, screen);
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Start the animation 
        /// </summary>
        /// <param name="storyboard">Name of the storyboard</param>
        private void BeginAnimation(Storyboard storyboard)
        {
            ResetIsAnimating();
            storyboard.Begin();

            if (object.ReferenceEquals(storyboard, fadeIn) || object.ReferenceEquals(storyboard, slideIn) || object.ReferenceEquals(storyboard, reduceOpacity))
                Hidden = false;

            if (object.ReferenceEquals(storyboard, fadeOut) || object.ReferenceEquals(storyboard, slideOut))
                Hidden = true;
        }

        /// <summary>
        /// Adjust the sliding behaviour depending on the current docking position
        /// </summary>
        /// <param name="width">Window with</param>
        /// <param name="height">Window height</param>
        /// <param name="dockedTo">Window docking position</param>
        private void AdjustSlidingBehaviour(double width, double height, TaskBar.TaskBarEdge dockedTo)
        {
            int offset = 0;
            if (PinStyle == PinStyle.TouchFriendly)
                offset = 16;

            switch (dockedTo)
            {
                case TaskBar.TaskBarEdge.NotDocked:
                    SlideOutWidth = 0;
                    SlideOutHeight = 0;
                    break;
                case TaskBar.TaskBarEdge.Left:
                    SlideOutWidth = -(width - HIDE_SIZE - offset);
                    SlideOutHeight = 0;
                    break;
                case TaskBar.TaskBarEdge.Top:
                    SlideOutWidth = 0;
                    SlideOutHeight = -(height - HIDE_SIZE - offset);
                    break;
                case TaskBar.TaskBarEdge.Right:
                    SlideOutWidth = width - HIDE_SIZE - offset;
                    SlideOutHeight = 0;
                    break;
                case TaskBar.TaskBarEdge.Bottom:
                    SlideOutWidth = 0;
                    SlideOutHeight = height - HIDE_SIZE - offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// This method stops the sliding animation
        /// </summary>
        private void OnStopSliding()
        {
            Storyboard sliding, opacity;
            try
            {
                switch (PinStyle)
                {
                    case PinStyle.Undefined:
                        break;

                    case PinStyle.AlwaysOn:
                        if ((sliding = slideOut) != null && sliding.GetCurrentState() == ClockState.Active)
                            sliding.Stop();

                        if ((sliding = slideIn) != null && sliding.GetCurrentState() == ClockState.Active)
                            sliding.SkipToFill();

                        if ((opacity = fadeIn) != null && opacity.GetCurrentState() == ClockState.Active)
                            opacity.SkipToFill();
                        break;

                    case PinStyle.AlwaysOff:
                    case PinStyle.TouchFriendly:
                    case PinStyle.Manual:
                        if ((sliding = slideOut) != null && sliding.GetCurrentState() == ClockState.Active)
                            sliding.Stop();

                        if ((sliding = slideIn) != null && sliding.GetCurrentState() == ClockState.Active)
                            sliding.SkipToFill();
                        break;

                    case PinStyle.Fixed:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing here
                // It's only because of accessing GetCurrentState(), if the animation has never run
            }
        }

        /// <summary>
        /// Send Property notification event
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Slider Events

        /// <summary>
        /// Called when the rendering size of the window changes
        /// </summary>
        /// <param name="sizeInfo">Information about the size changes</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            AdjustSlidingBehaviour(sizeInfo.NewSize.Width, sizeInfo.NewSize.Height, docker.DockedTo);
            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <summary>
        /// Used to start the SlideIn Animation
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        protected void OnSlideIn(object sender, EventArgs e)
        {
            Storyboard sliding;
            latencyTimer.Stop();
            if ((sliding = slideOut) != null)
                sliding.Stop();

            BeginAnimation(slideIn);
        }

        /// <summary>
        /// Used to start the SlideIn Animation
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        protected void OnSlideOut(object sender, EventArgs e)
        {
            Storyboard sliding;
            latencyTimer.Stop();
            if ((sliding = slideIn) != null)
                sliding.Stop();

            BeginAnimation(slideOut);
        }

        /// <summary>
        /// Called when the user leaves the window
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="mouseEventArgs">Event arguments</param>
        private void OnWindowLeaveArea(object sender, MouseEventArgs mouseEventArgs)
        {
            switch (PinStyle)
            {
                case PinStyle.Undefined:
                case PinStyle.TouchFriendly:
                case PinStyle.Manual:
                    break;

                case PinStyle.AlwaysOn:
                    if (!Hidden)
                        BeginAnimation(reduceOpacity);
                    break;

                case PinStyle.AlwaysOff:
                    latencyTimer.Stop();
                    break;

                case PinStyle.Fixed:
                    if (!Hidden)
                        BeginAnimation(reduceOpacity);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Called when the user presses a key
        /// </summary>
        /// <param name="sender">Sening object</param>
        /// <param name="keyEventArgs">Key events</param>
        private void OnKeyPressed(object sender, KeyEventArgs keyEventArgs)
        {
            IsMagicKeyPressed = keyEventArgs.KeyCode == MagicKeyCode;
        }

        /// <summary>
        /// Called when the user releases a key
        /// </summary>
        /// <param name="sender">Sening object</param>
        /// <param name="keyEventArgs">Key events</param>
        private void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            IsMagicKeyPressed = false;
        }

        /// <summary>
        /// Called when the user clicks on the screen
        /// </summary>
        /// <param name="sender">Sening object</param>
        /// <param name="e">Event arguments</param>
        private void OnMouseClickOnScreen(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var formLocation = new System.Drawing.Point((int)screenResolution.ConvertXDpi(e.Location.X), (int)screenResolution.ConvertYDpi(e.Location.Y));

            // Only interessting for Touch Friendly)
            if (PinStyle != PinStyle.TouchFriendly && PinStyle != PinStyle.AlwaysOff && PinStyle != PinStyle.Manual)
                return;

            // if the screen is not hidden, than everything is ok
            if (Hidden || IsAnimating)
                return;

            // Check if the user clicks outside the current window
            bool mustHide = !(new Rectangle((int)Left, (int)Top, (int)Width, (int)Height).Contains(formLocation));
            if (!mustHide) 
                return;

            // Check if the user pressed the magic key
            Storyboard sliding;
            latencyTimer.Stop();
            if (IsMagicKeyPressed && PinStyle == PinStyle.AlwaysOn) 
                return;

            // Stop the current animation and hide the window
            if ((sliding = slideIn) != null)
                sliding.Stop();

            BeginAnimation(slideOut);
        }

        /// <summary>
        /// Called when the user moves the mouse onto the screen
        /// </summary>
        /// <param name="sender">Sening object</param>
        /// <param name="e">Event arguments</param>
        private void OnMouseMoveOnScreen(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Only interessting for Sliding Style AlwaysOn
            if (PinStyle != PinStyle.AlwaysOn)
                return;

            // if the screen is not hidden, than everything is ok
            if (!Hidden || IsAnimating)
                return;

            bool mustShow = !(new Rectangle((int)Left, (int)Top, (int)Width, (int)Height).Contains(e.Location));

            if (mustShow)
                BeginAnimation(reduceOpacity);
        }

        /// <summary>
        /// Called when the user moves the mouse into the window
        /// </summary>
        /// <param name="sender">Sening object</param>
        /// <param name="mouseEventArgs">Event arguments</param>
        private void OnWindowEnterArea(object sender, MouseEventArgs mouseEventArgs)
        {
            switch (PinStyle)
            {
                case PinStyle.Undefined:
                case PinStyle.Manual:
                    break;

                case PinStyle.AlwaysOn:
                    if (!IsMagicKeyPressed)
                    {
                        if (!Hidden)
                            BeginAnimation(fadeOut);
                    }
                    else
                        BeginAnimation(fadeIn);
                    break;

                case PinStyle.AlwaysOff:
                    if (SlideInLatency.TotalMilliseconds > 0)
                    {
                        latencyTimer.Interval = (int)SlideInLatency.TotalMilliseconds;
                        latencyTimer.Tick += OnSlideIn;
                        latencyTimer.Start();
                    }
                    else
                        OnSlideIn(sender, mouseEventArgs);
                    break;

                case PinStyle.TouchFriendly:
                    break;

                case PinStyle.Fixed:
                    BeginAnimation(fadeIn);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Called when the user clicks into the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (PinStyle)
            {
                case PinStyle.Undefined:
                case PinStyle.Manual:
                    break;
                case PinStyle.AlwaysOn:
                    break;
                case PinStyle.AlwaysOff:
                    if (!ShowOnActivate)
                        OnSlideIn(sender, e);
                    break;
                case PinStyle.Fixed:
                    break;
                case PinStyle.TouchFriendly:
                    if (!ShowOnActivate)
                        OnSlideIn(sender, e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Called when the user moves the finger into the window
        /// </summary>
        /// <param name="sender">Sening object</param>
        /// <param name="touchEventArgs">Event arguments</param>
        private void OnWindowEnterTouchArea(object sender, TouchEventArgs touchEventArgs)
        {
            switch (PinStyle)
            {
                case PinStyle.Undefined:
                case PinStyle.Manual:
                    break;

                case PinStyle.AlwaysOn:
                    if (!IsMagicKeyPressed)
                    {
                        if (!Hidden)
                            BeginAnimation(fadeOut);
                    }
                    else
                        BeginAnimation(fadeIn);
                    break;

                case PinStyle.AlwaysOff:
                    if (SlideInLatency.TotalMilliseconds > 0)
                    {
                        latencyTimer.Interval = (int)SlideInLatency.TotalMilliseconds;
                        latencyTimer.Tick += OnSlideIn;
                        latencyTimer.Start();
                    }
                    else
                        OnSlideIn(sender, touchEventArgs);
                    break;

                case PinStyle.TouchFriendly:
                    OnSlideIn(sender, touchEventArgs);
                    break;

                case PinStyle.Fixed:
                    BeginAnimation(fadeIn);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Called when the docking style Changed
        /// </summary>
        /// <param name="dockedTo">New DockedTo Value</param>
        private void OnDockingChanged(TaskBar.TaskBarEdge dockedTo)
        {
            AdjustSlidingBehaviour(Width, Height, dockedTo);
        }
 
        #endregion

        /// <summary>
        /// For the snapped window use the .NET managed methods, even they are slower
        /// </summary>
        /// <param name="rect">Visual Rect</param>
        protected override void SetWindowVisualRect(Rect rect)
        {
            Left = rect.Left;
            Top = rect.Top;
            Width = rect.Width;
            Height = rect.Height;
        }
    }
}