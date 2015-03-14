// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    /// <summary>
    /// This request will be send, if an selected item shall be used
    /// </summary>
    public class ListPickerSelectItemRequest
    {
        /// <summary>
        /// Initializes a new instance of the ListPickerSelectItemRequest class
        /// </summary>
        /// <param name="selectedItem">selected listpicker item</param>
        /// <param name="listPickerId">id of the corresponding listpicker</param>
        public ListPickerSelectItemRequest(object selectedItem, Guid listPickerId)
        {
            SelectedItem = selectedItem;
            ListPickerId = listPickerId;
        }

        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        public object SelectedItem { get; private set; }

        /// <summary>
        /// Gets the ListPicker Id
        /// </summary>
        public Guid ListPickerId { get; private set; }
    }
}
