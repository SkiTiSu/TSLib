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

            while (true)
            {
                Console.Write("输入简体中文>");
                string s = Console.ReadLine();
                //Console.WriteLine(s);
                string ss = Pinyin.GetPinyin(s);
                Console.WriteLine(ss);
                Console.WriteLine(TSLib.Text.Pinyin.ToZhuyin(ss));
            }

            Console.ReadKey();
        }
    }
}
