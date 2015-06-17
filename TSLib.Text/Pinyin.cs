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
using System.IO;
using System.Reflection;

namespace TSLib.Text
{
    public class Pinyin
    {
        public enum Pinyins
        {
            Zhuyin,
            Weituoma,
            Zhuyin2,
            Yelu,
            FaguoYuandong,
            Deguo,
            HuayuTongyong,
            Pinyin,
            LuomaYin,
            LuomaYang,
            LuomaShang,
            LuomaQu
        }

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
            return Convert(str, Pinyins.Pinyin, Pinyins.Zhuyin);
        }

        public static string Convert(string str, Pinyins pfrom, Pinyins pto)
        {
            Stream sm = Assembly.GetExecutingAssembly().GetManifestResourceStream("TSLib.Text.PinyinTable.xml");
            str = str.ToLower();
            string[] strs = StringToArray(str, " ");
            string r = "";
            XElement root = XElement.Load(sm);
            int i = 0;
            foreach (string s in strs)
            {
                if (s == "\r\n")
                {
                    if (r.Substring(r.Length - 1, 1) != "\n")
                        r = r.Substring(0, r.Length - 1);
                    r += "\r\n";
                }
                else
                {
                    var query =
                        from el in root.Elements("aPinyin")
                        where (string)el.Element(pfrom.ToString()) == s
                        select el;
                    if (query.Count() == 0)
                        r += "[" + s + "] ";
                    else
                    {
                        XElement el = query.ElementAt(0);
                        r += (string)el.Element(pto.ToString()) + " ";
                    }
                }
                i++;
            }
            return r;
        }

        public static string[] StringToArray(string str, string splitChar)
        {
            if (str == null) return null;

            int i = 0;
            string t = "";
            List<string> list = new List<string>();
            while (i < str.Length)
            {
                if (str[i] == '\r')
                {
                    if (t != "")
                        list.Add(t);
                    t = "";
                    list.Add("\r\n");
                    i += 2;
                }
                else if (str[i] == ' ')
                {
                    if (t != "")
                        list.Add(t);
                    t = "";
                    i++;
                }
                else
                {
                    t += str[i];
                    i++;
                }
            }

            if (t != "")
                list.Add(t);

            return list.ToArray();

            //return Regex.Split(str, splitChar, RegexOptions.IgnoreCase).Where<string>(i => i != "").ToArray();
        }
    }
}
