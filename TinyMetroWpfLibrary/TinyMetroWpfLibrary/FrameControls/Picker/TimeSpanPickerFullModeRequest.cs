// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    public class TimeSpanPickerFullModeRequest
    {
        /// <summary>
        /// Initializes a new instance of the ListPickerFullModeRequest
        /// </summary>
        /// <param name="fullModeHeader"></param>
        /// <param name="value"></param>
        /// <param name="timeSpanPickerId"></param>
        public TimeSpanPickerFullModeRequest(string fullModeHeader, TimeSpan value, Guid timeSpanPickerId)
        {
            FullModeHeader = fullModeHeader;
            Value = value;
            TimeSpanPickerId = timeSpanPickerId;
        }

        /// <summary>
        /// Gets or sets the selected Date Value
        /// </summary>
        public TimeSpan Value { get; private set; }

        /// <summary>
        /// Gets or sets the FullMode Header
        /// </summary>
        public string FullModeHeader { get; private set; }

        /// <summary>
        /// Gets the TimePicker Id
        /// </summary>
        public Guid TimeSpanPickerId { get; private set; }    
    }
}
