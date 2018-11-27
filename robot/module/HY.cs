using System;
using System.Threading;
using robot.core;
using robot.util;

namespace robot.module
{
    public class HY
    {
        //HY启动
        public static void Start()
        {
            IntPtr hwnd = IntPtr.Zero;
            do
            {
                if (!TaskCore.NameCheck())
                {
                    return;
                }

                hwnd = HwndUtil.FindWindow("WTWindow", null);
                Thread.Sleep(500);
            } while (hwnd == IntPtr.Zero);

            //设置拨号延迟
            IntPtr hwndCf = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "设置");

            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndCf, IntPtr.Zero, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndCf, hwndEx, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndCf, hwndEx, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndCf, hwndEx, "Edit", null);
            HwndUtil.setText(hwndEx, (ConfigCore.Delay / 1000).ToString());
            //设置工号
            if (ConfigCore.InputId.Equals("1"))
            {
                IntPtr hwndId = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "会员");
                hwndEx = HwndUtil.FindWindowEx(hwndId, IntPtr.Zero, "Edit", null);
                hwndEx = HwndUtil.FindWindowEx(hwndId, hwndEx, "Edit", null);
                hwndEx = HwndUtil.FindWindowEx(hwndId, hwndEx, "Edit", null);
                HwndUtil.setText(hwndEx, ConfigCore.Id);
            }

            //开始投票
            IntPtr hwndStart = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "投票");
            HwndThread.createHwndThread(hwndStart);
            TaskCore.FinishStart();
        }

        //HY到票检测
        public static bool OverCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow("#32770", "信息：");
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
//                IniReadWriter.WriteIniKeys("Command", "OVER", "1", pathShare + @"\CF.ini");
                return true;
            }

            return false;
        }
    }
}