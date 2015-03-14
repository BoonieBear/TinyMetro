// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    public class TimePickerFullModeViewModel : ViewModelBase
    {
        #region Initialization Methods

        /// <summary>
        /// Initializes a new instance of the TimePickerFullModeViewModel
        /// </summary>
        public TimePickerFullModeViewModel()
        {
            if (DateTimeFormatInfo.CurrentInfo != null)
                SetPropertyValue(() => IsDesignatorVisible,  !string.IsNullOrEmpty(DateTimeFormatInfo.CurrentInfo.AMDesignator));

            AllDesignators = new[] { new DesignatorInfo(0), new DesignatorInfo(1)};
            AllHours = InitializeHours();
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
            var request = extraData as TimePickerFullModeRequest;
            if (request == null)
                return;

            // Initialize Page
            FullModeHeader = request.FullModeHeader;

            SetPropertyValue(() => Value, request.Value);
            SetPropertyValue(() => SelectedHour, AllHours[request.Value.Hour]);
            SetPropertyValue(() => SelectedMinute, AllMinutes[request.Value.Minute]);
            SetPropertyValue(() => SelectedDesignator, AllDesignators[request.Value.Hour / 12]);

            TimePickerId = request.TimePickerId;
        }

        /// <summary>
        /// Gets the TimePickerId
        /// </summary>
        public Guid TimePickerId { get; private set; }

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
        /// Gets or sets all available designators
        /// </summary>
        public DesignatorInfo[] AllDesignators { protected set; get; }

        /// <summary>
        /// Gets or sets a value that indicates whether the designator is visible or not.
        /// </summary>
        public bool IsDesignatorVisible
        {
            get { return GetPropertyValue(() => IsDesignatorVisible); }
            set
            {
                if (SetPropertyValue(() => IsDesignatorVisible, value))
                    AllHours = InitializeHours();
            }
        }

        /// <summary>
        /// Gets or sets the selected date value
        /// </summary>
        public DateTime Value
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
        /// Gets or sets the selected year
        /// </summary>
        public DesignatorInfo SelectedDesignator
        {
            get { return GetPropertyValue(() => SelectedDesignator); }
            set
            {
                if (!SetPropertyValue(() => SelectedDesignator, value)) 
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
            EventAggregator.PublishMessage(new TimePickerSelectRequest(Value, TimePickerId));
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
        internal void UpdateTimeValue()
        {
            if (SelectedHour == null || SelectedMinute == null || SelectedDesignator == null)
                return;

            var currentHour = SelectedHour.Hour;
            if (IsDesignatorVisible)
                currentHour = (currentHour % 12) + (SelectedDesignator.DesignatorNumber == 1 ? 12 : 0);

            SetPropertyValue(() => Value, new DateTime(Value.Year, Value.Month, Value.Day, currentHour, SelectedMinute.Minute, Value.Second, Value.Kind));
        }

        /// <summary>
        /// Create the Hour Information
        /// </summary>
        /// <returns></returns>
        private HourInfo[] InitializeHours()
        {
            if (!IsDesignatorVisible)
                return Enumerable.Range(0, 24).Select(v => new HourInfo(v)).ToArray();

            return new []
                       {
                        new HourInfo(12),    
                        new HourInfo(1),    
                        new HourInfo(2),    
                        new HourInfo(3),    
                        new HourInfo(4),    
                        new HourInfo(5),    
                        new HourInfo(6),    
                        new HourInfo(7),    
                        new HourInfo(8),    
                        new HourInfo(9),    
                        new HourInfo(10),    
                        new HourInfo(11),    
                        new HourInfo(12),    
                        new HourInfo(1),    
                        new HourInfo(2),    
                        new HourInfo(3),    
                        new HourInfo(4),    
                        new HourInfo(5),    
                        new HourInfo(6),    
                        new HourInfo(7),    
                        new HourInfo(8),    
                        new HourInfo(9),    
                        new HourInfo(10),    
                        new HourInfo(11),    
                       };
        }

        #endregion
    }
}
