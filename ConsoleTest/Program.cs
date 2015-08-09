using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSLib.Net;
using TSLib.Text;
using TSLib.Photogrammetry;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "TSLib 测试";

            Camera c = new Camera(Camera.ImageSensorTyprEnum.Full, 50, "HaHa");
            Console.WriteLine(c.GetFOV());

            Console.ReadKey();
        }
    }
}
