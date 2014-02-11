using System.Windows.Controls;
using Demo.ViewModels;

namespace Demo.Core
{

    public class TestViewModelLocator
    {
        private MainFrameViewModel _mainFrameViewModel;
        private MainPageViewModel _mainPageViewModel;
        private Page1ViewModel _page1ViewModel;
        private Page2ViewModel _page2ViewModel;
        /// <summary>
        /// Gets the MainFrame ViewModel
        /// </summary>
        public MainFrameViewModel MainFrameViewModel
        {
            get
            {
                // Creates the MainFrame ViewModel
                if (_mainFrameViewModel == null)
                {
                    _mainFrameViewModel = new MainFrameViewModel();
                    _mainFrameViewModel.Initialize();
                }
                return _mainFrameViewModel;
            }
        }

        /// <summary>
        /// Gets the Example ViewModel
        /// </summary>
        public MainPageViewModel MainPageViewModel
        {
            get
            {
                // Creates the Example ViewModel
                if (_mainPageViewModel == null)
                {
                    _mainPageViewModel = new MainPageViewModel();
                    _mainPageViewModel.Initialize();
                }
                return _mainPageViewModel;
            }
        }

        public Page1ViewModel Page1ViewModel
        {
            get
            {
                // Creates the Example ViewModel
                if (_page1ViewModel == null)
                {
                    _page1ViewModel = new Page1ViewModel();
                    _page1ViewModel.Initialize();
                }
                return _page1ViewModel;
            }
        }

        public Page2ViewModel Page2ViewModel
        {
            get
            {
                // Creates the Example ViewModel
                if (_page2ViewModel == null)
                {
                    _page2ViewModel = new Page2ViewModel();
                    _page2ViewModel.Initialize();
                }
                return _page2ViewModel;
            }
        }
    }
}
