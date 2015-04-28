using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace TSLib.Net
{
    /// <summary>
    /// 与网页有关的类
    /// </summary>
    public static class Page
    {
        /// <summary>
        /// 获取一个网页的源代码
        /// <para>使用UTF-8编码</para>
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>该网页的源代码</returns>
        public static string GetHtml(string url)
        {
            try
            {
                WebClient myWebClient = new WebClient();
                byte[] myDataBuffer = myWebClient.DownloadData(url);
                return Encoding.UTF8.GetString(myDataBuffer);
            }
            catch
            {
                return "网络错误！";
            }
        }

        /// <summary>
        /// 获取一个网页的源代码
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="e">
        /// 编码方式
        /// <para>如为其它编码，使用类似Encoding.GetEncoding("GBK")的语句</para>
        /// </param>
        /// <returns>该网页的源代码</returns>
        public static string GetHtml(string url, Encoding e)
        {
            try
            {
                WebClient myWebClient = new WebClient();
                byte[] myDataBuffer = myWebClient.DownloadData(url);
                return e.GetString(myDataBuffer);
            }
            catch
            {
                return "网络错误！";
            }
        }
    }
}
