// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Navigation;

namespace BoonieBear.TinyMetro.WPF.Events
{
    public class NavigationEvent
    {
        /// <summary>
        /// Initializes a new instance of the Navigation Event
        /// </summary>
        /// <param name="e"></param>
        public NavigationEvent(NavigatingCancelEventArgs e)
        {
            Arguments = e;
        }

        /// <summary>
        /// Gets the Navigation Event Arguments
        /// </summary>
        public NavigatingCancelEventArgs Arguments { get; private set; }
    }
}
