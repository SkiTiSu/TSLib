using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSLib.Net;
using TSLib.Text;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "TSLib 测试";

            string s = "我是四季天书";
            Console.WriteLine(s);
            string ss = Pinyin.GetPinyin(s);
            Console.WriteLine(ss);
            Console.WriteLine(TSLib.Text.Pinyin.ToZhuyin(ss));

            Console.ReadKey();
        }
    }
}
