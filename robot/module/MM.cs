using System;
using System.Threading;
using robot.core;
using robot.util;

namespace robot.module
{
    public class MM
    {
        //MM启动
        public static void start(int delay, string id)
        {
            IntPtr hwnd = IntPtr.Zero;
            do
            {
                hwnd = HwndUtil.FindWindow("WTWindow", null);
                Thread.Sleep(500);
            } while (hwnd == IntPtr.Zero);

            //设置拨号延迟
            IntPtr ButtonHwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "设置");
            IntPtr hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, IntPtr.Zero, "Edit", "3");
            if (hwndEx == IntPtr.Zero)
            {
                hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, IntPtr.Zero, "Edit", "4");
            }

            HwndUtil.setText(hwndEx, (delay / 1000).ToString());
            //设置工号
            ButtonHwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "会员");
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, IntPtr.Zero, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            HwndUtil.setText(hwndEx, id);
            //开始投票
            hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "自动投票");
            HwndThread.createHwndThread(hwndEx);
        }

        //MM到票检测
        private bool OverCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow(null, "投票软件提示");
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
                return true;
            }

            return false;
        }

        public static void StopAndUpload()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "停止投票");
                while (!Net.isOnline())
                {
                    Thread.Sleep(500);
                }

                HwndThread.createHwndThread(hwndEx);
                int s = 0;
                IntPtr hwndTip;
                do
                {
                    s++;
                    hwndTip = HwndUtil.FindWindow(null, "投票软件提示");
                    Thread.Sleep(500);
                } while (hwndTip == IntPtr.Zero && s < 60);
            }
        }
    }
}