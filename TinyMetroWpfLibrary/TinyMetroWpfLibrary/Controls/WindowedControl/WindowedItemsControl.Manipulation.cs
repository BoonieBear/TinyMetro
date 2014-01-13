// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Input;

namespace BoonieBear.TinyMetro.WPF.Controls.WindowedControl
{
    public partial class WindowedItemsControl
    {
        const int SLIDE_TIME = 250;         // milliseconds
        const double DECELERATION = 0.001;  // units per msec squared

        bool manipulationInProgress;
        bool manipulationDeltaInProgress;
        bool tapInertiaInProgess;
        bool inertiaToUnknownIndex;
        bool reverseInertiaChecked;
        int inertiaDirection;

        protected override void OnManipulationStarting(ManipulationStartingEventArgs args)
        {
            args.ManipulationContainer = this;
            args.Mode = ManipulationModes.TranslateY;

            // Initialize all fields
            manipulationInProgress = true;
            manipulationDeltaInProgress = false;
            tapInertiaInProgess = false;
            inertiaToUnknownIndex = false;
            reverseInertiaChecked = false;
            inertiaDirection = 1;

            args.Handled = true;
            base.OnManipulationStarting(args);
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs args)
        {
            if (!manipulationDeltaInProgress)
            {
                IsActive = true;

                if (!tapInertiaInProgess)
                    SelectedIndex = -1;
            }

            manipulationDeltaInProgress = true;

            // This is the fake inertia from a tap
            if (tapInertiaInProgess)
            {
                VerticalOffset -= args.DeltaManipulation.Translation.Y;
            }

            // All other direct manipulation and inertia
            else
            {
                // For non-wrappable panel, check for end of the line
                if (!isWrappableStackPanel)
                {
                    double newVerticalOffset = VerticalOffset - inertiaDirection * args.DeltaManipulation.Translation.Y;

                    if (FractionalCenteredIndexFromVerticalOffset(newVerticalOffset, false) < 0)
                    {
                        double verticalOffsetIncrement = VerticalOffset - VerticalOffsetFromCenteredIndex(0);
                        double verticalOffsetExcess = args.DeltaManipulation.Translation.Y - verticalOffsetIncrement;
                        VerticalOffset -= verticalOffsetIncrement;
                        SelectedIndex = 0;
                        args.ReportBoundaryFeedback(new ManipulationDelta(new Vector(0, verticalOffsetExcess), 0, new Vector(), new Vector()));
                        args.Complete();
                    }
                    else if (FractionalCenteredIndexFromVerticalOffset(newVerticalOffset, false) > Items.Count - 1)
                    {
                        double verticalOffsetIncrement = VerticalOffsetFromCenteredIndex(Items.Count - 1) - VerticalOffset;
                        double verticalOffsetExcess = args.DeltaManipulation.Translation.Y - verticalOffsetIncrement;
                        VerticalOffset += verticalOffsetIncrement;
                        SelectedIndex = Items.Count - 1;
                        args.ReportBoundaryFeedback(new ManipulationDelta(new Vector(0, verticalOffsetExcess), 0, new Vector(), new Vector()));
                        args.Complete();
                    }
                }

                // Here's where scrolling might reverse itself
                if (args.IsInertial && inertiaToUnknownIndex && !reverseInertiaChecked)
                    CheckForBackupManeuver(VerticalOffset, args.DeltaManipulation.Translation.Y, args.Velocities.LinearVelocity.Y);

                // This is the normal direct manipulation and inertia
                VerticalOffset -= inertiaDirection * args.DeltaManipulation.Translation.Y;
            }

            base.OnManipulationDelta(args);
        }


        void CheckForBackupManeuver(double verticalOffset, double translation, double velocityParam)
        {
            double newVerticalOffset = verticalOffset - inertiaDirection * translation;
            double stopDistance = velocityParam * velocityParam / (2 * DECELERATION);
            double index = FractionalCenteredIndexFromVerticalOffset(verticalOffset, false);
            int prevIndex, nextIndex;

            if (inertiaDirection * translation > 0)
            {
                prevIndex = (int)Math.Ceiling(index);
                nextIndex = (int)Math.Floor(index);
            }
            else
            {
                prevIndex = (int)Math.Floor(index);
                nextIndex = (int)Math.Ceiling(index);
            }

            double prevVerticalOffset = VerticalOffsetFromCenteredIndex(prevIndex);
            double nextVerticalOffset = VerticalOffsetFromCenteredIndex(nextIndex);

            if (stopDistance < Math.Abs(newVerticalOffset - nextVerticalOffset) &&
                stopDistance < Math.Abs(newVerticalOffset - prevVerticalOffset))
            {
                reverseInertiaChecked = true;

                if (Math.Abs(newVerticalOffset - prevVerticalOffset) < Math.Abs(newVerticalOffset - nextVerticalOffset))
                {
                    inertiaDirection = -inertiaDirection;
                    SelectedIndex = NormalizeIndex(prevIndex);
                }
                else
                {
                    SelectedIndex = NormalizeIndex(nextIndex);
                }
            }
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs args)
        {
            // Check to see if there have been any deltas; if not, it's a tap
            if (!manipulationDeltaInProgress)
            {
                // This might be negative or beyond bounds!
                var tappedItemIndex = (int)Math.Floor((args.ManipulationOrigin.Y + VerticalOffset) / itemsHeight);
                var destinationVerticalOffset = VerticalOffsetFromCenteredIndex(tappedItemIndex);
                var displacement = VerticalOffset - destinationVerticalOffset;

                // Set SelectedIndex
                SelectedIndex = NormalizeIndex(tappedItemIndex);

                // Now start inertia to slide into place
                if (displacement != 0)
                {
                    double localVelocity = 2 * displacement / SLIDE_TIME;
                    args.TranslationBehavior.DesiredDisplacement = Math.Abs(displacement);
                    args.TranslationBehavior.InitialVelocity = new Vector(0, localVelocity);
                    tapInertiaInProgess = true;
                }
            }
            // Not a tap -- there has been some movement and now the finger has lifted
            else
            {
                // Check for insufficient velocity
                var closestIndex = (int)Math.Round(FractionalCenteredIndexFromVerticalOffset(VerticalOffset, false));
                var verticalOffsetOfClosestIndex = VerticalOffsetFromCenteredIndex(closestIndex);
                var displacement = VerticalOffset - verticalOffsetOfClosestIndex;
                var localVelocity = 2 * displacement / SLIDE_TIME;

                // Band has been slid precisely into place
                if (displacement == 0)
                {
                    SelectedIndex = NormalizeIndex(closestIndex);
                }
                // Insufficient velocity to move beyond "destinationIndex"
                else if (Math.Abs(args.TranslationBehavior.InitialVelocity.Y) < Math.Abs(localVelocity))
                {
                    SelectedIndex = NormalizeIndex(closestIndex);
                    args.TranslationBehavior.DesiredDisplacement = Math.Abs(displacement);
                    args.TranslationBehavior.InitialVelocity = new Vector(0, localVelocity);
                }
                // Enough velocity for inertia to some unknown index
                else
                {
                    inertiaToUnknownIndex = true;
                    args.TranslationBehavior.DesiredDeceleration = DECELERATION;
                }
            }

            args.Handled = true;
            base.OnManipulationInertiaStarting(args);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs args)
        {
            if (!manipulationDeltaInProgress)
                IsActive ^= true;

            manipulationInProgress = false;
            SetVerticalOffsetFromSelectedIndex();

            args.Handled = true;
            base.OnManipulationCompleted(args);
        }
    }
}
