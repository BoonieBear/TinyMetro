// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace BoonieBear.TinyMetro.WPF.Frames
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
            DependencyProperty.Register("SlideDuration", typeof(Duration), typeof(AnimationFrame),
                new FrameworkPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(300))));

        /// <summary>
        /// slideDuration will be used as the duration for slide Out and slide In animations
        /// </summary>
        public Duration SlideDuration
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

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
                var da = new DoubleAnimation(1.0d, FadeDuration) { AccelerationRatio = 1.0d };
                da.Completed += (s, ev) => IsNavigating = false;
                contentPresenter.BeginAnimation(OpacityProperty, da, HandoffBehavior.Compose);
            }
        }

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
            var da = new DoubleAnimation(0.0d, target, SlideDuration)
                         {
                             EasingFunction = new QuarticEase { EasingMode = easingMode }
                         };

            // Start the Animation
            da.Completed += SlideOutCompleted;
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
            var da = new DoubleAnimation(0.0d, SlideDuration);
            da.EasingFunction = new QuarticEase { EasingMode = easingMode };
            da.Completed += (s, ev) => IsNavigating = false;

            // Start the Animation
            translate.BeginAnimation(TranslateTransform.XProperty, da, HandoffBehavior.Compose);
        }

        #endregion

        private ContentPresenter contentPresenter = null;
        private readonly HashSet<Uri> navigationEvents = new HashSet<Uri>();
        private readonly Queue<NavigatingCancelEventArgs> navArgs = new Queue<NavigatingCancelEventArgs>();
    }
}
