using robot.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace robot.core
{
    class ComCore
    {
        public static void ReMake()
        {
            HanderNat();
            HanderModem();
        }

        private static void HanderNat()
        {
            IntPtr hwnd = HwndUtil.FindWindow("#32770", "Windows - 系统错误");
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
            }
        }

        private static void HanderModem()
        {
            IntPtr hwnd = HwndUtil.FindWindow("#32770", "Windows - 没有软盘");
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "继续(&C)");
                HwndThread.createHwndThread(hwndEx);
            }
        }
    }
}
