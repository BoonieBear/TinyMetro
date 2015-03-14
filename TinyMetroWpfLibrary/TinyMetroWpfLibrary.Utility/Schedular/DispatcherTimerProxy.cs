﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Threading;
using TinyMetroWpfLibrary.Utility;
namespace TinyMetroWpfLibrary.Utility.Schedular
{
    public class DispatcherTimerProxy : ITimer
    {
        DispatcherTimer _timer = new DispatcherTimer();
        private TimerEventHandler _handler;

        public TimeSpan Interval
        {
            get
            {
                return _timer.Interval;
            }
            set
            {
                _timer.Interval = value;
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void BeginInvoke(TaskExecution execution)
        {
            _timer.Dispatcher.BeginInvoke(execution);
        }

        public void Register(TimerEventHandler Tick)
        {
            _handler = Tick;
            _timer.Tick += _timer_Tick;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (_handler != null)
            {
                _handler();
            }
        }
    }
}
