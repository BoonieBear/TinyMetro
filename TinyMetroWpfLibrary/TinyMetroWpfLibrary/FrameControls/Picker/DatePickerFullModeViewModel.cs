// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Input;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    public class DatePickerFullModeViewModel : ViewModelBase
    {
        const int MINYEAR = 1900;
        const int MAXYEAR = 2200;

        #region Initialization Methods

        /// <summary>
        /// Static Initializer
        /// </summary>
        static DatePickerFullModeViewModel()
        {
            AllYears = new YearInfo[MAXYEAR - MINYEAR + 1];
            for (var year = 0; year < AllYears.Length; year++)
            {
                AllYears[year] = new YearInfo
                {
                    YearNumber = year + MINYEAR
                };
            }

            AllMonths = new MonthInfo[12];
            for (var month = 0; month < 12; month++)
                AllMonths[month] = new MonthInfo
                {
                    MonthNumber = month + 1
                };
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
            var request = extraData as DatePickerFullModeRequest;
            if (request == null)
                return;

            // Initialize Page
            FullModeHeader = request.FullModeHeader;

            MakeNewDays(request.Value.Year, request.Value.Month);

            SetPropertyValue(() => Value, request.Value);
            SetPropertyValue(() => SelectedYear, AllYears[request.Value.Year - MINYEAR]);
            SetPropertyValue(() => SelectedMonth, AllMonths[request.Value.Month - 1]);
            SetPropertyValue(() => SelectedDay, AllDays[request.Value.Day - 1]);

            Value = request.Value;
            DatePickerId = request.DatePickerId;
        }

        /// <summary>
        /// Gets the ListPickerId
        /// </summary>
        public Guid DatePickerId { get; private set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets all available years
        /// </summary>
        public static YearInfo[] AllYears { protected set; get; }

        /// <summary>
        /// Gets or sets all available months
        /// </summary>
        public static MonthInfo[] AllMonths { protected set; get; }

        /// <summary>
        /// Gets or sets all available days
        /// </summary>
        public DayInfo[] AllDays { protected set; get; }

        /// <summary>
        /// Gets or sets the selected date value
        /// </summary>
        public DateTime Value
        {
            set
            {
                if (SelectedYear == null || SelectedMonth == null || SelectedDay == null ||
                    value.Year != SelectedYear.YearNumber || value.Month != SelectedMonth.MonthNumber || value.Day != SelectedDay.DayNumber)
                {
                    SelectedYear = AllYears[value.Year - MINYEAR];
                    SelectedMonth = AllMonths[value.Month - 1];
                    SelectedDay = AllDays[value.Day - 1];
                }

                SetPropertyValue(() => Value, new DateTime(SelectedYear.YearNumber, SelectedMonth.MonthNumber, SelectedDay.DayNumber, value.Hour, value.Minute, value.Second, value.Kind));
            }

            get
            {
                return GetPropertyValue(() => Value);
            }
        }

        /// <summary>
        /// Gets or sets the selected day
        /// </summary>
        public DayInfo SelectedDay
        {
            get { return GetPropertyValue(() => SelectedDay); }
            set
            {
                if (!SetPropertyValue(() => SelectedDay, value)) 
                    return;
                
                if (SelectedYear != null && SelectedMonth != null && SelectedDay != null)
                    Value = new DateTime(SelectedYear.YearNumber, SelectedMonth.MonthNumber, SelectedDay.DayNumber, Value.Hour, Value.Minute, Value.Second, Value.Kind);
            }
        }

        /// <summary>
        /// Gets or sets the selected month
        /// </summary>
        public MonthInfo SelectedMonth
        {
            get { return GetPropertyValue(() => SelectedMonth); }
            set
            {
                if (!SetPropertyValue(() => SelectedMonth, value)) 
                    return;

                if (SelectedYear != null && SelectedMonth != null)
                {
                    MakeNewDays(SelectedYear.YearNumber, SelectedMonth.MonthNumber);
                        
                    if (SelectedDay != null)
                        Value = new DateTime(SelectedYear.YearNumber, SelectedMonth.MonthNumber, SelectedDay.DayNumber, Value.Hour, Value.Minute, Value.Second, Value.Kind);
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected year
        /// </summary>
        public YearInfo SelectedYear
        {
            get { return GetPropertyValue(() => SelectedYear); }
            set
            {
                if (!SetPropertyValue(() => SelectedYear, value)) 
                    return;

                if (SelectedYear != null && SelectedMonth != null)
                {
                    MakeNewDays(SelectedYear.YearNumber, SelectedMonth.MonthNumber);
                        
                    if (SelectedDay != null)
                        Value = new DateTime(SelectedYear.YearNumber, SelectedMonth.MonthNumber, SelectedDay.DayNumber, Value.Hour, Value.Minute, Value.Second, Value.Kind);
                }
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
            EventAggregator.PublishMessage(new DatePickerSelectDateRequest(Value, DatePickerId));
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
        /// Helper method that creates a set of new days
        /// </summary>
        /// <param name="year">Selected year</param>
        /// <param name="month">Selected Month</param>
        void MakeNewDays(int year, int month)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            AllDays = new DayInfo[daysInMonth];

            for (int day = 0; day < daysInMonth; day++)
                AllDays[day] = new DayInfo
                {
                    Year = year,
                    Month = month,
                    DayNumber = day + 1
                };

            OnPropertyChanged(()=>AllDays);

            if (SelectedDay != null)
                SelectedDay = AllDays[Math.Min(SelectedDay.DayNumber, AllDays.Length) - 1];
        }

        #endregion
    }
}
