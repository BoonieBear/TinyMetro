// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TinyMetroWpfLibrary.Controller;
using TinyMetroWpfLibrary.EventAggregation;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    /// <summary>
    /// The ListPicker is a modification and extension to the DatePicker that Charles Petzold described.
    /// It's designed to work like the ListPicker of Windows Phone 7
    /// <see cref="http://msdn.microsoft.com/de-de/magazine/gg309180.aspx"/>
    /// </summary>
    public partial class ListPicker : IHandleMessage<ListPickerSelectItemRequest>, INotifyPropertyChanged
    {
        /// <summary>
        /// Unique Identifier used to identify the message from ListPickerFullModeViewModel
        /// </summary>
        private readonly Guid listPickerId = Guid.NewGuid(); 

        #region Dependency Properties

        /// <summary>
        /// Item source. That's an enumerable collection of elements
        /// </summary>
        public static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ListPicker));

        /// <summary>
        /// DisplayValue
        /// </summary>
        public static DependencyProperty DisplayValueProperty = DependencyProperty.Register("DisplayValue", typeof (string), typeof (ListPicker));

        /// <summary>
        /// Selected item of the item source.
        /// </summary>
        public static DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof (object), typeof (ListPicker), new PropertyMetadata(null, OnSelectedItemChangedCallback, OnCoerceValueCallback));

        /// <summary>
        /// IsReadonly property specifies if the value shall be shown, but not enabled for editing
        /// </summary>
        public static DependencyProperty IsReadonlyProperty = DependencyProperty.Register("IsReadonly", typeof(bool), typeof(ListPicker));

        private static object OnCoerceValueCallback(DependencyObject d, object basevalue)
        {
            return basevalue;
        }

        private static void OnSelectedItemChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null && e.OldValue != null)
                d.SetValue(DisplayValueProperty, string.Empty);
            else
                ((ListPicker)d).UpdateDisplayValue();
        }

        /// <summary>
        /// Name of the displayed properties of the selected item.
        /// If null, than the selected item will be shown within accessing any property.
        /// </summary>
        public static DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof (string), typeof (ListPicker), new PropertyMetadata(null, OnDisplayMemberPathChangedCallback));

        private static void OnDisplayMemberPathChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ListPicker)d).UpdateDisplayValue();
        }

        /// <summary>
        /// FullMode Header that is shown in the fullmode page
        /// </summary>
        public static DependencyProperty FullModeHeaderProperty =
            DependencyProperty.Register("FullModeHeader", typeof (string), typeof (ListPicker));

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the FullMode Item Template
        /// http://chris.59north.com/post/Using-DataTemplates-in-custom-controls.aspx
        /// </summary>
        public DataTemplate FullModeItemTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bound items source collection
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected item, which must be included in the items source
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Display Member Path
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string) GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the FullMode Header
        /// </summary>
        public string FullModeHeader
        {
            get { return (string) GetValue(FullModeHeaderProperty); }
            set { SetValue(FullModeHeaderProperty, value); }
        }

        /// <summary>
        /// Gets the Display Value
        /// </summary>
        public string DisplayValue
        {
            get { return (string)GetValue(DisplayValueProperty); }
            set { SetValue(DisplayValueProperty, value); }
        }

        /// <summary>
        /// Updates the DisplayValue
        /// </summary>
        private void UpdateDisplayValue()
        {
            // If no Item has been set, return an empty string
            if (SelectedItem == null)
            {
                DisplayValue = string.Empty;
                return;
            }

            // If the Display Member Path is not set, than use the SelectedItem as it is.
            if (string.IsNullOrEmpty(DisplayMemberPath))
            {
                DisplayValue = SelectedItem.ToString();
                return;
            }

            // Try to Get the Property out of the Selected Item
            var info = SelectedItem.GetType().GetProperty(DisplayMemberPath);
            if (info == null)
            {
                DisplayValue = string.Format("<{0}> not found", DisplayMemberPath);
                return;
            }

            // Return the evaluted property content
            var value = info.GetValue(SelectedItem, null);
            DisplayValue = value == null ? string.Empty : value.ToString();
        }

        /// <summary>
        /// Gets the Unique Identifier used to identify the message from ListPickerFullModeViewModel
        /// </summary>
        public Guid ListPickerId
        {
            get { return listPickerId; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ListPicker class
        /// </summary>
        public ListPicker()
        {
            InitializeComponent();
            ListPickerContent.DataContext = this;

            if (!ViewModelBase.IsDesignMode)
                Kernel.Instance.EventAggregator.Subscribe(this);

            IsEnabledChanged += (s, e) =>
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("PickerCursor"));
                }
            };
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ListPicker()
        {
            if (!ViewModelBase.IsDesignMode)
                Kernel.Instance.EventAggregator.Unsubscribe(this);
        }

        #endregion

        #region Send Request to Enter the FullMode

        /// <summary>
        /// Send FullMode Request when the user clicks the Selected Item
        /// </summary>
        private void OnEnterFullModeViaClick(object sender, MouseButtonEventArgs e)
        {
            oldAnimationMode = Kernel.Instance.EventAggregator.PublishMessage(
                new ChangeAnimationModeRequest(Frames.AnimationMode.Fade));

            Kernel.Instance.EventAggregator.PublishMessage(
                new ListPickerFullModeRequest(FullModeHeader, ItemsSource, SelectedItem, FullModeItemTemplate, ListPickerId));
        }

        /// <summary>
        /// Send FullMode Request when the user touches the Selected Item
        /// </summary>
        private void OnEnterFullModeViaTouch(object sender, TouchEventArgs e)
        {
            oldAnimationMode = Kernel.Instance.EventAggregator.PublishMessage(
                new ChangeAnimationModeRequest(Frames.AnimationMode.Fade));

            Kernel.Instance.EventAggregator.PublishMessage(
                new ListPickerFullModeRequest(FullModeHeader, ItemsSource, SelectedItem, FullModeItemTemplate, ListPickerId));
        }

        #endregion

        /// <summary>
        /// Handles the item selection
        /// </summary>
        /// <param name="message">item selection message</param>
        public void Handle(ListPickerSelectItemRequest message)
        {
            if (message == null || message.ListPickerId != ListPickerId)
                return;

            // Set the selected Item
            SelectedItem = message.SelectedItem;
            Kernel.Instance.EventAggregator.PublishMessage(new GoBackNavigationRequest());

            // switch the animation mode
            if (oldAnimationMode != null)
                Kernel.Instance.EventAggregator.PublishMessage(oldAnimationMode);
        }


        /// <summary>
        /// Gets or sets the IsReadonly flag
        /// </summary>
        public bool IsReadonly
        {
            get { return (bool)GetValue(IsReadonlyProperty); }
            set
            {
                SetValue(IsReadonlyProperty, value);

                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("PickerCursor"));
            }
        }

        /// <summary>
        /// Gets the Picker Cursor 
        /// </summary>
        public Cursor PickerCursor
        {
            get { return IsReadonly || !IsEnabled ? Cursors.Arrow : Cursors.Hand; }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private ChangeAnimationModeRequest oldAnimationMode;

        #endregion
    }
}
