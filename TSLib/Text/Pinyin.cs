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
    }
}
