using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Management;
using Microsoft.Win32;

namespace TSLib.Net
{
    /// <summary>
    /// TSLib定义的网络适配器类
    /// </summary>
    public class TSAdapter
    {
        //TODO:过去连接的WIFI：HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\NetworkList\Profiles

        public TSAdapter() { }

        /// <summary>
        /// 获取单个适配器的信息
        /// <para>所有请使用GetAdapters()</para>
        /// </summary>
        /// <param name="name">适配器的名称</param>
        public TSAdapter(string name)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter");
            ManagementObjectCollection wmiadps = searcher.Get();

            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Name == name)
                {
                    //this.name = adapter.Name;
                    this.description = adapter.Description;
                    this.status = adapter.OperationalStatus;
                    this.type = adapter.NetworkInterfaceType;

                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
                    IPv4InterfaceProperties p = adapterProperties.GetIPv4Properties();
                    //IP
                    if (uniCast.Count > 1)
                    {
                        if (uniCast[1].IPv4Mask != null)
                        {
                            this.ip = uniCast[1].Address.ToString();
                            this.mask = uniCast[1].IPv4Mask.ToString();
                        }

                    }
                    //网关
                    GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
                    if (addresses.Count > 0)
                    {
                        this.gateway = addresses[0].Address.ToString();
                    }

                    //DNS
                    IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                    switch (dnsServers.Count)
                    {
                        case 1:
                            this.dns = new string[1];
                            this.dns[0] = dnsServers[0].ToString();
                            break;
                        case 2:
                            this.dns = new string[2];
                            this.dns[0] = dnsServers[0].ToString();
                            this.dns[1] = dnsServers[1].ToString();
                            break;
                    }
                    //Index
                    if (p != null)
                    {
                        this.interFace = p.Index;
                    }

                    //WMI中的数据
                    foreach (ManagementObject mo in wmiadps)
                    {
                        if ((string)mo.GetPropertyValue("NetConnectionID") == this.name)
                        {
                            //服务名称
                            this.serviceName = (string)mo.GetPropertyValue("ServiceName");
                        }
                    }
                    break;
                }
            }
        }

        private string name;
        private string description;
        private string serviceName;
        private OperationalStatus status;
        private NetworkInterfaceType type;
        private string[] dns;
        private Int32? interFace;
        private string gateway;
        private string ip;
        private string mask;
        private bool isAutoDns;
        private string guid;

        /// <summary>
        /// 适配器的名称
        /// </summary>
        public string Name 
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// 适配器的描述
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// 适配器的服务名
        /// </summary>
        public string ServiceName
        {
            get
            {
                return serviceName;
            }
        }

        /// <summary>
        /// 适配器的状态
        /// </summary>
        public OperationalStatus Status
        {
            get
            {
                return status;
            }
        }

        /// <summary>
        /// 适配器的类型
        /// </summary>
        public NetworkInterfaceType Type
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// 是否为自动DNS
        /// </summary>
        public bool IsAutoDns
        {
            get
            {
                return isAutoDns;
            }
        }

        /// <summary>
        /// 设备的GUID
        /// </summary>
        public string GUID
        {
            get
            {
                return guid;
            }
        }

        /// <summary>
        /// 适配器的DNS服务器
        /// <para>可以设置1~2个DNS，第二个为备用DNS，赋为null切换回自动获取</para>
        /// </summary>
        public string[] DNS
        {
            get
            {
                return dns;
            }
            set
            {
                ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = wmi.GetInstances();
                ManagementBaseObject i = null;
                ManagementBaseObject o = null;
                foreach (ManagementObject mo in moc)
                {
                    if ((UInt32)mo["InterfaceIndex"] == Interface)
                    {
                        if (value == null)
                        {
                            UInt32 t;
                            t = (UInt32)mo.InvokeMethod("SetDNSServerSearchOrder", null);
                            if (t != 0)
                                throw new TSAdapterSetException((UInt32)o["returnValue"]);
                            break;
                        }
                        else
                        {
                            i = mo.GetMethodParameters("SetDNSServerSearchOrder");
                            i["DNSServerSearchOrder"] = value;
                            o = mo.InvokeMethod("SetDNSServerSearchOrder", i, null);
                            if ((UInt32)o["returnValue"] != 0)
                                throw new TSAdapterSetException((UInt32)o["returnValue"]);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 适配器的接口
        /// <para>就是.NET中的Index</para>
        /// </summary>
        public Int32? Interface 
        {
            get
            {
                return interFace;
            }
        }

        /// <summary>
        /// 适配器的网关
        /// <para>赋为null即将IP、子网掩码、网关一同恢复为自动获取</para>
        /// </summary>
        public string Gateway 
        {
            get
            {
                return gateway;
            }
            set
            {
                ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = wmi.GetInstances();
                ManagementBaseObject i = null;
                ManagementBaseObject o = null;
                foreach (ManagementObject mo in moc)
                {
                    if ((UInt32)mo["InterfaceIndex"] == Interface)
                    {
                        if (value == null)
                        {
                            i = mo.GetMethodParameters("EnableDHCP");
                            o = mo.InvokeMethod("EnableDHCP", i, null);
                            if ((UInt32)o["returnValue"] != 0)
                                throw new TSAdapterSetException((UInt32)o["returnValue"]);
                            break;
                        }
                        else
                        {
                            i = mo.GetMethodParameters("SetGateways");
                            i["DefaultIPGateway"] = new string[] { value };
                            o = mo.InvokeMethod("SetGateways", i, null);
                            if ((UInt32)o["returnValue"] != 0)
                                throw new TSAdapterSetException((UInt32)o["returnValue"]);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 该适配器分配到的IP地址
        /// <para>如需更改，请使用SetStatic方法</para>
        /// </summary>
        public string IP
        {
            get
            {
                return ip;
            }
        }

        /// <summary>
        /// 适配器的子网掩码
        /// <para>如需更改，请使用SetStatic方法</para>
        /// </summary>
        public string Mask
        {
            get
            {
                return mask;
            }
        }

        void SetNetwork(string[] ip, string[] submask, string[] getway, string[] dns)
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
                    //outPar["returnValue"].ToString()
                }
                else if (dns[0] == "back")
                {
                    mo.InvokeMethod("SetDNSServerSearchOrder", null);
                }
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
                adp.name = adapter.Name;
                adp.description = adapter.Description;
                adp.status = adapter.OperationalStatus;
                adp.type = adapter.NetworkInterfaceType;

                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;

                IPv4InterfaceProperties p;
                try
                {
                    p = adapterProperties.GetIPv4Properties();
                }
                catch
                {
                    p = null;
                }
                //IP
                if (uniCast.Count > 1)
                {
                    if (uniCast[1].IPv4Mask != null)
                    {
                        adp.ip = uniCast[1].Address.ToString();
                        adp.mask = uniCast[1].IPv4Mask.ToString();
                    }

                }
                //网关
                GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
                if (addresses.Count > 0)
                {
                    adp.gateway = addresses[0].Address.ToString();
                }

                //DNS
                IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                switch (dnsServers.Count)
                {
                    case 1:
                        adp.dns = new string[1];
                        adp.dns[0] = dnsServers[0].ToString();
                        break;                   
                    case 2:
                        adp.dns = new string[2];
                        adp.dns[0] = dnsServers[0].ToString();
                        adp.dns[1] = dnsServers[1].ToString();
                        break;
                }

                //Index
                if (p != null)
                {
                    adp.interFace = p.Index;
                }

                //WMI中的数据
                foreach (ManagementObject mo in wmiadps)
                {
                    if ((string)mo.GetPropertyValue("NetConnectionID") == adp.name)
                    {
                        //服务名称
                        adp.serviceName = (string)mo.GetPropertyValue("ServiceName");
                        //GUID
                        adp.guid = (string)mo.GetPropertyValue("GUID");
                    }
                }

                //是否为自动DNS
                RegistryKey r1 = Registry.LocalMachine;
                RegistryKey r2 = r1.OpenSubKey(@"SYSTEM\ControlSet001\Services\Tcpip\Parameters\Interfaces\" + adp.guid);
                string t = (string)r2.GetValue("NameServer");
                if (t == "")
                {
                    adp.isAutoDns = true;
                }
                else
                {
                    adp.isAutoDns = false;
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
                back += "是否为动态DNS：" + adp.IsAutoDns.ToString() + "\r\n";
                if (adp.DNS != null)
                {
                    foreach (string d in adp.DNS)
                    {
                        back += "DNS:" + d + "\r\n";
                    }
                }
                back += "接口:" + adp.Interface + "\r\n";
                back += "服务名:" + adp.ServiceName + "\r\n";
                back += "IP：" + adp.IP + "\r\n";
                back += "子网掩码：" + adp.Mask + "\r\n";
            }
            return back;
        }

        /// <summary>
        /// 刷新适配器状态
        /// </summary>
        public void Fresh()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter");
            ManagementObjectCollection wmiadps = searcher.Get();

            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Name == name)
                {
                    status = adapter.OperationalStatus;
                    type = adapter.NetworkInterfaceType;

                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
                    IPv4InterfaceProperties p = adapterProperties.GetIPv4Properties();
                    //IP
                    if (uniCast.Count > 1)
                    {
                        if (uniCast[1].IPv4Mask != null)
                        {
                            ip = uniCast[1].Address.ToString();
                            mask = uniCast[1].IPv4Mask.ToString();
                        }

                    }
                    //网关
                    GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
                    if (addresses.Count > 0)
                    {
                        gateway = addresses[0].Address.ToString();
                    }

                    //DNS
                    IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                    switch (dnsServers.Count)
                    {
                        case 1:
                            dns = new string[1];
                            dns[0] = dnsServers[0].ToString();
                            break;
                        case 2:
                            dns = new string[2];
                            dns[0] = dnsServers[0].ToString();
                            dns[1] = dnsServers[1].ToString();
                            break;
                    }

                    //Index
                    if (p != null)
                    {
                        interFace = p.Index;
                    }

                    //WMI中的数据
                    foreach (ManagementObject mo in wmiadps)
                    {
                        if ((string)mo.GetPropertyValue("NetConnectionID") == name)
                        {
                            //服务名称
                            serviceName = (string)mo.GetPropertyValue("ServiceName");
                        }
                    }

                    //是否为自动DNS
                    RegistryKey r1 = Registry.LocalMachine;
                    RegistryKey r2 = r1.OpenSubKey(@"SYSTEM\ControlSet001\Services\Tcpip\Parameters\Interfaces\" + guid);
                    string t = (string)r2.GetValue("NameServer");
                    if (t == "")
                    {
                        isAutoDns = true;
                    }
                    else
                    {
                        isAutoDns = false;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// 设置静态IP以及子网掩码
        /// </summary>
        /// <param name="ip">IP地址，如为null即设置IP与子网掩码为自动获取</param>
        /// <param name="mask">子网掩码，如为null即设置IP与子网掩码为自动获取</param>
        public void SetStatic(string ip,string mask)
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            ManagementBaseObject i = null;
            ManagementBaseObject o = null;
            foreach (ManagementObject mo in moc)
            {
                if ((UInt32)mo["InterfaceIndex"] == Interface)
                {
                    if ((ip == null)||(mask == null))
                    {
                        i = mo.GetMethodParameters("EnableDHCP");
                        o = mo.InvokeMethod("EnableDHCP", i, null);
                        if ((UInt32)o["returnValue"] != 0)
                            throw new TSAdapterSetException((UInt32)o["returnValue"]);
                        break;
                    }
                    else
                    {
                        i = mo.GetMethodParameters("EnableStatic");
                        i["IPAddress"] = new string[] { ip };
                        i["SubnetMask"] = new string[] { mask };
                        o = mo.InvokeMethod("EnableStatic", i, null);
                        if ((UInt32)o["returnValue"] != 0)
                            throw new TSAdapterSetException((UInt32)o["returnValue"]);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 重写的基类ToString方法
        /// </summary>
        /// <returns>适配器的名称</returns>
        public override string ToString()
        {
            return this.name;
        }
        
    }

//===================================================================================================
 
    /// <summary>
    /// TSAdapter设置异常类
    /// </summary>
    public class TSAdapterSetException : Exception
    {
        private UInt32 code;

        /// <summary>
        /// 错误码
        /// </summary>
        public UInt32 Code
        {
            get
            {
                return code;
            }
        }
        /// <summary>
        /// 错误描述
        /// </summary>
        new public string Source { get; protected set; }

        /// <summary>
        /// 生成一个TSAdapter设置异常
        /// </summary>
        /// <param name="c">WMI返回的错误码</param>
        public TSAdapterSetException(UInt32 c) :
            base("设置时发生错误！错误码：" + c.ToString())
        {
            code = c;
            switch (code)
            {
                case 1:
                    Source = "设置已完成，但是需要重启。（环境特殊？）";
                    break;
                case 65:
                    Source = "未知错误。";
                    break;
                case 66:
                    Source = "无效子网掩码。";
                    break;
                case 70:
                    Source = "无效IP地址。";
                    break;
                case 71:
                    Source = "无效网关地址。";
                    break;
                case 91:
                    Source = "拒绝访问。（需要管理员权限？）";
                    break;
                default:
                    Source = "尚未翻译，请进入http://msdn.microsoft.com/en-us/library/aa390383(v=vs.85).aspx查询错误码";
                    break;
            }
        }
    }
}
