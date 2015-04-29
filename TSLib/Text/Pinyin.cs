/* 
 * 本类依赖的微软全球化包可以在以下地址获取
 * http://www.microsoft.com/zh-cn/download/details.aspx?id=15251
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace TSLib.Text
{
    public class Pinyin
    {
        public static string GetPinyin(string str)
        {
            string r = string.Empty;
            foreach (char obj in str)
            {
                try
                {
                    ChineseChar chineseChar = new ChineseChar(obj);
                    string t = chineseChar.Pinyins[0].ToLower();
                    r += t.Substring(0, t.Length - 1) + " ";
                }
                catch
                {
                    r += obj.ToString();
                }
            }
            return r;
        }

        public static string ToZhuyin(string str)
        {
            str = str.ToLower();
            string[] strs = StringToArray(str, " ");
            string r = "";
            XElement root = XElement.Load(@"Text\PinyinTable.xml");
            foreach (string s in strs)
            {
                var query =
                    from el in root.Elements("Main")
                    where (string)el.Element("Pinyin") == s
                    select el;
                foreach (XElement el in query)
                    r += (string)el.Element("Zhuyin") + " ";
            }
            return r;
        }

        public static string[] StringToArray(string str, string splitChar)
        {
            if (str == null) return null;
            return Regex.Split(str, splitChar, RegexOptions.IgnoreCase).Where<string>(i => i != "").ToArray();
        }
    }
}
