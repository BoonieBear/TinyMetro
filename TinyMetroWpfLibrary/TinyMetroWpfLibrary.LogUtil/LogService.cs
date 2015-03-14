using System;

namespace TinyMetroWpfLibrary.LogUtil
{
    public class LogService
    {
        public static ILogService GetLogger(Type t)
        {
            return new FileLogService(t);
        }
    }
}
