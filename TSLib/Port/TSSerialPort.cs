using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace TSLib.Port
{
    /// <summary>
    /// 天书串口类
    /// </summary>
    public class TSSerialPort
    {
        public TSSerialPort()
        {
            Baud = 19200;
            IsLine = true;
            IsOpen = false;
        }

        /// <summary>
        /// 串口号
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Des { get; protected set; }
        /// <summary>
        /// 制造商
        /// </summary>
        public string Manu { get; protected set; }
        /// <summary>
        /// 波特率（需手动设置）
        /// </summary>
        public int Baud
        {
            get
            {
                return baud;
            }
            set
            {
                if (port != null)
                {
                    port.BaudRate = value;
                }
               baud = value;
            }
        }

        private int baud;
        /// <summary>
        /// 是否按行读取
        /// </summary>
        public bool IsLine { get; set; }

        /// <summary>
        /// 是否打开
        /// </summary>
        public bool IsOpen { get; set; }

        public override string ToString()
        {
            return Name + " - " + Des;
        }

        private SerialPort port = null;

        /// <summary>
        /// 打开串口
        /// </summary>
        public void Open()
        {
            try
            {
                string portName = Name;
                port = new SerialPort(portName, Baud);
                port.Encoding = Encoding.UTF8;
                port.DataReceived += port_DataReceived;
                port.Open();
                IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化串口发生错误：" + ex.Message, "TSLib - TSSerialPort", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (SerialArrived != null)
            {
                if (IsLine)
                {
                    SerialArrived(ReadLineSerialData());
                }
                else
                {
                    SerialArrived(ReadSerialData());
                }
            }

        }

        /// <summary>
        /// 从串口按行读取数据并转换为字符串形式
        /// </summary>
        /// <returns>串口数据</returns>
        private string ReadLineSerialData()
        {
            string value = "";
            try
            {
                value = port.ReadLine() + "\n";
            }
            catch (TimeoutException) { }
            return value;
        }

        /// <summary>
        /// 从串口读取数据并转换为字符串形式
        /// </summary>
        /// <returns>串口数据</returns>
        private string ReadSerialData()
        {
            string value = "";
            try
            {
                if (port != null && port.BytesToRead > 0)
                {
                    value = port.ReadExisting();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取串口数据发生错误：" + ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return value;
        }


        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Close()
        {
            if (port != null)
            {
                try
                {
                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                    port.Dispose();
                    IsOpen = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("关闭串口发生错误：" + ex.Message, "TSLib - TSSerialPort", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        
        /// <summary>
        /// 带有换行符的写入
        /// </summary>
        /// <param name="s">写入的内容</param>
        public void Writeline(string s)
        {
            if (port != null && port.IsOpen)
            {
                port.WriteLine(s);
            }
        }

        /// <summary>
        /// 不带换行符的写入
        /// </summary>
        /// <param name="s">写入的内容</param>
        public void Write(string s)
        {
            if (port != null && port.IsOpen)
            {
                port.Write(s);
            }
        }

        public delegate void SerialArrivedEventHandler(string s);

        /// <summary>
        /// 当串口有新数据时返回
        /// <para>注：IsLine==true时将自动在\r后加上\n换行</para>
        /// </summary>
        public event SerialArrivedEventHandler SerialArrived;


        //TODO:接受到数据(DataReceived)后通过事件返回收到的值

        /// <summary>
        /// 获取串口们
        /// </summary>
        /// <returns>天书串口类型</returns>
        public static List<TSSerialPort> GetPorts()
        {
            List<TSSerialPort> back = new List<TSSerialPort>();

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity"))
            {
                var hardInfos = searcher.Get();
                foreach (var hardInfo in hardInfos)
                {
                    if ((hardInfo.Properties["Name"].Value != null) &&
                        (hardInfo.Properties["Name"].Value.ToString().Contains("COM")))
                    {
                        TSSerialPort temp = new TSSerialPort();
                        string s = hardInfo.Properties["Name"].Value.ToString();
                        int p = s.IndexOf('(');
                        temp.Des = s.Substring(0, p);
                        temp.Name = s.Substring(p + 1, s.Length - p - 2);
                        temp.Manu = hardInfo.Properties["Manufacturer"].Value.ToString();
                        back.Add(temp);
                    }
                }
                searcher.Dispose();
            }

            return back;
        }
    }
}