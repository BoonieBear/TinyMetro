// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace BoonieBear.TinyMetro.WPF.Events
{
    /// <summary>
    /// This event will force the control to update the bindings.
    /// That's especially useful when the last textbox does not update it's binding when the user clicks the ApplicationBar
    /// as discussed in <see cref="http://forums.create.msdn.com/forums/p/76635/465673.aspx"/>
    /// </summary>
    public class ForceBindingUpdateEvent
    {
    }
}
