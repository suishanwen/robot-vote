using System;
using System.Threading;
using robot.util;

namespace robot.core
{
    public class HwndThread
    {
        private static IntPtr inHwnd;

        //hwndThread创建
        public static void createHwndThread(IntPtr hwnd)
        {
            inHwnd = hwnd;
            Thread thread = new Thread(clickHwndByThread);
            thread.Start();
        }

        //处理句柄操作线程
        private static void clickHwndByThread()
        {
            HwndUtil.clickHwnd(inHwnd);
        }
    }
}