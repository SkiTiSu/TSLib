using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TSLib.Net;
using System.Web;
using System.Web.Extensions;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using TSLib.Port;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = TSLib.Net.Weather.GetAQIReport("nanjing");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = TSAdapter.GetAdaptersReport();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<TSAdapter> Adps = TSAdapter.GetAdapters();
            foreach (TSAdapter adp in Adps)
            {
                if (adp.Name == "WLAN 2")
                {
                    try
                    {
                        adp.DNS = new string[] { "8.8.8.8" };
                        textBox1.Text = "OK";
                    }
                    catch (TSAdapterSetException ee)
                    {
                        MessageBox.Show("设置时发生错误：" + ee.Source,
                            "TSLib");
                    }
                    break;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TSSerialPort p = (TSSerialPort)comboBox.SelectedItem;
            p.Open();
            p.SerialArrived += p_SerialArrived;
        }

        void p_SerialArrived(string s)
        {
            textBox1.Text += s;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //PublicIP ip = new PublicIP();
            //textBox1.Text = ip.IP + ip.Location + ip.ISP;
            TSDownload tsdd = new TSDownload("http://xiazai.xiazaiba.com/Soft/Q/QQ2014_5.5(11447)_XiaZaiBa.exe");
            tsdd.Progressbar = progressBar1;
            tsdd.Start();
            textBox1.Text = "开始下载？";
            
        }

        TSDownload tsd = new TSDownload("http://xiazai.xiazaiba.com/Soft/P/ProcessLasso_6.8.0.6_x86_XiaZaiBa.zip");
        private void button6_Click(object sender, EventArgs e)
        {
            tsd.Progressbar = progressBar1;
            tsd.Start();
            textBox1.Text = "开始下载？";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = String.Format("已下载{0:f2}%，{1:f2}MB/{2:f2}MB，速度{3}kB/s", tsd.Percent, (float)tsd.DownloadedBytes/1024/1024, (float)tsd.TotalBytes/1024/1024, tsd.Speed);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string city = TSLib.Net.PublicIP.GetCity();
            string ak = "请修改此处！";
            Weather.BDWeather weather = Weather.GetBDWeather(city, ak);
            textBox1.Text += "您所在的城市：";
            textBox1.Text += weather.results[0].currentCity + "\r\n";
            textBox1.Text += weather.results[0].weather_data[0].date + "\r\n";
            textBox1.Text += weather.results[0].weather_data[0].weather + "\r\n";
            textBox1.Text += weather.results[0].weather_data[0].wind;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<TSSerialPort> ss = TSSerialPort.GetPorts();
            foreach (TSSerialPort s in ss)
            {
                comboBox.Items.Add(s);
            }
            if (comboBox.Items.Count != 0)
                comboBox.SelectedIndex = 0;

            CheckForIllegalCrossThreadCalls = false;
        }
    }
}
