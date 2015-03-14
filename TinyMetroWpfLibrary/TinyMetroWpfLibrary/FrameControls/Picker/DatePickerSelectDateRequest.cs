// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    /// <summary>
    /// This request will be send, if an selected date shall be used
    /// </summary>    
    public class DatePickerSelectDateRequest
    {
        /// <summary>
        /// Initializes a new instance of the ListPickerSelectItemRequest class
        /// </summary>
        /// <param name="value">selected date value to use</param>
        /// <param name="datePickerId">id of the corresponding listpicker</param>
        public DatePickerSelectDateRequest(DateTime value, Guid datePickerId)
        {
            Value = value;
            DatePickerId = datePickerId;
        }

        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        public DateTime Value { get; private set; }

        /// <summary>
        /// Gets the DatePickerId Id
        /// </summary>
        public Guid DatePickerId { get; private set; }
    }
}
