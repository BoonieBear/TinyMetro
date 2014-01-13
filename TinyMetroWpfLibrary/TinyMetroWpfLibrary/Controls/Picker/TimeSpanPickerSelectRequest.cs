// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace BoonieBear.TinyMetro.WPF.Controls.Picker
{
    public class TimeSpanPickerSelectRequest
    {
        /// <summary>
        /// Initializes a new instance of the TimeSpanPickerSelectRequest class
        /// </summary>
        /// <param name="value">selected date value to use</param>
        /// <param name="timeSpanPickerId">id of the corresponding listpicker</param>
        public TimeSpanPickerSelectRequest(TimeSpan value, Guid timeSpanPickerId)
        {
            Value = value;
            TimeSpanPickerId = timeSpanPickerId;
        }

        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        public TimeSpan Value { get; private set; }

        /// <summary>
        /// Gets the TimePicker Id
        /// </summary>
        public Guid TimeSpanPickerId { get; private set; }}
}
