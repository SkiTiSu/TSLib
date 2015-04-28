using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSLib.Consoles
{
    public class Passwd
    {
        /// <summary>
        /// 无限循环验证密码
        /// </summary>
        /// <param name="passwd">密码</param>
        public static void VerifyPasswd(string passwd)
        {
            VerifyPasswd(passwd, "[INFO]密码错误，请重新输入");
        }

        /// <summary>
        /// 无限循环验证密码
        /// </summary>
        /// <param name="passwd">密码</param>
        /// <param name="errorinfo">输错提示</param>
        public static void VerifyPasswd(string passwd, string errorinfo)
        {
            while (true)
            {
                if (EnterPasswd() == passwd)
                    break;
                else
                    Console.WriteLine(errorinfo);
            }
        }

        /// <summary>
        /// 有限次数验证密码
        /// </summary>
        /// <param name="passwd">密码</param>
        /// <param name="errorinfo">输错提示</param>
        /// <param name="retries">重试次数</param>
        /// <returns>在重试次数内密码是否正确</returns>
        public static bool VerifyPasswd(string passwd, string errorinfo, int retries)
        {
            for (int i = 1; i <= retries; i++)
            {
                if (EnterPasswd() == passwd)
                    return true;
                else
                    Console.WriteLine(errorinfo);
            }
            return false;
        }

        /// <summary>
        /// 带星号和退格的密码输入功能
        /// </summary>
        /// <returns>输入的密码</returns>
        public static string EnterPasswd()
        {
            while (true)
            {
                Console.Write("请输入密码>");
                string key = string.Empty;
                while (true)
                {
                    ConsoleKeyInfo keyinfo = Console.ReadKey(true);
                    if (keyinfo.Key == ConsoleKey.Enter) //按下回车，结束
                        break;
                    else if (keyinfo.Key == ConsoleKey.Backspace && key.Length > 0) //如果是退格键并且字符没有删光
                    {
                        Console.Write("\b \b"); //输出一个退格（此时光标向左走了一位），然后输出一个空格取代最后一个星号，然后再往前走一位，也就是说其实后面有一个空格但是你看不见= =
                        key = key.Substring(0, key.Length - 1);
                    }

                    else if (!char.IsControl(keyinfo.KeyChar)) //过滤掉功能按键等
                    {
                        key += keyinfo.KeyChar.ToString();
                        Console.Write("*");
                    }
                }
                Console.WriteLine();
                return key;
            }
        }
    }
}
