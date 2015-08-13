using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSLib.Photogrammetry
{
    public class AerialSurvey
    {
        /// <summary>
        /// 相机
        /// </summary>
        Camera camera;
        /// <summary>
        /// 水平视场（°）
        /// </summary>
        double FOV;
        /// <summary>
        /// 飞行高度
        /// </summary>
        double height;
        /// <summary>
        /// 图像传感器长宽比
        /// </summary>
        double aspectRatio;
        /// <summary>
        /// 重叠度（%）
        /// </summary>
        double overlap;
        /// <summary>
        /// 横向每张照片的偏移
        /// </summary>
        double offsetX;
        /// <summary>
        /// 纵向每张照片的偏移
        /// </summary>
        double offsetY;
        /// <summary>
        /// 单张覆盖长度
        /// </summary>
        double coverageX;
        /// <summary>
        /// 单张覆盖宽度
        /// </summary>
        double coverageY;
        /// <summary>
        /// 单张覆盖面积
        /// </summary>
        double area;

        double GSD;
        
        public void setup(Camera cam,double height,double overlap)
        {
            SetFromCamera(cam);
            this.height = height;
            this.overlap = overlap;
            Calculate();
        }

        public void SetFromCamera(Camera cam)
        {
            camera = cam;
            FOV = camera.GetFOV();
            aspectRatio = camera.Sensor.AspectRatio;
        }

        public void Calculate()
        {
            coverageX = 2 * height * Math.Tan(FOV / 2 * Math.PI / 180);
            coverageY = 1 / aspectRatio * coverageX;
            area = coverageX * coverageY;
            offsetX = coverageX * Math.Sqrt(1 - overlap);
            offsetY = coverageY * Math.Sqrt(1 - overlap);
            GSD = Math.Sqrt((area * 10000) / camera.Pixels);
        }

        string s = "";
        public string GetReport()
        {
            s = "";
            s += "本类正在测试中，所生成数据不具可参考性哦~\r\n";
            s += "==相机信息================================\r\n";
            sAdd("名称", camera.Name);
            sAdd("传感器类型", camera.Sensor.ImageSensorType.ToString());
            sAdd("焦距", camera.Focal,"mm");
            sAdd("水平FOV", FOV,"°");
            sAdd("长宽比", aspectRatio);
            s += "==航线信息================================\r\n";
            sAdd("高度", height,"m");
            sAdd("重叠度", overlap*100,"%");
            s += "每张照片的偏移：横向" + offsetX + "m / 纵向" + offsetY + "m\r\n";
            s += "单张覆盖：" + coverageX + "m * " + coverageY + "m = " + area + "m²" + "\r\n";
            sAdd("理想GSD", GSD, "cm");
            return s;
        }

        public void sAdd(string a,string b,string c = "")
        {
            s += a + "：" + b + c + "\r\n";
        }

        public void sAdd(string a, double b, string c = "")
        {
            s += a + "：" + b + c + "\r\n";
        }
    }
}
