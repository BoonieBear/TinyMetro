// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    public class DatePickerFullModeRequest
    {
        /// <summary>
        /// Initializes a new instance of the ListPickerFullModeRequest
        /// </summary>
        /// <param name="fullModeHeader"></param>
        /// <param name="value"></param>
        /// <param name="datePickerId"></param>
        public DatePickerFullModeRequest(string fullModeHeader, DateTime value, Guid datePickerId)
        {
            FullModeHeader = fullModeHeader;
            Value = value;
            DatePickerId = datePickerId;
        }

        /// <summary>
        /// Gets or sets the selected Date Value
        /// </summary>
        public DateTime Value { get; private set; }

        /// <summary>
        /// Gets or sets the FullMode Header
        /// </summary>
        public string FullModeHeader { get; private set; }

        /// <summary>
        /// Gets the DatePicker Id
        /// </summary>
        public Guid DatePickerId { get; private set; }
    }
}
