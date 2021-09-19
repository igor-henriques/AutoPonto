using System;
using System.Runtime.InteropServices;

namespace AutoPonto.Models
{
    public class Alert
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type = 0);
    }
}
