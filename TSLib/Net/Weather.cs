using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace TSLib.Net
{
    /// <summary>
    /// 与天气有关的类
    /// </summary>
    public static partial class Weather
    {
        /// <summary>
        /// 通过百度API获取天气
        /// <para>http://lbsyun.baidu.com/apiconsole/key</para>
        /// </summary>
        /// <param name="city">城市名称</param>
        /// <param name="ak">开发者密钥</param>
        /// <returns>百度天气格式</returns>
        public static BDWeather GetBDWeather(string city, string ak)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create
                ("http://api.map.baidu.com/telematics/v3/weather?location=" + city + "&output=json&ak=" + ak);
            request.Timeout = 5000;
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            string jsonstr = sr.ReadLine();
            JavaScriptSerializer j = new JavaScriptSerializer();
            BDWeather weather = new BDWeather();
            weather = j.Deserialize<BDWeather>(jsonstr);

            return weather;
        }

        #region AQI（空气质量指数）相关函数
        /// <summary>
        /// 获取AQI报告
        /// <para>由pm25.in提供，采用国家HJ633-2012标准计算，AQI为24小时均值</para>
        /// </summary>
        /// <param name="city">城市名称，支持中文名（不含“市”）、拼音（不含“shi”）、区号</param>
        /// <returns>带有换行的AQI报告</returns>
        public static string GetAQIReport(string city)
        {
            //TODO:获取各个监测点报告

            string back = "";
            string html, allinfo;
            string[,] data = new string[9, 3];
            string[,] info = new string[4, 3];
            int q;
            html = Page.GetHtml("http://pm25.in/" + city);
            if (html == "error")
            {
                back += "网络错误！" + "\r\n";
                return back;
            }
            //常量
            allinfo = "城市,污染等级,更新时间,";
            q = 0;
            for (int i = 1; i <= 3; ++i)
            {
                do
                {
                    info[i, 1] += allinfo.Substring(q, 1);
                    ++q;
                } while (allinfo.Substring(q, 1) != ",");
                ++q;
            }

            //判断是否有该城市数据
            if (html.Substring(184, 1) == "P")
            {
                back += "对不起，该地区暂无环境监测数据！" + "\r\n";
                return back;
            }

            //获取info
            q = html.IndexOf("<h2>") + 4;
            do
            {
                info[1, 2] += html.Substring(q, 1);
                ++q;
            } while (html.Substring(q, 1) != "<");
            q += 99;
            do
            {
                info[2, 2] += html.Substring(q, 1);
                ++q;
            } while (html.Substring(q, 1) != " ");
            q += 172;
            do
            {
                info[3, 2] += html.Substring(q, 1);
                ++q;
            } while (html.Substring(q, 1) != "<");


            //获取alldata与data
            q += 294;
            for (int i = 1; i <= 8; ++i)
            {
                if ((i == 6) | (i == 7)) ++q;
                do
                {
                    data[i, 2] += html.Substring(q, 1);
                    ++q;
                } while (html.Substring(q, 1) != " ");
                q += 61;
                do
                {
                    data[i, 1] += html.Substring(q, 1);
                    ++q;
                } while (html.Substring(q, 1) != " ");
                q += 102;

            }

            //输出
            for (int i = 1; i <= 3; ++i)
            {
                back += info[i, 1] + ":" + info[i, 2] + "\r\n";
            }
            for (int i = 1; i <= 8; ++i)
            {
                back += data[i, 1] + ":" + data[i, 2] + "\r\n";
            }

            return back;
        }

        /// <summary>
        /// 获取AQI数据
        /// <para>由pm25.in提供，采用国家HJ633-2012标准计算，AQI为24小时均值</para>
        /// </summary>
        /// <param name="city">城市名称，支持中文名（不含“市”）、拼音（不含“shi”）、区号</param>
        /// <returns>AQI数据</returns>
        public static AQIData GetAQI(string city)
        {
            AQIData back = new AQIData();
            string html;
            string[] d = new string[9];
            int q;
            html = Page.GetHtml("http://pm25.in/" + city);
            //判断网络
            if (html == "error")
            {
                back.OK = false;
                return back;
            }
            //判断是否有该城市数据
            if (html.Substring(184, 1) == "P")
            {
                back.OK = false;
                return back;
            }

            //开始分析数据了。。。
            q = html.IndexOf("<h2>") + 4;
            do
            {
                back.City += html.Substring(q, 1);
                ++q;
            } while (html.Substring(q, 1) != "<");

            q += 99;
            do
            {
                back.Level += html.Substring(q, 1);
                ++q;
            } while (html.Substring(q, 1) != " ");

            q += 172;
            do
            {
                back.UpdateTime += html.Substring(q, 1);
                ++q;
            } while (html.Substring(q, 1) != "<");

            q += 294;
            for (int i = 1; i <= 8; ++i)
            {
                if ((i == 6) || (i == 7)) ++q;
                do
                {
                    d[i] += html.Substring(q, 1);
                    ++q;
                } while (html.Substring(q, 1) != " ");
                q += 61;
                do
                {
                    ++q;
                } while (html.Substring(q, 1) != " ");
                q += 102;
            }

            back.AQI = Convert.ToInt32(d[1]);
            back.PM25 = Convert.ToInt32(d[2]);
            back.PM10 = Convert.ToInt32(d[3]);
            back.CO = Convert.ToSingle(d[4]);
            back.NO2 = Convert.ToInt32(d[5]);
            back.O3 = Convert.ToInt32(d[6]);
            back.O3p8h = Convert.ToInt32(d[7]);
            back.SO2 = Convert.ToInt32(d[8]);

            return back;
        }
        #endregion
    }
}
