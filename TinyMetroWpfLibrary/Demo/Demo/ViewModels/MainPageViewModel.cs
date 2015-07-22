using System.Windows.Input;
using Demo.NavigationEvents;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using System.Collections.Generic;

namespace Demo.ViewModels
{

    public class MainPageViewModel : ViewModelBase
    {
        #region Overrides of ViewModelBase


        public override void Initialize()
        {
            GoPage1Command = RegisterCommand(ExecuteGoPage1Command, CanExecuteGoPage1Command, true);
            GoBackCommand = RegisterCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand, true);
        
        }


        public override void InitializePage(object extraData)
        {
            Error = "Some error!";
            IsLoading = true;
            ExecuteAsync(() => IsLoading = false, 2000);
        }

        #endregion
        public List<string> ID
        {
            get
            {
                List<string> id = new List<string>();
                for (int i = 1; i < 64; i++)
                {
                    id.Add(i.ToString());
                }
                return id;
            }
        }
                
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
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        #endregion

        #region GoPage1 Command


        public ICommand GoPage1Command
        {
            get { return GetPropertyValue(() => GoPage1Command); }
            set { SetPropertyValue(() => GoPage1Command, value); }
        }


        public void CanExecuteGoPage1Command(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        public void ExecuteGoPage1Command(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoPage1NavigationRequest());
        }

        #endregion
    }
}