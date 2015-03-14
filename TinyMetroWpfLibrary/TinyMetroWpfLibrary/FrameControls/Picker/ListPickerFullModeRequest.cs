// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Windows;

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    /// <summary>
    /// This event is used to request the full mode of the listpicker
    /// </summary>
    public class ListPickerFullModeRequest
    {
        /// <summary>
        /// Initializes a new instance of the ListPickerFullModeRequest
        /// </summary>
        /// <param name="fullModeHeader"></param>
        /// <param name="itemsSource"></param>
        /// <param name="selectedItem"></param>
        /// <param name="fullModeItemTemplate"></param>
        /// <param name="listPickerId"></param>
        public ListPickerFullModeRequest(string fullModeHeader, IEnumerable itemsSource, object selectedItem, DataTemplate fullModeItemTemplate, Guid listPickerId)
        {
            FullModeHeader = fullModeHeader;
            ItemsSource = itemsSource;
            SelectedItem = selectedItem;
            FullModeItemTemplate = fullModeItemTemplate;
            ListPickerId = listPickerId;
        }

        /// <summary>
        /// Gets or sets the ItemsSource
        /// </summary>
        public IEnumerable ItemsSource { get; private set; }

        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        public object SelectedItem { get; private set; }

        /// <summary>
        /// Gets or sets the FullModeItemTemplate
        /// </summary>
        public DataTemplate FullModeItemTemplate { get; private set; }

        /// <summary>
        /// Gets or sets the FullMode Header
        /// </summary>
        public string FullModeHeader { get; private set; }

        /// <summary>
        /// Gets the ListPicker Id
        /// </summary>
        public Guid ListPickerId { get; private set; }
    }
}
