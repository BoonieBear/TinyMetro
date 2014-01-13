// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Drawing;

namespace BoonieBear.TinyMetro.WPF.Docking
{
    public class OnTaskBarMovedEventHandlerArgs
    {
        public TaskBar.TaskBarEdge CurrentEdge { get; set; }
        public Point CurrentPosition { get; set; }

        public OnTaskBarMovedEventHandlerArgs(TaskBar.TaskBarEdge currentEdge, Point currentPosition)
        {
            CurrentEdge = currentEdge;
            CurrentPosition = currentPosition;
        }
    }
}