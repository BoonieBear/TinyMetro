using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoonieBear.TinyMetro.WPF.Controls.Picker
{
    /// <summary>
    /// Item Filter used to filter list items
    /// </summary>
    public interface IItemFilter
    {
        /// <summary>
        /// Method that returns true, if the item matches the filter 
        /// </summary>
        /// <param name="filter">Filter value</param>
        /// <returns>true, if the value will be accepted</returns>
        bool IsValueAccepted(string filter);
    }
}
