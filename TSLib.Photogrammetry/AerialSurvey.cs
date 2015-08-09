using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSLib.Photogrammetry
{
    public class AerialSurvey
    {
        /// <summary>
        /// 水平视场
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
        /// 重叠度
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

        public void Calculate()
        {
            coverageX = 2 * height * Math.Tan(FOV);
            coverageY = 1 / aspectRatio * coverageX;
            area = coverageX * coverageY;
            offsetX = coverageX * Math.Sqrt(1 - overlap);
            offsetY = coverageY * Math.Sqrt(1 - overlap);
        }
    }
}
