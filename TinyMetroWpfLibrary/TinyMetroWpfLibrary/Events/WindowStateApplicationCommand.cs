// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;

namespace TinyMetroWpfLibrary.Events
{
    public class WindowStateApplicationCommand
    {
        /// <summary>
        /// Initializes a new instance of the WindowStateApplicationCommand
        /// </summary>
        /// <param name="state">the new Window State to set</param>
        public WindowStateApplicationCommand(WindowState state)
        {
            WindowState = state;
        }

        /// <summary>
        /// Gets the Window State to set
        /// </summary>
        public WindowState WindowState { get; private set; }
    }
}
