using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TinyMetroWpfLibrary.Controls.Utils;

namespace TinyMetroWpfLibrary.Controls.AnimationSlide
{
    public class AnimationSlide : ContentControl
    {
        static AnimationSlide()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimationSlide), new FrameworkPropertyMetadata(typeof(AnimationSlide)));
        }

        #region propertys
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(AnimationSlide), new PropertyMetadata(OnIsOpenPropertyChanged));



        public bool IsAutoClose
        {
            get { return (bool)GetValue(IsAutoCloseProperty); }
            set { SetValue(IsAutoCloseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAutoClose.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAutoCloseProperty =
            DependencyProperty.Register("IsAutoClose", typeof(bool), typeof(AnimationSlide), new PropertyMetadata(true));

        
        #endregion

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimationSlide control = d as AnimationSlide;
            if (control != null && e.NewValue is bool)
            {
                bool isOpen = (bool)e.NewValue;
                control.OnIsOpenChanged(isOpen);
            }
        }

        public void OnIsOpenChanged(bool isOpen)
        {
            if (double.IsNaN(this.Width))
            {
                return;
            }

            TranslateTransform animatedTranslateTransform = new TranslateTransform();
            this.RenderTransform = animatedTranslateTransform;
            if (this.FindName("AnimatedTranslateTransform") != null)
            {
                this.UnregisterName("AnimatedTranslateTransform");
            }
            this.RegisterName("AnimatedTranslateTransform", animatedTranslateTransform);
            
            DoubleAnimation translationAnimation = new DoubleAnimation();
            translationAnimation.From = isOpen ? this.Width : 0;
            translationAnimation.To = isOpen ? 0 : this.Width;
            translationAnimation.Duration = new Duration(new TimeSpan(0,0,0,0,300));
            translationAnimation.EasingFunction = new PowerEase() 
            {
                Power = 3,
                EasingMode = isOpen ? EasingMode.EaseOut : EasingMode.EaseIn
            };

            Storyboard.SetTargetName(translationAnimation, "AnimatedTranslateTransform");
            Storyboard.SetTargetProperty(translationAnimation, new PropertyPath(TranslateTransform.XProperty));
            Storyboard translationStoryboard = new Storyboard();
            translationStoryboard.Children.Add(translationAnimation);
            translationStoryboard.Begin(this);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (double.IsNaN(this.Width))
            {
                return;
            }

            TranslateTransform transform = new TranslateTransform();
            transform.X = this.Width;
            this.RenderTransform = transform;
            Window window = TreeHelper.TryFindParent<Window>(this);
            if (window != null)
            {
                window.PreviewMouseDown += OnWindowPreviewMouseDown;
            }
        }

        private void OnWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsMouseOver && this.IsOpen && IsAutoClose)
            {
                this.IsOpen = false;
                e.Handled = true;
            }
        }

    }

}