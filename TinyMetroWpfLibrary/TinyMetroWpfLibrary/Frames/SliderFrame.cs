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

namespace TinyMetroWpfLibrary.Frames
{
    /// <summary>
    /// The SliderFrame is based on the work of Dr.WPF 2007.
    /// I changed the animation to a Slide animation and enhanced it with some features that allows stacking page navigations. 
    /// The previous implementation failed when a navigation event is directly followed by a second one.
    /// 
    /// Original implementation can be found here <see cref="http://drwpf.com/blog/Portals/0/Code/FaderFrame.cs.txt"/>
    /// </summary>
    public class SliderFrame : Frame
    {
        #region slideDuration

        public static readonly DependencyProperty SlideDurationProperty =
            DependencyProperty.Register("SlideorGrowDuration", typeof(Duration), typeof(SliderFrame),
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

        public SliderFrame()
        {
            // watch for navigations
            Navigating += OnNavigating;
        }

        public override void OnApplyTemplate()
        {
            // get a reference to the frame's content presenter
            // this is the element we will slide in and out
            contentPresenter = GetTemplateChild("PART_FrameCP") as ContentPresenter;
            base.OnApplyTemplate();
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
            //da.Completed += (s, ev) => navigationEvents.Remove(nav.Uri);

            // Start the Animation
            translate.BeginAnimation(TranslateTransform.XProperty, da, HandoffBehavior.Compose);
        }

        
        protected void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (Content == null || contentPresenter == null || e.NavigationMode == NavigationMode.Refresh)
                return;

            // if we did not internally initiate the navigation:
            //   1. cancel the navigation,
            //   2. cache the target,
            //   3. disable hittesting during the slide, and
            //   4. slide out the current content
            if (!navigationEvents.Contains(e.Uri))
            {
                e.Cancel = true;
                navArgs.Enqueue(e);
                IsHitTestVisible = contentPresenter.IsHitTestVisible = false;

                ExecuteFirstTransition(e.NavigationMode);
            }
            else
                navigationEvents.Remove(e.Uri);
        }

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
                    (ThreadStart)(()=>ExecuteSecondTransition(nav)));
            }
        }

        private ContentPresenter contentPresenter = null;
        private readonly HashSet<Uri> navigationEvents = new HashSet<Uri>();
        private readonly Queue<NavigatingCancelEventArgs> navArgs = new Queue<NavigatingCancelEventArgs>();
    }
}
