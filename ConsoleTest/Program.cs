using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSLib.Net;
using TSLib.Text;
using TSLib.Photo;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "TSLib 测试";

            exiftest();

            Console.ReadKey();
        }

        static void cameratest()
        {
            Camera c = new Camera(Camera.ImageSensorTyprEnum.Full, 25, "5D2");
            c.Pixels = 5616 * 3744;
            //Console.WriteLine(c.GetFOV());

            AerialSurvey ars = new AerialSurvey();
            ars.setup(c, 100, 0.5);
            Console.WriteLine(ars.GetReport());
        }

        static void exiftest()
        {
            Encoding ascii = Encoding.ASCII;

            //			System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(
            //				"F:\\webdev\\raw.images.pk\\saripaya\\lonely Tree - Paey.jpg"
            //				);
            //			Goheer.EXIF.EXIFextractor er = new Goheer.EXIF.EXIFextractor(ref bmp,"\n");
            //			foreach(System.Web.UI.Pair s in er )
            //			{
            //				Console.Write(s.First+" : " + s.Second +"\n");
            //			}
            //			if (er["Image Title"] == null)
            //				er.setTag(0x320, "http://www.beautifulpakistan.com");
            //			if (er["Artist"] == null)
            //				er.setTag(0x13B, "http://www.beautifulpakistan.com");
            //			if (er["User Comment"] == null)
            //				er.setTag(0x9286, "http://www.beautifulpakistan.com");
            //			if (er["Copyright"] == null)
            //				er.setTag(0x8298, "http://www.beautifulpakistan.com");

            EXIF.EXIFextractor er2 = new EXIF.EXIFextractor(@"D:\DSC07501.JPG", "", "");

            foreach (KeyValuePair<string,string> s in er2)
            {
                Console.WriteLine(s.Key + " : " + s.Value);
            }
            Console.WriteLine(er2["User Comment"]);
        }
    }
}
