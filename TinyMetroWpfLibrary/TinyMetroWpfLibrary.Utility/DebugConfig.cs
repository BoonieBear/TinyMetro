using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TinyMetroWpfLibrary.Utility
{
    public class DebugConfig
    {
        private static bool _isInstantiated = false;
        private static object lockObj = new object();
        private static DebugConfig _config = null;

        public static DebugConfig GetInstance()
        {
            if (!_isInstantiated)
            {
                try
                {
                    Monitor.Enter(lockObj);
                    if (!_isInstantiated)
                    {
                        _config = new DebugConfig();
                        _isInstantiated = true;
                    }
                }
                finally
                {
                    Monitor.Exit(lockObj);
                }
                return _config;
            }

            return _config;
        }

        public bool LoadFakeData { get; private set; }
        public bool LogWlannotificationCallback { get; private set; }
        public bool LogProcessingIteration { get; private set; }
        public int ParserIndex { get; private set; }
        public bool SkipLoadParser { get; private set; }
        public bool LogWlanScan { get; private set; }
        public bool LogWlanScanInterval { get; private set; }
        public bool LogWlanScanUpdateDetail { get; private set; }
        public bool LogIterationResult { get; private set; }
        public bool LogFTPHTTPEachTransfer { get; private set; }
        public int ConfigRefreshInterval { get; private set; }
        public bool SkipAssociationTest { get; private set; }
        public bool LogAssociationDetail { get; private set; }
        public int AssociationNumOfTest { get; private set; }
        public bool ResolveNameWhenTrace { get; private set; }
        public bool MonitorDevice { get; private set; }
        public string MonitoredDeviceMac { get; private set; }
        public List<string> MonitoredDeviceMacList 
        {
            get
            {
                List<string> list = new List<string>();

                string[] adds = MonitoredDeviceMac.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (adds.Length > 0)
                {
                    foreach (string add in adds)
                    {
                        list.Add(add);
                    }
                }

                return list;
            }
        }
        public bool OutputAllDevice { get; private set; }
        public int FoucusChannel { get; private set; }
        public bool CheckEthernetWhenDoUX { get; private set; }
        public bool LogWlanInvocation { get; private set; }
        public bool LogAllNetworkInterfaceInfo { get; private set; }

        private double _interval = 60000;
        private System.Timers.Timer _timer;

        private DebugConfig()
        {
            _timer = new System.Timers.Timer();
            Refresh();
        }

        public void BeginRefresh()
        {
            if (ConfigRefreshInterval > 0)
            {
                _interval = ConfigRefreshInterval;
            }
            _timer.Interval = _interval;
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            LoadFakeData = false;
            LogWlannotificationCallback = false;
            LogProcessingIteration = false;
            ParserIndex = 0;
            SkipLoadParser = false;
            LogWlanScan = false;
            LogIterationResult = true;
            LogFTPHTTPEachTransfer = false;
            ConfigRefreshInterval = 0;
            SkipAssociationTest = false;
            LogWlanScanInterval = false;
            LogAssociationDetail = true;
            AssociationNumOfTest = 0;
            ResolveNameWhenTrace = true;
            LogWlanScanUpdateDetail = false;
            MonitorDevice = false;
            MonitoredDeviceMac = string.Empty;
            OutputAllDevice = false;
            FoucusChannel = -1;
            CheckEthernetWhenDoUX = true;
            LogWlanInvocation = false;
            LogAllNetworkInterfaceInfo = false;

            try
            {
                if (!File.Exists("debugconfig.conf"))
                {
                    return;
                }

                string[] lines = System.IO.File.ReadAllLines("debugconfig.conf");

                foreach (string line in lines)
                {
                    string[] strarr = line.Split('=');

                    if (strarr[0] == "LogAllNetworkInformation")
                    {
                        if (strarr[1] == "true")
                        {
                            LogAllNetworkInterfaceInfo = true;
                        }
                    }

                    if (strarr[0] == "LogWlanInvocation")
                    {
                        if (strarr[1] == "true")
                        {
                            LogWlanInvocation = true;
                        }
                    }

                    if (strarr[0] == "LoadFakeData")
                    {
                        if (strarr[1] == "true")
                        {
                            LoadFakeData = true;
                        }
                    }

                    if (strarr[0] == "LogWlannotificationCallback")  
                    {
                        if (strarr[1] == "true")
                        {
                            LogWlannotificationCallback = true;
                        }
                    }

                    if (strarr[0] == "LogProcessingIteration")
                    {
                        if (strarr[1] == "true")
                        {
                            LogProcessingIteration = true;
                        }
                    }

                    if (strarr[0] == "ParserIndex")
                    {
                        ParserIndex = Convert.ToInt32(strarr[1]);
                    }

                    if (strarr[0] == "SkipLoadParser")
                    {
                        if (strarr[1] == "true")
                        {
                            SkipLoadParser = true;
                        }
                    }

                    if (strarr[0] == "LogWlanScan")
                    {
                        if (strarr[1] == "true")
                        {
                            LogWlanScan = true;
                        }
                    }

                    if (strarr[0] == "LogWlanScanUpdateDetail")
                    {
                        if (strarr[1] == "true")
                        {
                            LogWlanScanUpdateDetail = true;
                        }
                    }

                    if (strarr[0] == "LogIterationResult")
                    {
                        if (strarr[1] == "false")
                        {
                            LogIterationResult = false;
                        }
                    }

                    if (strarr[0] == "LogFTPHTTPEachTransfer")
                    {
                        if (strarr[1] == "true")
                        {
                            LogFTPHTTPEachTransfer = true;
                        }
                    }

                    if (strarr[0] == "ConfigRefreshInterval")
                    {
                        ConfigRefreshInterval = Convert.ToInt32(strarr[1]);

                    }
                    if (strarr[0] == "SkipAssociationTest")
                    {
                        if (strarr[1] == "true")
                        {
                            SkipAssociationTest = true;
                        }
                    }
                    if (strarr[0] == "LogWlanScanInterval")
                    {
                        if (strarr[1] == "true")
                        {
                            LogWlanScanInterval = true;
                        }
                    }

                    if (strarr[0] == "LogAssociationDetail")
                    {
                        if (strarr[1] == "false")
                        {
                            LogAssociationDetail = false;
                        }
                    }

                    if (strarr[0] == "AssociationNumOfTest")
                    {
                        AssociationNumOfTest = Convert.ToInt32(strarr[1]);
                    }

                    if (strarr[0] == "ResolveNameWhenTrace")
                    {
                        if (strarr[1] == "true")
                        {
                            ResolveNameWhenTrace = true;
                        }
                        else
                        {
                            ResolveNameWhenTrace = false;
                        }
                    }

                    if (strarr[0] == "MonitorDevice")
                    {
                        if (strarr[1] == "true")
                        {
                            MonitorDevice = true;
                        }
                    }

                    if (strarr[0] == "MonitoredDeviceMac")
                    {
                        MonitoredDeviceMac = strarr[1];
                    }

                    if (strarr[0] == "OutputAllDevice")
                    {
                        if (strarr[1] == "true")
                        {
                            OutputAllDevice = true;
                        }
                    }

                    if (strarr[0] == "FoucusChannel")
                    {
                        FoucusChannel = Convert.ToInt32(strarr[1]);
                    }

                    if (strarr[0] == "CheckEthernetWhenDoUX")
                    {
                        if (strarr[1] == "false")
                        {
                            CheckEthernetWhenDoUX = false;
                        }
                    }
                }
            }
            catch(Exception)
            {
            }
        }
    }
}
