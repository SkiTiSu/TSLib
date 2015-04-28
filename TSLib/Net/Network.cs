using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TSLib.Net
{
    public static class Network
    {
        [DllImport("wininet")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        /// <summary>
        /// 检查是否联网
        /// </summary>
        /// <returns>是否联网</returns>
        public static bool CheckNet()
        {
            int i = 0;
            if (InternetGetConnectedState(out i, 0))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
