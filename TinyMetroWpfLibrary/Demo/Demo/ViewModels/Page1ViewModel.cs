using System.Windows.Input;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using Demo.NavigationEvents;

namespace Demo.ViewModels
{
    /// <summary>
    /// That's the ViewModel that belongs to the Example View
    /// </summary>
    public class Page1ViewModel : ViewModelBase
    {
        #region Overrides of ViewModelBase


        public override void Initialize()
        {
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
            GoPage2Command = RegisterCommand(ExecuteGoPage2Command, CanExecuteGoPage2Command, true);
        }


        public override void InitializePage(object extraData)
        {
        }

        #endregion

        #region GoBack Command


        public ICommand GoBackCommand
        {
            get { return GetPropertyValue(() => GoBackCommand); }
            set { SetPropertyValue(() => GoBackCommand, value); }
        }


        public void CanExecuteGoBackCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoBackCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoMainPageNavigationRequest());
        }

        #endregion

        #region GoBack Command


        public ICommand GoPage2Command
        {
            get { return GetPropertyValue(() => GoPage2Command); }
            set { SetPropertyValue(() => GoPage2Command, value); }
        }


        public void CanExecuteGoPage2Command(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoPage2Command(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoPage2NavigationRequest());
        }

        #endregion
    }
}
