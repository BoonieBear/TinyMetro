// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;

namespace BoonieBear.TinyMetro.WPF.Controls.Panels
{
    public class TouchableScrollViewer : ScrollViewer
    {
        private double offset = 0;

        public TouchableScrollViewer()
        {
            IsManipulationEnabled = true;
        }

        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            offset = e.VerticalOffset;
            base.OnScrollChanged(e);
        }

        protected override void OnManipulationStarting(System.Windows.Input.ManipulationStartingEventArgs e)
        {
            base.OnManipulationStarting(e);
        }

        protected override void OnManipulationDelta(System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            offset += e.DeltaManipulation.Translation.Y;
            ScrollToVerticalOffset(offset);
            base.OnManipulationDelta(e);
        }
    }
}
