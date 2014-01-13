using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace BoonieBear.TinyMetro.WPF.Controller
{
    public interface IKernel
    {
        /// <summary>
        /// Gets the event aggregator.
        /// </summary>
        /// <returns></returns>
        IEventAggregator EventAggregator { get; }

        /// <summary>
        /// Gets the current used controller
        /// </summary>
        /// <returns></returns>
        INavigationController Controller { get; }
    }
}