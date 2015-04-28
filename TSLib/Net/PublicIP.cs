using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSLib.Net
{
    /// <summary>
    /// 公网IP相关类
    /// <para>使用ip138数据</para>
    /// </summary>
    public class PublicIP
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; protected set; }
        /// <summary>
        /// IP归属地
        /// </summary>
        public string Location { get; protected set; }
        /// <summary>
        /// 运营商
        /// </summary>
        public string ISP { get; protected set; }
        /// <summary>
        /// 城市（暂时仅支持中国大陆部分城市）
        /// </summary>
        public string City { get; protected set; }

        public PublicIP()
        {
            Fresh();
        }

        /// <summary>
        /// 刷新所有属性
        /// </summary>
        public void Fresh()
        {
            string html = Page.GetHtml("http://20140507.ip138.com/ic.asp", Encoding.GetEncoding("GBK"));
            int i = html.IndexOf('[') + 1;

            IP = "";
            do
            {
                IP += html.Substring(i, 1);
                ++i;
            } while (html.Substring(i, 1) != "]");

            i += 5;
            Location = "";
            do
            {
                Location += html.Substring(i, 1);
                ++i;
            } while (html.Substring(i, 1) != " ");

            i += 1;
            ISP = "";
            do
            {
                ISP += html.Substring(i, 1);
                ++i;
            } while (html.Substring(i, 1) != "<");

            City = "";
            int p = Location.IndexOf("省");
            if (p == 0)
            {
                City = Location.Substring(0, Location.Length - 1);
            }
            else
            {
                City = Location.Substring(p + 1, Location.Length - p - 2);
            }
        }

        #region 静态方法
        /// <summary>
        /// 获取公网IP地址
        /// </summary>
        /// <returns>公网IP地址</returns>
        public static string GetIP()
        {
            string html = Page.GetHtml("http://20140507.ip138.com/ic.asp", Encoding.GetEncoding("GBK"));
            string back = "";
            int i = html.IndexOf('[') + 1;
            do
            {
                back += html.Substring(i, 1);
                ++i;
            } while (html.Substring(i, 1) != "]");
            return back;
        }

        /// <summary>
        /// 获取IP归属地
        /// </summary>
        /// <returns>IP归属地</returns>
        public static string GetLocation()
        {
            string html = Page.GetHtml("http://20140507.ip138.com/ic.asp", Encoding.GetEncoding("GBK"));
            string back = "";
            int i = html.IndexOf("自：") + 2;
            do
            {
                back += html.Substring(i, 1);
                ++i;
            } while (html.Substring(i, 1) != " ");
            return back;
        }

        /// <summary>
        /// 获取所在市
        /// <para>暂仅支持中国大陆大部分地区</para>
        /// </summary>
        /// <returns>所在市（不含"市"）</returns>
        public static string GetCity()
        {
            string back = GetLocation();
            
            int p = back.IndexOf("省");
            if (p == 0)
            {
                return back.Substring(0, back.Length - 1);
            }
            else
            {
                return back.Substring(p + 1, back.Length - p - 2);
            }
        }

        /// <summary>
        /// 获取运营商
        /// </summary>
        /// <returns>运营商</returns>
        public static string GetISP()
        {
            string html = Page.GetHtml("http://20140507.ip138.com/ic.asp", Encoding.GetEncoding("GBK"));
            int i = html.IndexOf("</c");
            int t = i;
            do
            {
                --t;
            } while (html.Substring(t, 1) != " ");
            return html.Substring(t + 1, i - t - 1);
        }
        #endregion

        //TODO:添加百度接口
    }
}
