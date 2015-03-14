using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.Win32;
using TinyMetroWpfLibrary.LogUtil;
namespace TinyMetroWpfLibrary.Utility
{
    public class NetworkInterfaceQuery
    {
        private static ILogService logger = new FileLogService(typeof(NetworkInterfaceQuery));

        public static string QueryWirelessCardGuid(string name)
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && adapter.Description.ToLower().Contains(name.ToLower()))
                {
                    return adapter.Id;
                }
            }
            return "";
        }

        /*
        public static List<NetworkInterface> GetAvailableWirelessInterfacesExcludeGuid(List<string> guidList)
        {
            List<NetworkInterface> interfaceList = new List<NetworkInterface>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                foreach (var guid in guidList)
                {
                    if (guid == null)
                    {

                        filterInterface(interfaceList, adapter);
                    }
                    else
                    {
                        if ("{" + guid.ToString().ToLower() + "}" != adapter.Id.ToLower())
                        {
                            filterInterface(interfaceList, adapter);
                        }
                    }
                }
            }
            return interfaceList;
        }

        private static void filterInterface(List<NetworkInterface> interfaceList, NetworkInterface adapter)
        {
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
            {
                if (IsRealAdapter(adapter))
                {
                    interfaceList.Add(adapter);
                }
            }
        }
         */
 
        public static List<NetworkInterface> GetAvailableEthernetInterfaces()
        {
            List<NetworkInterface> interfaceList = new List<NetworkInterface>();

            //to show all network information in registry
            if (DebugConfig.GetInstance().LogAllNetworkInterfaceInfo)
            {
                IsRealAdapter(null);
            }

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (DebugConfig.GetInstance().LogAllNetworkInterfaceInfo)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("Id=");
                    builder.Append(adapter.Id);
                    builder.Append(",OperationalStatus=");
                    builder.Append(adapter.OperationalStatus);
                    builder.Append(",NetworkInterfaceType=");
                    builder.Append(adapter.NetworkInterfaceType);
                    builder.Append(",Description=");
                    builder.Append(adapter.Description);
                    logger.Debug(builder.ToString());
                }

                if (adapter.OperationalStatus == OperationalStatus.Up && adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    if (IsRealAdapter(adapter))
                    {
                        interfaceList.Add(adapter);
                    }
                }
            }
            return interfaceList;
        }

        public static List<string> GetAvailableEthernetInterfacesGuid()
        {
            List<string> interfaceList = new List<string>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up && adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    if (IsRealAdapter(adapter))
                    {
                        interfaceList.Add(adapter.Id.TrimStart('{').TrimEnd('}'));
                    }
                }
            }
            return interfaceList;
        }

        public static NetworkInterface GetAdapterInterfaceByGuid(string guid)
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if ("{" + guid.ToString().ToLower() + "}" == adapter.Id.ToLower())
                {
                    return adapter;
                }
            }
            return null;
        }

        private static bool IsRealAdapter(NetworkInterface adapter)
        {
            //todo :(Dominic) need refactor this code, no need to execute this code every time
            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey system = hkml.OpenSubKey("SYSTEM", true);
            RegistryKey currentControlSet = system.OpenSubKey("CurrentControlSet", true);
            RegistryKey control = currentControlSet.OpenSubKey("Control", true);
            RegistryKey cla = control.OpenSubKey("Class", true);
            RegistryKey guid = cla.OpenSubKey("{4D36E972-E325-11CE-BFC1-08002bE10318}", true);

            string[] keyarray = guid.GetSubKeyNames();

            foreach (string keystring in keyarray)
            {
                int keyInt;
                if (!int.TryParse(keystring, out keyInt))
                {
                    continue;
                }

                RegistryKey key = guid.OpenSubKey(keystring, true);

                object obj = null;
                if (key != null)
                {
                    obj = key.GetValue("NetCfgInstanceId");

                    if (obj != null)
                    {
                        string netcardid = obj.ToString();

                        if (adapter != null && netcardid == adapter.Id)
                        {
                            object CharacteristicsObj = key.GetValue("Characteristics");
                            if (CharacteristicsObj != null)
                            {
                                int Characteristics = 0;
                                Characteristics = Convert.ToInt32(CharacteristicsObj);

                                if ((Characteristics & 0x4) == 0x4)
                                {
                                    return true;
                                }
                            }
                        }

                        if(adapter == null)
                        {
                            if (DebugConfig.GetInstance().LogAllNetworkInterfaceInfo)
                            {
                                StringBuilder builder = new StringBuilder();
                                builder.Append("NetCfgInstanceId=");
                                builder.Append(netcardid);
                                builder.Append(",Characteristics=");
                                object CharacteristicsObj = key.GetValue("Characteristics");
                                int Characteristics = 0;
                                if (CharacteristicsObj != null)
                                {
                                    Characteristics = Convert.ToInt32(CharacteristicsObj);
                                }
                                builder.Append(Characteristics);
                                builder.Append(",Characteristics&0x4=");
                                builder.Append(Characteristics & 0x4);
                                logger.Debug(builder.ToString());
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
