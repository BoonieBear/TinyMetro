// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Navigation;

namespace BoonieBear.TinyMetro.WPF.Events
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NavigationCompletedEvent<T>
    {
        public NavigationCompletedEvent(NavigationMode mode)
        {
            NavigationMode = mode;
        }

        /// <summary>
        /// Gets or sets the navigation mode.
        /// </summary>
        /// <value>The navigation mode.</value>
        public NavigationMode NavigationMode { get; private set; }
    }
}
