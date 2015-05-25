using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace TinyMetroWpfLibrary.Controls
{
    public  class MetroZoomableCanvas : ZoomableCanvas
    {
        #region const
        private const int MIN_SCALE = 1;
        private const int MAX_SCALE = 1000;
        private const int ScaleAnimationRelativeDuration = 400;
        private const double MinScaleRelativeToMinSize = 0.8;
        #endregion

        public MetroZoomableCanvas()
        {
            MouseTouchDevice.RegisterEvents(this);
            this.IsManipulationEnabled = true;
        }
        private IScrollInfo ScrollInfo
        {
            get
            {
                return this as IScrollInfo;
            }
        }

        private Point GetValideOffset(Point offset)
        {
            double scrollableHeight = ScrollInfo.ExtentHeight - ScrollInfo.ViewportHeight;
            double scrollableWidth = ScrollInfo.ExtentWidth - ScrollInfo.ViewportWidth;
            offset.X = Math.Min(Math.Max(0, offset.X), scrollableWidth);
            offset.Y = Math.Min(Math.Max(0, offset.Y), scrollableHeight);
            return offset;
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            var x = Math.Pow(2, e.Delta / 3.0 / Mouse.MouseWheelDeltaForOneLine);
            ScaleCanvas(x, e.GetPosition(this));
            e.Handled = true;
        }

        private double MaxScaleRelativeToMaxSize;
        private double _originalScale;
        private void ScaleCanvas(double relativeScale, Point center, bool animate = false)
        {
            var scale = this.Scale;

            if (scale <= 0) return;

            // minimum size = 80% of size where the whole image is visible
            // maximum size = Max(120% of full resolution of the image, 120% of original scale)

            MaxScaleRelativeToMaxSize = 1.2;
            relativeScale = relativeScale.Clamp(
                MinScaleRelativeToMinSize * _originalScale / scale,
                Math.Max(MaxScaleRelativeToMaxSize, MaxScaleRelativeToMaxSize * _originalScale) / scale);

            var targetScale = scale * relativeScale;

            //var newLevel = Source.GetLevel(targetScale);
            //var level = _spatialSource.CurrentLevel;
            //if (newLevel != level)
            //{
            //    // If it's zooming in, throttle
            //    if (newLevel > level)
            //        ThrottleChangeLevel(newLevel);
            //    else
            //        _spatialSource.CurrentLevel = newLevel;
            //}

            if (targetScale != scale)
            {
                var position = (Vector)center;
                var targetOffset = (Point)((Vector)(this.Offset + position) * relativeScale - position);

                if (animate)
                {
                    if (relativeScale < 1)
                        relativeScale = 1 / relativeScale;
                    var duration = TimeSpan.FromMilliseconds(relativeScale * ScaleAnimationRelativeDuration);
                    var easing = new CubicEase();
                    this.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(targetScale, duration) { EasingFunction = easing }, HandoffBehavior.Compose);
                    this.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(targetOffset, duration) { EasingFunction = easing }, HandoffBehavior.Compose);
                }
                else
                {
                    this.Scale = targetScale;
                    this.Offset = targetOffset;
                }
            }
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);

            //var oldScale = this.Scale;
            //this.ApplyAnimationClock(ZoomableCanvas.ScaleProperty, null);
            //this.Scale = oldScale;

            //var oldOffset = this.Offset;
            //this.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            //this.Offset = oldOffset;

            var scale = e.DeltaManipulation.Scale.X;
            ScaleCanvas(scale, e.ManipulationOrigin);

            //this.Offset -= e.DeltaManipulation.Translation;
            e.Handled = true;
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
            base.OnManipulationInertiaStarting(e);
            e.TranslationBehavior = new InertiaTranslationBehavior { DesiredDeceleration = 0.0096 };
            e.ExpansionBehavior = new InertiaExpansionBehavior { DesiredDeceleration = 0.000096 };
            e.Handled = true;
        }
    }
}
