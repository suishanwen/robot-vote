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
            IntPtr hwnd = HwndUtil.FindWindow("#32770", "Windows - 没有软盘");
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "继续(&C)");
                HwndThread.createHwndThread(hwndEx);
            }
            else
            {
                Process.Start("shutdown.exe", "-r -t 0");
                Form1.MainClose();
            }
        }
    }
}
