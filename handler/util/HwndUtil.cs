using System;
using System.Runtime.InteropServices;

namespace handler.util
{
    class HwndUtil
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        public static void clickHwnd(IntPtr hWnd)
        {
            SendMessage(hWnd, 0xF5, IntPtr.Zero, null);
        }
    }
}
