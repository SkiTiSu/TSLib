using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TSLib.Port
{
    /// <summary>
    /// U盘监控类
    /// <para>由于技术限制，此类必须继承于Form，导致成员繁杂，请使用对象浏览器查看成员</para>
    /// </summary>
    public partial class UDWatcher : Form
    {
        /// <summary>
        /// 初始化类
        /// <para>销毁请用Close()方法</para>
        /// </summary>
        public UDWatcher()
        {
            InitializeComponent();
            this.Show();
            this.Hide();
        }

        /// <summary>
        /// U盘监控类处理方法
        /// </summary>
        /// <param name="ID">带有":"的盘符</param>
        public delegate void DeviceEventHandler(string ID);

        /// <summary>
        /// U盘已经可以使用
        /// </summary>
        public event DeviceEventHandler DeviceArrial;
        /// <summary>
        /// U盘已经弹出
        /// </summary>
        public event DeviceEventHandler DeviceRemoveComplete;

        #region WndProc常量
        internal const int WM_DEVICECHANGE = 0x219;//U盘插入后，OS的底层会自动检测到，然后向应用程序发送“硬件设备状态改变“的消息
        internal const int DBT_DEVICEARRIVAL = 0x8000;  //就是用来表示U盘可用的。一个设备或媒体已被插入一块，现在可用。
        internal const int DBT_CONFIGCHANGECANCELED = 0x0019;  //要求更改当前的配置（或取消停靠码头）已被取消。
        internal const int DBT_CONFIGCHANGED = 0x0018;  //当前的配置发生了变化，由于码头或取消固定。
        internal const int DBT_CUSTOMEVENT = 0x8006; //自定义的事件发生。 的Windows NT 4.0和Windows 95：此值不支持。
        internal const int DBT_DEVICEQUERYREMOVE = 0x8001;  //审批要求删除一个设备或媒体作品。任何应用程序也不能否认这一要求，并取消删除。
        internal const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;  //请求删除一个设备或媒体片已被取消。
        internal const int DBT_DEVICEREMOVECOMPLETE = 0x8004;  //一个设备或媒体片已被删除。
        internal const int DBT_DEVICEREMOVEPENDING = 0x8003;  //一个设备或媒体一块即将被删除。不能否认的。
        internal const int DBT_DEVICETYPESPECIFIC = 0x8005;  //一个设备特定事件发生。
        internal const int DBT_DEVNODES_CHANGED = 0x0007;  //一种设备已被添加到或从系统中删除。
        internal const int DBT_QUERYCHANGECONFIG = 0x0017;  //许可是要求改变目前的配置（码头或取消固定）。
        internal const int DBT_USERDEFINED = 0xFFFF;  //此消息的含义是用户定义的
        internal const int DBT_DEVTYP_VOLUME = 0x00000002;  //逻辑卷标
        #endregion

        internal string ID = "";
        internal string Value;

        [StructLayout(LayoutKind.Sequential)]
        internal struct DEV_BROADCAST_VOLUME
        {
            public int dbcv_size;
            public int dbcv_devicetype;
            public int dbcv_reserved;
            public int dbcv_unitmask;
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == WM_DEVICECHANGE)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case WM_DEVICECHANGE:
                            break;
                        case DBT_DEVICEARRIVAL:
                            DriveInfo[] s = DriveInfo.GetDrives();
                            foreach (DriveInfo DriveI in s)
                            {
                                int devType = Marshal.ReadInt32(m.LParam, 4);
                                if (devType == DBT_DEVTYP_VOLUME)
                                {
                                    DEV_BROADCAST_VOLUME vol;
                                    vol = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                                    ID = vol.dbcv_unitmask.ToString("x");
                                    DeviceArrial(IO(ID));
                                    break;
                                }
                            }
                            break;
                        case DBT_CONFIGCHANGECANCELED:
                            break;
                        case DBT_CONFIGCHANGED:
                            break;
                        case DBT_CUSTOMEVENT:
                            break;
                        case DBT_DEVICEQUERYREMOVE:
                            break;
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                            DriveInfo[] I = DriveInfo.GetDrives();
                            foreach (DriveInfo DrInfo in I)
                            {
                                int devType = Marshal.ReadInt32(m.LParam, 4);
                                if (devType == DBT_DEVTYP_VOLUME)
                                {
                                    DEV_BROADCAST_VOLUME vol;
                                    vol = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                                    ID = vol.dbcv_unitmask.ToString("x");
                                    DeviceRemoveComplete(IO(ID));
                                    break;
                                }
                            }
                            MessageBox.Show(this.Text);
                            break;
                        case DBT_DEVICEREMOVEPENDING:
                            break;
                        case DBT_DEVICETYPESPECIFIC:
                            break;
                        case DBT_DEVNODES_CHANGED:
                            break;
                        case DBT_QUERYCHANGECONFIG:
                            break;
                        case DBT_USERDEFINED:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            base.WndProc(ref m);
        }
        internal string IO(string ff)
        {
            switch (ff)
            {
                case "1":
                    Value = "A:";
                    break;
                case "2":
                    Value = "B:";
                    break;
                case "4":
                    Value = "C:";
                    break;
                case "8":
                    Value = "D:";
                    break;
                case "10":
                    Value = "E:";
                    break;
                case "20":
                    Value = "F:";
                    break;
                case "40":
                    Value = "G:";
                    break;
                case "80":
                    Value = "H:";
                    break;
                case "100":
                    Value = "I:";
                    break;
                case "200":
                    Value = "J:";
                    break;
                case "400":
                    Value = "K:";
                    break;
                case "800":
                    Value = "L:";
                    break;
                case "1000":
                    Value = "M:";
                    break;
                case "2000":
                    Value = "N:";
                    break;
                case "4000":
                    Value = "O:";
                    break;
                case "8000":
                    Value = "P:";
                    break;
                case "10000":
                    Value = "Q:";
                    break;
                case "20000":
                    Value = "R:";
                    break;
                case "40000":
                    Value = "S:";
                    break;
                case "80000":
                    Value = "T:";
                    break;
                case "100000":
                    Value = "U:";
                    break;
                case "200000":
                    Value = "V:";
                    break;
                case "400000":
                    Value = "W:";
                    break;
                case "800000":
                    Value = "X:";
                    break;
                case "1000000":
                    Value = "Y:";
                    break;
                case "2000000":
                    Value = "Z:";
                    break;
                default: break;
            }
            return Value;
        }
    }
}
