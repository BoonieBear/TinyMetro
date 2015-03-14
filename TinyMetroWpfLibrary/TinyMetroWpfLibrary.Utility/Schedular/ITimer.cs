using System;

namespace TinyMetroWpfLibrary.Utility.Schedular
{
    public interface ITimer
    {
        TimeSpan Interval { get; set; }
        void Start();
        void Stop();
        void Register(TimerEventHandler Tick);
        //void Register(ElapsedEventHandler Tick);
        //void Register(EventHandler Tick);
    }
}
