﻿using System;

namespace TinyMetroWpfLibrary.Utility.Schedular
{
    public class Task
    {
        private TimeSpan _interval;
        TaskExecution _execution;

        public TaskExecution Execution { get { return _execution; } }

        public TimeSpan Interval { get { return _interval; } }

        public Task(TimeSpan interval, TaskExecution execution)
        {
            _interval = interval;
            _execution = execution;
        }
    }
}
