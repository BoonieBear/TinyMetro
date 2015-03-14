// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace TinyMetroWpfLibrary.FrameControls.WindowedControl
{
    public partial class WindowedItemsControl
    {
        private Point originMousePosition;
        private Point lastMousePosition;
        private Timer leaveTimer;
        private double velocity;
        private int originIndex = -1;

        /// <summary>
        /// Called when the user presses the mouse button
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            originMousePosition = e.GetPosition(this);

            if (manipulationDeltaInProgress)
                FinishAnimation(originMousePosition);
            else
            {
                originIndex = SelectedIndex;
                manipulationInProgress = true;
                manipulationDeltaInProgress = false;
                velocity = 0;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (leaveTimer != null)
            {
                leaveTimer.Dispose();
            }
            leaveTimer = null;

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            var position = e.GetPosition(this);

            if ((leaveTimer == null) && !(manipulationDeltaInProgress && velocity != 0))
            {
                leaveTimer = new Timer(cb => 
                    Dispatcher.BeginInvoke(
                    (Action)(() =>
                        {
                            manipulationDeltaInProgress = true;
                            FinishAnimation(position);
                        }), null), null, 100, -1);
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Called when the user releases the mouse button
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (!manipulationInProgress)
                return;

            var position = e.GetPosition(this);

            // This might be negative or beyond bounds!
            if (manipulationDeltaInProgress && velocity != 0)
            {
                StartVelocityAnimation();
            }
            else
            {
                FinishAnimation(position);
            }

            base.OnMouseDown(e);
        }

        private void StartVelocityAnimation()
        {
            var moveTo = VerticalOffset + velocity;

            // Maybe we have to correct the moveTo value, if we don't have Wrappable StackPanel
            if (!isWrappableStackPanel)
            {
                if (FractionalCenteredIndexFromVerticalOffset(moveTo, false) < 0)
                {
                    var verticalOffsetIncrement = VerticalOffset - VerticalOffsetFromCenteredIndex(0);
                    var verticalOffsetExcess = velocity - verticalOffsetIncrement;
                    moveTo -= verticalOffsetExcess;
                }
                else if (FractionalCenteredIndexFromVerticalOffset(moveTo, false) > Items.Count - 1)
                {
                    var verticalOffsetIncrement = VerticalOffsetFromCenteredIndex(Items.Count - 1) - VerticalOffset;
                    var verticalOffsetExcess = velocity - verticalOffsetIncrement;
                    moveTo -= verticalOffsetExcess;
                }
            }
            velocity = moveTo - VerticalOffset;

            var moveInPlace = new DoubleAnimation(moveTo, new Duration(TimeSpan.FromMilliseconds(Math.Abs(velocity))))
                                  {
                                      EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                                      FillBehavior = FillBehavior.Stop
                                  };
            moveInPlace.Completed += (s, a) =>
                                         {
                                             var closestIndex = (int)Math.Round(FractionalCenteredIndexFromVerticalOffset(moveTo, false));
                                             CorrectDisplacement(closestIndex, 0);
                                         };
            BeginAnimation(VerticalOffsetProperty, moveInPlace, HandoffBehavior.SnapshotAndReplace);
        }

        /// <summary>
        /// Finish the current animation and corrects the displacement
        /// </summary>
        public void FinishAnimation()
        {
            FinishAnimation(lastMousePosition);
        }

        /// <summary>
        /// Finish the current animation and corrects the displacement if necessary
        /// </summary>
        /// <param name="position">current mouse position</param>
        private void FinishAnimation(Point position)
        {
            var tappedItemIndex = (int)Math.Floor((position.Y + VerticalOffset) / itemsHeight);
            var closestIndex = (int)Math.Round(FractionalCenteredIndexFromVerticalOffset(VerticalOffset, false));
            var indexToUse = manipulationDeltaInProgress ? closestIndex : tappedItemIndex;

            var destinationVerticalOffset = VerticalOffsetFromCenteredIndex(indexToUse);
            var displacement = VerticalOffset - destinationVerticalOffset;

            // Now start inertia to slide into place
            CorrectDisplacement(indexToUse, displacement);
        }

        /// <summary>
        /// Starts an displacement correcting animation if necessary
        /// </summary>
        /// <param name="indexToUse">Target Index to use</param>
        /// <param name="displacement">Calculated Displacement</param>
        private void CorrectDisplacement(int indexToUse, double displacement)
        {
            if (displacement != 0)
            {
                SelectedIndex = -1;
                var moveInPlace = new DoubleAnimation(VerticalOffset-displacement, new Duration(TimeSpan.FromMilliseconds(SLIDE_TIME)))
                                      {
                                          EasingFunction = new ExponentialEase { EasingMode = manipulationDeltaInProgress ? EasingMode.EaseOut : EasingMode.EaseIn },
                                          FillBehavior = FillBehavior.Stop
                                      };
                moveInPlace.Completed += (s, a) =>
                                             {
                                                 manipulationDeltaInProgress = manipulationInProgress = false;
                                                 int newIndex = NormalizeIndex(indexToUse);
                                                 SelectedIndex = newIndex;
                                                 velocity = 0;
                                             };
                BeginAnimation(VerticalOffsetProperty, moveInPlace, HandoffBehavior.SnapshotAndReplace);
            }
            else
            {
                manipulationDeltaInProgress = manipulationInProgress = false;
                int newIndex = NormalizeIndex(indexToUse);
                SelectedIndex = newIndex;
                velocity = 0;

                if (originIndex == SelectedIndex)
                {
                    IsActive ^= true;
                    originIndex = -1;
                }
            }
        }

        /// <summary>
        /// Called when the user uses the scroll wheel
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            manipulationInProgress = manipulationDeltaInProgress = true;
            IsActive = true;
            SelectedIndex = -1;

            velocity -= e.Delta/2.0;
            StartVelocityAnimation();
            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Called when the user press the mousebutton and moves the mouse
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !manipulationInProgress)
            {
                base.OnMouseMove(e);
                return;
            }

            // Store this, in order to evaluate the new index when releasing the mouse button
            IsActive = true;
            SelectedIndex = -1;
            manipulationDeltaInProgress = true;

            var position = lastMousePosition = e.GetPosition(this);
            var offset = originMousePosition.Y - position.Y;
            velocity = offset*50;
            originMousePosition = position;
 
            // For non-wrappable panel, check for end of the line
            if (!isWrappableStackPanel)
            {
                var newVerticalOffset = VerticalOffset + offset;

                if (FractionalCenteredIndexFromVerticalOffset(newVerticalOffset, false) < 0)
                {
                    SelectedIndex = 0;
                }
                else if (FractionalCenteredIndexFromVerticalOffset(newVerticalOffset, false) > Items.Count - 1)
                {
                    SelectedIndex = Items.Count - 1;
                }
                else
                    VerticalOffset += offset;
            }
            else
                VerticalOffset += offset;

            base.OnMouseMove(e);
        }
    }
}
