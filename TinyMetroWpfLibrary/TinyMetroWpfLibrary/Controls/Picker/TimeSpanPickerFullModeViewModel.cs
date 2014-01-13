// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Windows.Input;
using BoonieBear.TinyMetro.WPF.Events;
using BoonieBear.TinyMetro.WPF.ViewModel;

namespace BoonieBear.TinyMetro.WPF.Controls.Picker
{
    public class TimeSpanPickerFullModeViewModel : ViewModelBase
    {
        #region Initialization Methods

        /// <summary>
        /// Initializes a new instance of the TimePickerFullModeViewModel
        /// </summary>
        public TimeSpanPickerFullModeViewModel()
        {
            AllHours = Enumerable.Range(0, 24).Select(v => new HourInfo(v)).ToArray();
            AllMinutes = Enumerable.Range(0, 60).Select(v => new MinuteInfo(v)).ToArray();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            CheckCommand = RegisterCommand(ExecuteCheckCommand, CanExecuteCheckCommand, true);
            CancelCommand = RegisterCommand(ExecuteCancelCommand, CanExecuteCancelCommand, true);
        }

        /// <summary>
        /// Initializes the page.
        /// This method will be called every time the user navigates to the page
        /// </summary>
        /// <param name="extraData">The extra Data, if there's any. Otherwise NULL</param>
        public override void InitializePage(object extraData)
        {
            var request = extraData as TimeSpanPickerFullModeRequest;
            if (request == null)
                return;

            // Reset Seconds / Milliseconds
            TimeSpan timeSpan = request.Value;
            timeSpan = timeSpan.Subtract(new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds));

            // Initialize Page
            FullModeHeader = request.FullModeHeader;

            SetPropertyValue(() => Value, timeSpan);
            SetPropertyValue(() => SelectedHour, AllHours[timeSpan.Hours]);
            SetPropertyValue(() => SelectedMinute, AllMinutes[timeSpan.Minutes]);

            TimeSpanPickerId = request.TimeSpanPickerId;
        }

        /// <summary>
        /// Gets the TimeSpanPickerId
        /// </summary>
        public Guid TimeSpanPickerId { get; private set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets all available hours
        /// </summary>
        public HourInfo[] AllHours { protected set; get; }

        /// <summary>
        /// Gets or sets all available months
        /// </summary>
        public MinuteInfo[] AllMinutes { protected set; get; }

        /// <summary>
        /// Gets or sets the selected date value
        /// </summary>
        public TimeSpan Value
        {
            get
            {
                return GetPropertyValue(() => Value);
            }
        }

        /// <summary>
        /// Gets or sets the selected hour
        /// </summary>
        public HourInfo SelectedHour
        {
            get { return GetPropertyValue(() => SelectedHour); }
            set
            {
                if (!SetPropertyValue(() => SelectedHour, value)) 
                    return;

                UpdateTimeValue();
            }
        }

        /// <summary>
        /// Gets or sets the selected minute
        /// </summary>
        public MinuteInfo SelectedMinute
        {
            get { return GetPropertyValue(() => SelectedMinute); }
            set
            {
                if (!SetPropertyValue(() => SelectedMinute, value)) 
                    return;

                UpdateTimeValue();
            }
        }

        /// <summary>
        /// Gets or sets the FullMode Header
        /// </summary>
        public string FullModeHeader
        {
            get { return GetPropertyValue(() => FullModeHeader); }
            set { SetPropertyValue(() => FullModeHeader, value); }
        }

        #endregion

        #region Save Command

        /// <summary>
        /// Gets or sets the Save command.
        /// </summary>
        /// <value>The Save command.</value>
        public ICommand CheckCommand
        {
            get { return GetPropertyValue(() => CheckCommand); }
            set { SetPropertyValue(() => CheckCommand, value); }
        }

        /// <summary>
        /// Determines whether this instance can execute Save command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Save command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public void CanExecuteCheckCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        /// <summary>
        /// Executes the Save command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public void ExecuteCheckCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new TimeSpanPickerSelectRequest(Value, TimeSpanPickerId));
        }

        #endregion

        #region Cancel Command

        /// <summary>
        /// Gets or sets the Cancel command.
        /// </summary>
        /// <value>The Cancel command.</value>
        public ICommand CancelCommand
        {
            get { return GetPropertyValue(() => CancelCommand); }
            set { SetPropertyValue(() => CancelCommand, value); }
        }

        /// <summary>
        /// Determines whether this instance can execute Cancel command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Cancel command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public void CanExecuteCancelCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        /// <summary>
        /// Executes the Cancel command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public void ExecuteCancelCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// This method updates the Time Value
        /// </summary>
        private void UpdateTimeValue()
        {
            if (SelectedHour == null || SelectedMinute == null)
                return;

            SetPropertyValue(() => Value, new TimeSpan(SelectedHour.Hour, SelectedMinute.Minute, Value.Seconds));
        }

        #endregion
    }
}
