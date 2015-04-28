using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Net.NetworkInformation;
using System.Net;

namespace TSLib.Set
{
    /// <summary>
    /// 与网络设置有关的类
    /// </summary>
    public static class Adapter
    {
        /// <summary>
        /// 设置所有适配器的属性
        /// <para>如设置失败不会产生错误，请在设置后获取其当前值以检查是否成功</para>
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="submask">子网掩码</param>
        /// <param name="getway">网关地址</param>
        /// <param name="dns">DNS服务器地址，如[0]为"back"即恢复为自动获取</param>
        public static void SetNetwork(string[] ip, string[] submask, string[] getway, string[] dns)
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            ManagementBaseObject inPar = null;
            ManagementBaseObject outPar = null;
            foreach (ManagementObject mo in moc)
            {
                //如果没有启用IP设置的网络设备则跳过
                if (!(bool)mo["IPEnabled"])
                    continue;

                //设置IP地址和子网掩码
                if (ip != null && submask != null)
                {
                    inPar = mo.GetMethodParameters("EnableStatic");
                    inPar["IPAddress"] = ip;
                    inPar["SubnetMask"] = submask;
                    outPar = mo.InvokeMethod("EnableStatic", inPar, null);
                }

                //设置网关地址
                if (getway != null)
                {
                    inPar = mo.GetMethodParameters("SetGateways");
                    inPar["DefaultIPGateway"] = getway;
                    outPar = mo.InvokeMethod("SetGateways", inPar, null);
                }

                //设置DNS地址
                if (dns != null)
                {
                    inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                    inPar["DNSServerSearchOrder"] = dns;
                    outPar = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
                }
                else if (dns[0] == "back")
                {
                    mo.InvokeMethod("SetDNSServerSearchOrder", null);
                }
            }
        }
        /// <summary>
        /// 设置所有适配器的DNS服务器
        /// <para>如设置失败不会产生错误，请在设置后获取当前DNS以检查是否成功</para>
        /// </summary>
        /// <param name="d1">首选DNS服务器地址，如为null则设置为自动获取</param>
        /// <param name="d2">备用DNS服务器地址，如为null且d1不为为null则备用为空</param>
        static void SetDNS(string d1, string d2)
        {
            if (d1 != null)
            {
                if (d2 != null)
                {
                    SetNetwork(null, null, null, new string[] { d1, d2 });
                }
                else
                {
                    SetNetwork(null, null, null, new string[] { d1 });
                }
            }
            else
            {
                SetNetwork(null, null, null, new string[] { "back" });
            }
        }

        /// <summary>
        /// 获取所有适配器的状态、属性
        /// </summary>
        /// <returns>每个适配器的泛型</returns>
        public static List<TSAdapter> GetAdapters()
        {
            List<TSAdapter> back = new List<TSAdapter>();
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter");
            ManagementObjectCollection wmiadps = searcher.Get();

            foreach (NetworkInterface adapter in adapters)
            {
                TSAdapter adp = new TSAdapter();
                adp.Name = adapter.Name;
                adp.Description = adapter.Description;
                adp.Status = adapter.OperationalStatus;
                adp.Type = adapter.NetworkInterfaceType;

                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                //网关
                GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
                if (addresses.Count > 0)
                {
                    adp.Gateway = addresses[0].Address.ToString();
                }

                //DNS
                IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                if (dnsServers.Count > 0)
                {
                    foreach (IPAddress dns in dnsServers)
                    {
                        adp.DNS.Add(dns.ToString());
                    }
                }

                //WMI中的数据
                foreach (ManagementObject mo in wmiadps)
                {
                    if ((string)mo.GetPropertyValue("NetConnectionID") == adp.Name)
                    {
                        //接口(XP没有该属性!)
                        try
                        {
                            adp.Interface = (UInt32)mo.GetPropertyValue("InterfaceIndex");
                        }
                        catch
                        {
                            adp.Interface = null;
                        }
                        //服务名称
                        adp.ServiceName = (string)mo.GetPropertyValue("ServiceName");
                    }
                }
                back.Add(adp);
            }
            return back;
        }

        /// <summary>
        /// 获取所有适配器的报告
        /// </summary>
        /// <returns>带有换行符的所有适配器的报告</returns>
        public static string GetAdaptersReport()
        {
            List<TSAdapter> adps = GetAdapters();
            string back = "";
            foreach (TSAdapter adp in adps)
            {
                back += "==================================================\r\n";
                back += "名称:" + adp.Name + "\r\n";
                back += "描述:" + adp.Description + "\r\n";
                back += "类型:" + adp.Type.ToString() + "\r\n";
                back += "状态:" + adp.Status.ToString() + "\r\n";
                back += "网关:" + adp.Gateway + "\r\n";
                foreach (string d in adp.DNS)
                {
                    back += "DNS:" + d + "\r\n";
                }
                back += "接口:" + adp.Interface + "\r\n";
                back += "服务名:" + adp.ServiceName + "\r\n";
            }

            return back;
        }
        //TODO:设置其他属性
    }
}
