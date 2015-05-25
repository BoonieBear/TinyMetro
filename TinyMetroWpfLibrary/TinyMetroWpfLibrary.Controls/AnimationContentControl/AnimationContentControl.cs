using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace TinyMetroWpfLibrary.Controls
{
    public class AnimationContentControl : ContentControl
    {
        static AnimationContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimationContentControl), new FrameworkPropertyMetadata(typeof(AnimationContentControl)));
        }
        public AnimationContentControl()
        {
            this.Loaded += (s, e) => { BeginAnimation(); };
        }

        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(AnimationContentControl), new PropertyMetadata(PlacementMode.Right));

        public double OffSet
        {
            get { return (double)GetValue(OffSetProperty); }
            set { SetValue(OffSetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffSet.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffSetProperty =
            DependencyProperty.Register("OffSet", typeof(double), typeof(AnimationContentControl), new PropertyMetadata(10.0));

        public UIElement TargetPopup
        {
            get { return (UIElement)GetValue(TargetPopupProperty); }
            set { SetValue(TargetPopupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetPopup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetPopupProperty =
            DependencyProperty.Register("TargetPopup", typeof(UIElement), typeof(AnimationContentControl), new PropertyMetadata(null));


        Border _contentBorder = new Border();
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _contentBorder = GetTemplateChild("ContentBorder") as Border;

        }

        private void BeginAnimation()
        {
            TranslateTransform animatedTranslateTransform = new TranslateTransform();
            this.RenderTransform = animatedTranslateTransform;
            if (this.FindName("AnimatedTranslateTransform") != null)
            {
                this.UnregisterName("AnimatedTranslateTransform");
            }
            this.RegisterName("AnimatedTranslateTransform", animatedTranslateTransform);

            DoubleAnimation translationAnimation = new DoubleAnimation();
            translationAnimation.From = GetAnimationFrom();
            translationAnimation.To = 0;
            translationAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
            translationAnimation.EasingFunction = new PowerEase()
            {
                Power = 3,
                EasingMode = EasingMode.EaseOut
            };

            Storyboard.SetTargetName(translationAnimation, "AnimatedTranslateTransform");
            Storyboard.SetTargetProperty(translationAnimation, GetAnimationProperty());
            Storyboard translationStoryboard = new Storyboard();
            translationStoryboard.Children.Add(translationAnimation);
            translationStoryboard.Begin(this);
        }

        private double GetAnimationFrom()
        {
            double ret = 0.0;
            switch (Placement)
            {
                case PlacementMode.Bottom:
                    ret = OffSet;
                    break;
                case PlacementMode.Left:
                    ret = -OffSet;
                    break;
                case PlacementMode.Right:
                    ret = OffSet;
                    break;
                case PlacementMode.Top:
                    ret = -OffSet;
                    break;
                default:
                    ret = 0;
                    break;
            }
            return ret;
        }

        private PropertyPath GetAnimationProperty()
        {
            return (Placement == PlacementMode.Right || Placement == PlacementMode.Left) ?
                new PropertyPath(TranslateTransform.XProperty)
                : new PropertyPath(TranslateTransform.YProperty);
        }
    }
}
