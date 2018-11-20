using System;
using System.Threading;
using robot.core;
using robot.util;

namespace robot.module
{
    public class JZ
    {
        //JZ启动
        public static void start(int delay, string id)
        {
            IntPtr hwnd = IntPtr.Zero;
            do
            {
                hwnd = HwndUtil.FindWindow("TMainForm", null);
                Thread.Sleep(500);
            }
            while (hwnd == IntPtr.Zero);
            //设置拨号延迟

            IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwnd, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwnd, hwndEx, "TEdit", null);
            HwndUtil.setText(hwndEx, (delay / 1000).ToString());
            //设置工号
            IntPtr hwndTGroupBox0 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "会员选项");
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox0, IntPtr.Zero, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox0, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox0, hwndEx, "TEdit", null);
            HwndUtil.setText(hwndEx, id);
            //开始投票
            IntPtr hwndTGroupBox = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "当前状态");
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, IntPtr.Zero, "TButton", "开 始");
            HwndThread.createHwndThread(hwndEx);
        }

        
        //JZ到票检测
        private bool OverCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow("TMessageForm", null);
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
                return true;
            }
            return false;
        }

        public static void StopAndUpload()
        {
            IntPtr hwnd = HwndUtil.FindWindow("TMainForm", null);
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hwndTGroupBox = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "当前状态");
                IntPtr hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, IntPtr.Zero, "TButton", "停 止");
                while (!Net.isOnline())
                {
                    Thread.Sleep(500);
                }
                HwndThread.createHwndThread(hwndEx);
                Thread.Sleep(5000);
            }
        }
    }
}