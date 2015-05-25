using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows;
using System;
using System.Windows.Controls;

namespace TinyMetroWpfLibrary.Controls
{
    public partial class FanPanel : System.Windows.Controls.Panel
    {
        public FanPanel()
        {
            //this.Background = Brushes.Blue;                    // Good for debugging
            this.Background = Brushes.Transparent;            // Make sure we get mouse events

        }

        private Size ourSize;
        private bool foundNewChildren = false;
        private double scaleFactor = 1;

        public int AnimationMilliseconds
        {
            get { return (int)GetValue(AnimationMillisecondsProperty); }
            set { SetValue(AnimationMillisecondsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnimationMilliseconds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationMillisecondsProperty =
            DependencyProperty.Register("AnimationMilliseconds", typeof(int), typeof(FanPanel), new UIPropertyMetadata(1250));

        protected override Size MeasureOverride(Size availableSize)
        {
            // Allow children as much room as they want - then scale them
            Size size = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
            foreach (UIElement child in Children)
            {
                child.Measure(size);
            }

            // EID calls us with infinity, but framework doesn't like us to return infinity
            if (double.IsInfinity(availableSize.Height) || double.IsInfinity(availableSize.Width))
                return new Size(0, 0);
            else
                return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {

            if (this.Children == null || this.Children.Count == 0)
                return finalSize;

            ourSize = finalSize;
            foundNewChildren = false;

            foreach (UIElement child in this.Children)
            {
                // If this is the first time we've seen this child, add our transforms
                if (child.RenderTransform as TransformGroup == null)
                {
                    foundNewChildren = true;
                    child.RenderTransformOrigin = new Point(0, 0);
                    TransformGroup group = new TransformGroup();
                    child.RenderTransform = group;
                    //group.Children.Add(new ScaleTransform());
                    group.Children.Add(new TranslateTransform());
                    //group.Children.Add(new RotateTransform());
                }

                // Don't allow our children any clicks in icon form

                child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));

                // Scale the children so they fit in our size
                double sf = (Math.Min(ourSize.Width, ourSize.Height) * 0.4) / Math.Max(child.DesiredSize.Width, child.DesiredSize.Height);
                scaleFactor = Math.Min(scaleFactor, sf);
            }

            AnimateAll();

            return finalSize;
        }


        private void AnimateAll()
        {
            //System.Diagnostics.Debug.WriteLine("AnimateAll()");
            if (!IsOpen)
            {
                foreach (UIElement child in this.Children)
                {
                    if (foundNewChildren)
                        child.SetValue(Panel.ZIndexProperty, 0);
                    AnimateTo(child, 0, 0, 0);
                }
            }
            else
            {
                // On mouse over explode out the children and don't rotate them
                Random rand = new Random();
                double beginTime = 0;
                foreach (UIElement child in this.Children)
                {
                    //child.SetValue(Panel.ZIndexProperty, rand.Next(this.Children.Count));
                    double angle = FanPanel.GetAngle(child) * Math.PI / 180;
                    double x = PanelRadius * Math.Cos(angle);
                    double y = PanelRadius * Math.Sin(angle);
                    //double y = (rand.Next(16) - 8) * ourSize.Height / 32;
                    AnimateTo(child, x, y, beginTime);
                    //beginTime += BeginTimeIntervel;
                }
            }

        }

        private void AnimateTo(UIElement child, double x, double y, double beginTime)
        {
            TransformGroup group = (TransformGroup)child.RenderTransform;
            //ScaleTransform scale = (ScaleTransform)group.Children[0];
            TranslateTransform trans = (TranslateTransform)group.Children[0];
            //RotateTransform rot = (RotateTransform)group.Children[2];

            //rot.BeginAnimation(RotateTransform.AngleProperty, MakeAnimation(360,0));
            trans.BeginAnimation(TranslateTransform.XProperty, MakeAnimation(x, beginTime));
            trans.BeginAnimation(TranslateTransform.YProperty, MakeAnimation(y, beginTime));
            //scale.BeginAnimation(ScaleTransform.ScaleXProperty, MakeAnimation(1, 0));
            //scale.BeginAnimation(ScaleTransform.ScaleYProperty, MakeAnimation(1,0));
        }

        //private DoubleAnimation MakeAnimation(double to)
        //{
        //    return MakeAnimation(to, be);
        //}

        private DoubleAnimation MakeAnimation(double to, double beginTime = 0)
        {
            DoubleAnimation anim = new DoubleAnimation(to, TimeSpan.FromMilliseconds(AnimationMilliseconds));
            anim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            anim.BeginTime = TimeSpan.FromMilliseconds(beginTime);
            //if (endEvent != null)
            //  anim.Completed += endEvent;
            return anim;
        }

        //void anim_Completed(object sender, EventArgs e)
        //{
        //    RaiseEvent(new RoutedEventArgs(FanPanel.AnimationCompletedEvent, e));
        //}

        #region eric add new




        public double PanelRadius
        {
            get { return (double)GetValue(PanelRadiusProperty); }
            set { SetValue(PanelRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelRadiusProperty =
            DependencyProperty.Register("PanelRadius", typeof(double), typeof(FanPanel), new PropertyMetadata(0.0));





        public static double GetAngle(DependencyObject obj)
        {
            return (double)obj.GetValue(AngleProperty);
        }

        public static void SetAngle(DependencyObject obj, double value)
        {
            obj.SetValue(AngleProperty, value);
        }

        // Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.RegisterAttached("Angle", typeof(double), typeof(FanPanel), new PropertyMetadata(0.0));



        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(FanPanel), new PropertyMetadata(false, OnIsOpenChanged));

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FanPanel panel = d as FanPanel;
            if (panel != null)
            {
                panel.InvalidateMeasure();
            }
        }



        public double BeginTimeIntervel
        {
            get { return (double)GetValue(BeginTimeIntervelProperty); }
            set { SetValue(BeginTimeIntervelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BeginTimeIntervel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeginTimeIntervelProperty =
            DependencyProperty.Register("BeginTimeIntervel", typeof(double), typeof(FanPanel), new PropertyMetadata(100.0));

        
        #endregion
    }
}
