using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace TinyMetroWpfLibrary.Controller
{
    /// <summary>
    /// Interface for the Base Controller
    /// </summary>
    public interface INavigationController
    {
        /// <summary>
        /// Used to initialize the Navigation Controller
        /// </summary>
        void Init();

        /// <summary>
        /// Initializes the Navigation Controller
        /// </summary>
        void SetRootFrame(Frame contentFrame);

        /// <summary>
        /// Navigates to page.
        /// </summary>
        /// <param name="navigateToPage">The navigate to page.</param>
        void NavigateToPage(string navigateToPage);

        /// <summary>
        /// Navigates to a page and add additional data
        /// </summary>
        /// <param name="navigateToPage">The navigate to page.</param>
        /// <param name="message">The message that will be send to the page</param>
        void NavigateToPage(string navigateToPage, object message);

        /// <summary>
        /// Determines whether this instance can navigate back.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can navigate back; otherwise <c>false</c>.
        /// </returns>
        bool CanGoBack { get; }

        /// <summary>
        /// Gets a value indicating whether the closing of the current window shall be forced.
        /// If that value is true, than the client code can't stop closing by returning false with CanClose.
        /// It's therefore a override of the CanClose result.
        /// </summary>
        bool ForceClosing { get; }
    }
}
