// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace TinyMetroWpfLibrary.Events
{
    public class CloseApplicationCommand
    {
        /// <summary>
        /// Initializes a new instance of the CloseApplicationCommand
        /// </summary>
        /// <param name="forceClosing"></param>
        public CloseApplicationCommand(bool forceClosing = false)
        {
            ForceClosing = forceClosing;
        }

        /// <summary>
        /// True, if the closing is forced.
        /// If this flag is true, no other component will be asked, if the application may be allowed to close
        /// </summary>
        public bool ForceClosing { get; private set; }
    }
}
