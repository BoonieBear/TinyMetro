// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace TinyMetroWpfLibrary.Metro
{
    /// <summary>
    /// The Resize Flags defines the direction where the window can be sized
    /// </summary>
    [Flags]
    public enum ResizeDirectionFlags
    {
        /// <summary> Not Resizeable </summary>
        None = 0,

        /// <summary> Sizeable to the North </summary>
        SizeN = 1,
        
        /// <summary> Sizeable to the South </summary>
        SizeS = 2,

        /// <summary> Sizeable to the West </summary>
        SizeW = 4,
        
        /// <summary> Sizeable to the East </summary>
        SizeE = 8,
        
        /// <summary> Sizeable to the South/East </summary>
        SizeSE = 16,

        /// <summary> Sizeable to the North/West </summary>
        SizeNW = 32,

        /// <summary> Sizeable to the South/West </summary>
        SizeSW = 64,

        /// <summary> Sizeable to the North/East </summary>
        SizeNE = 128,
    }
}
