// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace TinyMetroWpfLibrary.Events
{
    /// <summary>
    /// This event will be send prior to the application closing
    /// </summary>
    public class ApplicationIsClosingEvent
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Closing shall be canceled
        /// </summary>
        public bool Cancel { get; set; }
    }
}
