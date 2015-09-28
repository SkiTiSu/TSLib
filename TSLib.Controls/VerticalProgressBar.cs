using System.Windows.Forms;

namespace TSLib.Controls
{
    /// <summary>
    /// 垂直进度条
    /// </summary>
    public partial class VerticalProgressBar : ProgressBar
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x04;
                return cp;
            }
        }

    }
}