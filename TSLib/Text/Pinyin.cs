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
            str = str.ToLower();
            string[] strs = StringToArray(str, " ");
            string r = "";
            XElement root = XElement.Load(@"Text\PinyinTable.xml");
            foreach (string s in strs)
            {
                var query =
                    from el in root.Elements("aPinyin")
                    where (string)el.Element(pfrom.ToString()) == s
                    select el;
                foreach (XElement el in query)
                    r += (string)el.Element(pto.ToString()) + " ";
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
