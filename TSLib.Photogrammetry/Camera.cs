using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSLib.Photogrammetry
{
    /// <summary>
    /// 摄影测量-相机相关的计算
    /// </summary>
    public class Camera
    {
        public Camera(ImageSensorTyprEnum sensorEnum, int f, string name = "Default")
        {
            Name = name;
            Sensor = new ImageSensor(sensorEnum);
            Focal = f;
        }

        public Camera(int length, int width, int f, string name = "Default")
        {
            Name = name;
            Sensor = new ImageSensor(length, width);
            Focal = f;
        }

        public string Name { set; get; }
        public ImageSensor Sensor { set; get; }
        public int Focal { get; set; }
        
        //TODO:重整逻辑，只获取长度和宽度，另外对角线长度

        /// <summary>
        /// 获取图像传感器对角线长度
        /// </summary>
        /// <param name="s">图像传感器画幅</param>
        /// <returns>图像传感器对角线长度(mm)</returns>
        public static double GetSensorDiagonal(ImageSensorTyprEnum s)
        {
            double r = 0.0;
            switch (s)
            {
                case ImageSensorTyprEnum.Full:
                    r = 43.27;
                    break;
                case ImageSensorTyprEnum.CannonC:
                    r = 26.68;
                    break;
                case ImageSensorTyprEnum.APSC:
                    r = 28.4;
                    break;
                default:
                    r = 43.27;
                    break;
            }
            return r;
        }

        /// <summary>
        /// 获取水平视场
        /// </summary>
        /// <param name="f">焦距</param>
        /// <param name="d">传感器对角线长度</param>
        /// <returns>水平视场(角度)</returns>
        public static double GetFOV(int f, double d)
        {
            return (2 * Math.Atan(d / (2 * f))) * 180 / Math.PI;
        }

        public double GetFOV()
        {
            return (2 * Math.Atan(Sensor.Diagonal / (2 * Focal))) * 180 / Math.PI;
        }

        /// <summary>
        /// 获取水平视场
        /// </summary>
        /// <param name="f">焦距</param>
        /// <param name="s">图像传感器画幅</param>
        /// <returns>水平视场(角度)</returns>
        public static double GetFOV(int f, ImageSensorTyprEnum s)
        {
            return GetFOV(f, GetSensorDiagonal(s));
        }

        #region ImageSensor
        /// <summary>
        /// 图像传感器画幅
        /// </summary>
        public enum ImageSensorTyprEnum
        {
            /// <summary>
            /// 全画幅
            /// </summary>
            Full,
            /// <summary>
            /// 佳能APS-C画幅
            /// </summary>
            CannonC,
            /// <summary>
            /// APS-C画幅
            /// </summary>
            APSC,
            M4d3,
            /// <summary>
            /// 其它（注意，默认获取数值为全画幅）
            /// </summary>
            Other
        }

        /// <summary>
        /// 图像传感器类
        /// </summary>
        public class ImageSensor
        {
            public ImageSensor(ImageSensorTyprEnum s)
            {
                ImageSensorType = s;
            }

            public ImageSensor(int length, int width)
            {
                Length = length;
                Width = width;
            }

            public ImageSensorTyprEnum ImageSensorType
            {
                get
                {
                    return imageSensorType;
                }
                set
                {
                    switch (value)
                    {
                        case ImageSensorTyprEnum.Full:
                            Length = 36;
                            Width = 24;
                            break;
                        case ImageSensorTyprEnum.CannonC:
                            Length = 22.2;
                            Width = 14.8;
                            break;
                        case ImageSensorTyprEnum.APSC:
                            Length = 23.6;
                            Width = 15.8;
                            break;
                        default:
                            Length = 36;
                            Width = 24;
                            break;
                    }
                    imageSensorType = value;
                }
            }
            ImageSensorTyprEnum imageSensorType;

            public double Length { get; set; }
            public double Width { get; set; }
            public double Diagonal
            {
                get
                {
                    return Math.Sqrt(Length * Length + Width * Width);
                }
            }
            public double AspectRatio
            {
                get
                {
                    return Length / Width;
                }
            }
        }
        #endregion
    }


}
