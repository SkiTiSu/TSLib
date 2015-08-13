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

            Camera c = new Camera(Camera.ImageSensorTyprEnum.Full, 25, "5D2");
            c.Pixels = 5616*3744;
            //Console.WriteLine(c.GetFOV());

            AerialSurvey ars = new AerialSurvey();
            ars.setup(c, 100, 0.5);
            Console.WriteLine(ars.GetReport());

            Console.ReadKey();
        }
    }
}
