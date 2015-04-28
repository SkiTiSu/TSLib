using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace TSLib.Local
{
    /// <summary>
    /// 屏幕截图类
    /// </summary>
    public class Screenshot
    {
        /// <summary>
        /// 获取完整屏幕截图
        /// </summary>
        /// <returns>完整的屏幕截图</returns>
        public static Bitmap GetFullScreen()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;
            Bitmap bmp = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(
                new Point(0, 0),
                new Point(0, 0),
                new Size(w, h)
                );
            return bmp;
        }

        /// <summary>
        /// 获取屏幕部分截图
        /// </summary>
        /// <param name="x">起始点X坐标</param>
        /// <param name="y">起始点Y坐标</param>
        /// <param name="w">区域宽度</param>
        /// <param name="h">区域高度</param>
        /// <returns></returns>
        public static Bitmap GetPartScreen(int x, int y, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(
                new Point(0, 0),
                new Point(x, y),
                new Size(w, h)
                );
            return bmp;
        }

        //TODO:更多截图相关功能
    }
}
