using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
namespace TinyMetroWpfLibrary.Controls
{
    public class ListBoxHorizontalRepeatButton : RepeatButton, INotifyPropertyChanged
    {
        public ListBox TargetListBox
        {
            get { return (ListBox)GetValue(TargetListBoxProperty); }
            set { SetValue(TargetListBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetListBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetListBoxProperty =
            DependencyProperty.Register("TargetListBox", typeof(ListBox), typeof(ListBoxHorizontalRepeatButton), new PropertyMetadata(null, TargetListBoxChanged));

        private static void TargetListBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBoxHorizontalRepeatButton listboxRepeatButton = d as ListBoxHorizontalRepeatButton;
            if (listboxRepeatButton != null)
            {
                listboxRepeatButton.OnTargetListBoxChanged(e.NewValue);
            }
        }

        public HorizontalAlignment HorizontalType
        {
            get { return (HorizontalAlignment)GetValue(HorizontalTypeProperty); }
            set { SetValue(HorizontalTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalTypeProperty =
            DependencyProperty.Register("HorizontalType", typeof(HorizontalAlignment), typeof(ListBoxHorizontalRepeatButton), new PropertyMetadata(HorizontalAlignment.Center));


       

        private ListBox _targetListBox;
        private ScrollViewer _targetScrollViewer;
        private const double SMALL_CHANGE_SIZE = 20;
        public void OnTargetListBoxChanged(object newValue)
        {
            _targetListBox = newValue as ListBox;
            _targetScrollViewer = TreeHelper.FindChildOfType<ScrollViewer>(_targetListBox);
            if (_targetScrollViewer != null)
            {
                _targetScrollViewer.ScrollChanged += OnScrollViewChanged;
                OnScrollViewChanged(null, null);
            }
        }

        private void OnScrollViewChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_targetScrollViewer == null)
            {
                return;
            }

            if (HorizontalType == System.Windows.HorizontalAlignment.Left)
            {
                this.IsEnabled = _targetScrollViewer.HorizontalOffset == 0 ? false : true;
            }
            else
            {
                this.IsEnabled = _targetScrollViewer.HorizontalOffset == _targetScrollViewer.ScrollableWidth ? false : true;
            }
        }

        protected override void OnClick()
        {
            base.OnClick();
            if (_targetScrollViewer == null)
            {
                return;
            }

            if (HorizontalType == System.Windows.HorizontalAlignment.Left)
            {
                _targetScrollViewer.ScrollToHorizontalOffset(_targetScrollViewer.HorizontalOffset - SMALL_CHANGE_SIZE);
            }
            else
            {
                _targetScrollViewer.ScrollToHorizontalOffset(_targetScrollViewer.HorizontalOffset + SMALL_CHANGE_SIZE);
            }            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(name);
                PropertyChanged(this, e);
            }
        }
    }
}
