using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSLib.Net;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "TSLib 测试";
            if (TSLib.Consoles.Passwd.VerifyPasswd("123456", "budui", 3) == false)
                Console.WriteLine("sb");
            Console.WriteLine(TSLib.Text.Pinyin.GetPinyin("哈哈"));
            Console.ReadKey();
        }
    }
}
