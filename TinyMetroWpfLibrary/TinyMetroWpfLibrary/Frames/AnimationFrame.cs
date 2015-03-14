// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace TinyMetroWpfLibrary.Frames
{
    public class AnimationFrame : Frame
    {
        /// <summary>
        /// Initializes a new instance of the Animation Frame
        /// </summary>
        public AnimationFrame()
        {
            // watch for navigations
            Navigating += OnNavigating;
        }


        #region Fade Duration Dependency Property

        public static readonly DependencyProperty FadeDurationProperty =
            DependencyProperty.Register("FadeDuration", typeof(Duration), typeof(AnimationFrame),
                new FrameworkPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(300))));

        public static readonly DependencyProperty FadeOffsetProperty =
            DependencyProperty.Register("FadeOffset", typeof(TimeSpan), typeof(AnimationFrame),
                new FrameworkPropertyMetadata(TimeSpan.FromMilliseconds(0)));

        /// <summary>
        /// FadeDuration will be used as the duration for Fade Out and Fade In animations
        /// </summary>
        public Duration FadeDuration
        {
            get { return (Duration)GetValue(FadeDurationProperty); }
            set { SetValue(FadeDurationProperty, value); }
        }

        /// <summary>
        /// FadeOffset defines, how long the thread sleeps until the fade starts
        /// </summary>
        public TimeSpan FadeOffset
        {
            get { return (TimeSpan)GetValue(FadeOffsetProperty); }
            set { SetValue(FadeOffsetProperty, value); }
        }

        #endregion

        #region Slide Duration Dependency Property

        public static readonly DependencyProperty SlideDurationProperty =
            DependencyProperty.Register("SlideorGrowDuration", typeof(Duration), typeof(AnimationFrame),
                new FrameworkPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(300))));

        /// <summary>
        /// slideDuration will be used as the duration for slide Out and slide In animations
        /// </summary>
        public Duration SlideorGrowDuration
        {
            get { return (Duration)GetValue(SlideDurationProperty); }
            set { SetValue(SlideDurationProperty, value); }
        }

        #endregion

        #region Animation Mode Dependency Property

        public static readonly DependencyProperty AnimationModeProperty =
            DependencyProperty.Register("AnimationMode", typeof(AnimationMode), typeof(AnimationFrame),
                new FrameworkPropertyMetadata(AnimationMode.Fade));

        /// <summary>
        /// slideDuration will be used as the duration for slide Out and slide In animations
        /// </summary>
        public AnimationMode AnimationMode
        {
            get { return (AnimationMode)GetValue(AnimationModeProperty); }
            set { SetValue(AnimationModeProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether an animation is running
        /// </summary>
        public bool IsNavigating { get; set; }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            // get a reference to the frame's content presenter
            // this is the element we will fade in and out
            contentPresenter = GetTemplateChild("PART_FrameCP") as ContentPresenter;
            base.OnApplyTemplate();
        }

        #endregion

        #region Navigation Methods

        /// <summary>
        /// Executed on Navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (Content == null || contentPresenter == null || e.NavigationMode == NavigationMode.Refresh)
                return;

            switch (AnimationMode)
            {
                case AnimationMode.Fade:
                    // if we did not internally initiate the navigation:
                    //   1. cancel the navigation,
                    //   2. cache the target,
                    //   3. disable hittesting during the fade, and
                    //   4. fade out the current content
                    if (!navigationEvents.Contains(e.Uri))
                    {
                        IsNavigating = true;
                        e.Cancel = true;
                        navArgs.Enqueue(e);
                        IsHitTestVisible = contentPresenter.IsHitTestVisible = false;

                        // Start Animation
                        var da = new DoubleAnimation(0.0d, FadeDuration)
                        {
                            DecelerationRatio = 1.0d,
                            BeginTime = FadeOffset
                        };

                        da.Completed += OnFadeOutCompleted;
                        contentPresenter.BeginAnimation(OpacityProperty, da, HandoffBehavior.Compose);
                    }
                    else
                        navigationEvents.Remove(e.Uri);
                    break;

                case AnimationMode.Slide:
                    // if we did not internally initiate the navigation:
                    //   1. cancel the navigation,
                    //   2. cache the target,
                    //   3. disable hittesting during the slide, and
                    //   4. slide out the current content
                    if (!navigationEvents.Contains(e.Uri))
                    {
                        IsNavigating = true;
                        e.Cancel = true;
                        navArgs.Enqueue(e);
                        IsHitTestVisible = contentPresenter.IsHitTestVisible = false;

                        ExecuteFirstTransition(e.NavigationMode);
                    }
                    else
                        navigationEvents.Remove(e.Uri);
                    break;

                case AnimationMode.Grow:
                    //页面缩放切换
                    if (!navigationEvents.Contains(e.Uri))
                    {
                        IsNavigating = true;
                        e.Cancel = true;
                        navArgs.Enqueue(e);
                        IsHitTestVisible = contentPresenter.IsHitTestVisible = false;

                        ExecuteGrowOut(e.NavigationMode);
                    }
                    else
                        navigationEvents.Remove(e.Uri);
                    break;
                case AnimationMode.SmoothSlide:
                    if (!navigationEvents.Contains(e.Uri))
                    {
                        IsNavigating = true;
                        e.Cancel = true;
                        navArgs.Enqueue(e);
                        IsHitTestVisible = contentPresenter.IsHitTestVisible = false;
                        ExecuteSmoothSlide();
                    }
                    else
                        navigationEvents.Remove(e.Uri);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ExecuteSmoothSlide()
        {
            FrameworkElement srcElement = contentPresenter;
            RenderTargetBitmap rtb = RenderBitmap(srcElement);
            

            item = new Rectangle()
            {
                Fill = new ImageBrush(BitmapFrame.Create(rtb))
            };

            if (contentPresenter != null)
            {
                IsHitTestVisible = contentPresenter.IsHitTestVisible = true;

                

                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, 
                    (ThreadStart)(AnimationFrame_Navigated));

            }
        }

        private void AnimationFrame_Navigated()
        {
            var nav = navArgs.Dequeue();
            navigationEvents.Add(nav.Uri);

            switch (nav.NavigationMode)
            {
                case NavigationMode.New:
                    target = -ActualWidth;
                    if (nav.Uri == null)
                    {
                        NavigationService.Navigate(nav.Content, nav.ExtraData);
                    }
                    else
                    {
                        NavigationService.Navigate(nav.Uri, nav.ExtraData);
                    }
                    break;

                case NavigationMode.Back:
                    target = ActualWidth;
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
                    break;

                case NavigationMode.Forward:
                    if (NavigationService.CanGoForward)
                        NavigationService.GoForward();
                    break;

                case NavigationMode.Refresh:
                    NavigationService.Refresh();
                    break;
            }
            var container = Parent as Grid;
            if (container != null)
            {
                container.Children.Add(item);
                //变小
                var srctranslate = new ScaleTransform(1, 1,ActualWidth/2,ActualHeight/2);
                var translate = new TranslateTransform(0, 0);
                var srcgrouptranslate = new TransformGroup();
                srcgrouptranslate.Children.Add(srctranslate);
                srcgrouptranslate.Children.Add(translate);
                item.RenderTransform = srcgrouptranslate;

                // Create the animation
                var srcda = new DoubleAnimation(1.0d, 0.9d, SlideorGrowDuration)
                {
                    EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn }
                };
                //Begin animation
                srctranslate.BeginAnimation(ScaleTransform.ScaleXProperty, srcda, HandoffBehavior.Compose);
                srctranslate.BeginAnimation(ScaleTransform.ScaleYProperty, srcda, HandoffBehavior.Compose);
                var dstslider = contentPresenter;

                // Create a translation Transformation for the Sliding Content
                var dstScaletranslate = new ScaleTransform(1, 1, ActualWidth / 2, ActualHeight / 2);
                var dsttranslate = new TranslateTransform(0, 0);
                var dstgrouptranslate = new TransformGroup();
                dstgrouptranslate.Children.Add(dstScaletranslate);
                dstgrouptranslate.Children.Add(dsttranslate);
                dstslider.RenderTransform = dstgrouptranslate;

                // Create the animation
                var dstda = new DoubleAnimation(1.0d, 0.9d, SlideorGrowDuration)
                {
                    EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn }
                    
                };
                dstda.Completed += (sender,e) => BeginSlideAnimation(item, target, container,srcgrouptranslate,dstgrouptranslate);

                //Begin animation
                dstScaletranslate.BeginAnimation(ScaleTransform.ScaleXProperty, dstda, HandoffBehavior.Compose);
                dstScaletranslate.BeginAnimation(ScaleTransform.ScaleYProperty, dstda, HandoffBehavior.Compose);

            }
        }


        private void BeginSlideAnimation(Rectangle item1,double target, Grid container,TransformGroup rectGroup,TransformGroup dstGroup)
        {
            item = item1;
            mainGrid = container;
            // Create the animation
            var doubleAnimation = new DoubleAnimation(0.0d, target, SlideorGrowDuration)
            {
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn }
            };
            //Begin animation
            rectGroup.Children[1].BeginAnimation(TranslateTransform.XProperty, doubleAnimation, HandoffBehavior.Compose);
            
            
            // Create the animation
            var dstda = new DoubleAnimation(-target, 0.0d, SlideorGrowDuration)
            {
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn }
            };
           
            dstGroup.Children[1].BeginAnimation(TranslateTransform.XProperty, dstda, HandoffBehavior.Compose);
            //变大
            
            var scaleda = new DoubleAnimation(0.9d, 1.0d, SlideorGrowDuration)
            {
                BeginTime = SlideorGrowDuration.TimeSpan,
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn }
            };
            scaleda.Completed += (sender, e) =>
            {

                item.Fill = null;
                mainGrid.Children.Remove(item);
                IsNavigating = false;
            };
            //Begin animation
            rectGroup.Children[0].BeginAnimation(ScaleTransform.ScaleXProperty, scaleda, HandoffBehavior.Compose);
            rectGroup.Children[0].BeginAnimation(ScaleTransform.ScaleYProperty, scaleda, HandoffBehavior.Compose);
            dstGroup.Children[0].BeginAnimation(ScaleTransform.ScaleXProperty, scaleda, HandoffBehavior.Compose);
            dstGroup.Children[0].BeginAnimation(ScaleTransform.ScaleYProperty, scaleda, HandoffBehavior.Compose);
            
       
        }

        public RenderTargetBitmap RenderBitmap(FrameworkElement element)
        {
            double topLeft = 0;
            double topRight = 0;
            int width = (int)element.ActualWidth;
            int height = (int)element.ActualHeight;
            double dpiX = 96; // this is the magic number
            double dpiY = 96; // this is the magic number

            PixelFormat pixelFormat = PixelFormats.Default;
            VisualBrush elementBrush = new VisualBrush(element);
            DrawingVisual visual = new DrawingVisual();
            DrawingContext dc = visual.RenderOpen();

            dc.DrawRectangle(elementBrush, null, new Rect(topLeft, topRight, width, height));
            dc.Close();

            RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height, dpiX, dpiY, pixelFormat);

            bitmap.Render(visual);
            return bitmap;
        } 
        #region FadeOutMode

        /// <summary>
        /// Executed when the fade out has been completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFadeOutCompleted(object sender, EventArgs e)
        {
            // after the fade out
            //   1. re-enable hittesting
            //   2. initiate the delayed navigation
            //   3. invoke the FadeIn animation at Loaded priority
            var clock = sender as AnimationClock;
            if (clock != null)
                clock.Completed -= OnFadeOutCompleted;

            if (contentPresenter != null)
            {
                IsHitTestVisible = contentPresenter.IsHitTestVisible = true;

                var nav = navArgs.Dequeue();
                navigationEvents.Add(nav.Uri);

                switch (nav.NavigationMode)
                {
                    case NavigationMode.New:
                        if (nav.Uri == null)
                        {
                            NavigationService.Navigate(nav.Content, nav.ExtraData);
                        }
                        else
                        {
                            NavigationService.Navigate(nav.Uri, nav.ExtraData);
                        }
                        break;

                    case NavigationMode.Back:
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        break;

                    case NavigationMode.Forward:
                        if (NavigationService.CanGoForward)
                            NavigationService.GoForward();
                        break;

                    case NavigationMode.Refresh:
                        NavigationService.Refresh();
                        break;
                }

                // Start Animation
                var da = new DoubleAnimation(1.0d, FadeDuration) {AccelerationRatio = 1.0d};
                da.Completed += (s, ev) => IsNavigating = false;
                contentPresenter.BeginAnimation(OpacityProperty, da, HandoffBehavior.Compose);
            }
        }

        #endregion


        #region slidemode
        /// <summary>
        /// Executes the first transition.
        /// </summary>
        private void ExecuteFirstTransition(NavigationMode mode)
        {
            double target = 0;
            var easingMode = EasingMode.EaseInOut;

            switch (mode)
            {
                case NavigationMode.Back:
                    target = ActualWidth;
                    easingMode = EasingMode.EaseIn;
                    break;
                case NavigationMode.Forward:
                case NavigationMode.New:
                    target = -ActualWidth;
                    easingMode = EasingMode.EaseIn;
                    break;
            }

            var slider = contentPresenter;

            // Create a translation Transformation for the Sliding Content
            var translate = new TranslateTransform(0, 0);
            slider.RenderTransform = translate;

            // Create the animation
            var da = new DoubleAnimation(0.0d, target, SlideorGrowDuration)
            {
                EasingFunction = new QuarticEase { EasingMode = easingMode }
            };

            // Start the Animation
            da.Completed += SlideOutCompleted;
           // var t = new DispatcherTimer(TimeSpan.FromMilliseconds(SlideorGrowDuration.TimeSpan.TotalMilliseconds/2), DispatcherPriority.Normal, Tick, Dispatcher.CurrentDispatcher);

            translate.BeginAnimation(TranslateTransform.XProperty, da, HandoffBehavior.Compose);
        }

       

        /// <summary>
        /// Method is called when the slide out has been completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlideOutCompleted(object sender, EventArgs e)
        {
            // after the slide out
            //   1. re-enable hittesting
            //   2. initiate the delayed navigation
            //   3. invoke the slideIn animation at Loaded priority
            var clock = sender as AnimationClock;
            if (clock != null)
                clock.Completed -= SlideOutCompleted;


            if (contentPresenter != null)
            {
                IsHitTestVisible = contentPresenter.IsHitTestVisible = true;

                var nav = navArgs.Dequeue();
                navigationEvents.Add(nav.Uri);

                switch (nav.NavigationMode)
                {
                    case NavigationMode.New:
                        if (nav.Uri == null)
                        {
                            NavigationService.Navigate(nav.Content, nav.ExtraData);
                        }
                        else
                        {
                            NavigationService.Navigate(nav.Uri, nav.ExtraData);
                        }
                        break;

                    case NavigationMode.Back:
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        break;

                    case NavigationMode.Forward:
                        if (NavigationService.CanGoForward)
                            NavigationService.GoForward();
                        break;

                    case NavigationMode.Refresh:
                        NavigationService.Refresh();
                        break;
                }

                Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                    (ThreadStart)(() => ExecuteSecondTransition(nav)));
            }
        }

        /// <summary>
        /// The second transition moves the target frame into the view
        /// </summary>
        /// <param name="nav"></param>
        private void ExecuteSecondTransition(NavigatingCancelEventArgs nav)
        {
            double source = 0;
            EasingMode easingMode = EasingMode.EaseInOut;

            switch (nav.NavigationMode)
            {
                case NavigationMode.Forward:
                case NavigationMode.New:
                    source = ActualWidth;
                    easingMode = EasingMode.EaseOut;
                    break;
                case NavigationMode.Back:
                    source = -ActualWidth;
                    easingMode = EasingMode.EaseOut;
                    break;
            }

            var slider = contentPresenter;

            // Create a translation Transformation for the Sliding Content
            var translate = new TranslateTransform(source, 0);
            slider.RenderTransform = translate;

            // Create the animation
            var da = new DoubleAnimation(0.0d, SlideorGrowDuration);
            da.EasingFunction = new QuarticEase { EasingMode = easingMode };
            da.Completed += (s, ev) => IsNavigating = false;

            // Start the Animation
            translate.BeginAnimation(TranslateTransform.XProperty, da, HandoffBehavior.Compose);
        }
        #endregion

        #region GrowMode
        private void ExecuteGrowOut(NavigationMode mode)
        {
            double target = 0;
            var easingMode = EasingMode.EaseInOut;

            var slider = contentPresenter;
            // Create a translation Transformation for the Sliding Content
            var translate = new ScaleTransform(0, 0,ActualWidth/2,ActualHeight/2);
            slider.RenderTransform = translate;

            // Create the animation
            var da = new DoubleAnimation(1.0d, target, SlideorGrowDuration)
            {

                DecelerationRatio = 0.6,
                EasingFunction = new QuarticEase { EasingMode = easingMode }
            };
            var opda = new DoubleAnimation(0.0d, new Duration(TimeSpan.FromMilliseconds(SlideorGrowDuration.TimeSpan.TotalMilliseconds / 2)))
            {
               
            };
            // Start the Animation
            da.Completed += GrowOutCompleted;
            slider.BeginAnimation(OpacityProperty, opda, HandoffBehavior.Compose);
            translate.BeginAnimation(ScaleTransform.ScaleXProperty, da, HandoffBehavior.Compose);
            translate.BeginAnimation(ScaleTransform.ScaleYProperty, da, HandoffBehavior.Compose);
        }

        private void GrowOutCompleted(object sender, EventArgs e)
        {
             var clock = sender as AnimationClock;
            if (clock != null)
                clock.Completed -= GrowOutCompleted;


            if (contentPresenter != null)
            {
                IsHitTestVisible = contentPresenter.IsHitTestVisible = true;

                var nav = navArgs.Dequeue();
                navigationEvents.Add(nav.Uri);

                switch (nav.NavigationMode)
                {
                    case NavigationMode.New:
                        if (nav.Uri == null)
                        {
                            NavigationService.Navigate(nav.Content, nav.ExtraData);
                        }
                        else
                        {
                            NavigationService.Navigate(nav.Uri, nav.ExtraData);
                        }
                        break;

                    case NavigationMode.Back:
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        break;

                    case NavigationMode.Forward:
                        if (NavigationService.CanGoForward)
                            NavigationService.GoForward();
                        break;

                    case NavigationMode.Refresh:
                        NavigationService.Refresh();
                        break;
                }

                    Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                        (ThreadStart) (() => ExecuteGrowIn(nav)));

            }
        
        }

        private void ExecuteGrowIn(NavigatingCancelEventArgs nav)
        {
            double target = 1;
            var easingMode = EasingMode.EaseInOut;

            var slider = contentPresenter;
            // Create a translation Transformation for the Sliding Content
            var translate = new ScaleTransform(0, 0, ActualWidth / 2, ActualHeight / 2);
            slider.RenderTransform = translate;

            // Create the animation
            var da = new DoubleAnimation(0.0d, target, SlideorGrowDuration)
            {
                AccelerationRatio = 0.4,
                DecelerationRatio = 0.4,
                EasingFunction = new QuarticEase { EasingMode = easingMode }
            };
            var opda = new DoubleAnimation(1.0d, new Duration(TimeSpan.FromMilliseconds(SlideorGrowDuration.TimeSpan.TotalMilliseconds / 2)))
            {
                BeginTime = TimeSpan.FromMilliseconds(SlideorGrowDuration.TimeSpan.TotalMilliseconds / 2),
            };
            da.Completed += (s, ev) => IsNavigating = false;
            slider.BeginAnimation(OpacityProperty, opda, HandoffBehavior.Compose);
            translate.BeginAnimation(ScaleTransform.ScaleXProperty, da, HandoffBehavior.Compose);
            translate.BeginAnimation(ScaleTransform.ScaleYProperty, da, HandoffBehavior.Compose);
            
        }
        #endregion

        #endregion

        double target = 0;
        private  static object lockobject =new object();
        private Rectangle item;
        private Grid mainGrid;
        private ContentPresenter contentPresenter = null;
        private readonly HashSet<Uri> navigationEvents = new HashSet<Uri>();
        private readonly Queue<NavigatingCancelEventArgs> navArgs = new Queue<NavigatingCancelEventArgs>();
        
    }
}
