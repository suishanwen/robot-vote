using System;
using System.Threading;
using robot.core;
using robot.util;

namespace robot.module
{
    public class JT
    {
        //JT启动
        public static void start()
        {
            TaskCore taskCore = MonitorCore.GetTaskCore();
            taskCore.ProjectName = TaskCore.TASK_VOTE_JT;

            IntPtr hwnd = IntPtr.Zero;
            do
            {
                if (!taskCore.NameCheck())
                {
                    return;
                }

                hwnd = HwndUtil.FindWindow("ThunderRT6FormDC", null);
                Thread.Sleep(500);
            } while (hwnd == IntPtr.Zero);

            //设置拨号延迟
            IntPtr ThunderRT6Frame = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "ThunderRT6Frame", "设置");
            IntPtr hwndEx = HwndUtil.FindWindowEx(ThunderRT6Frame, IntPtr.Zero, "ThunderRT6TextBox", null);
            hwndEx = HwndUtil.FindWindowEx(ThunderRT6Frame, hwndEx, "ThunderRT6TextBox", null);
            HwndUtil.setText(hwndEx, (ConfigCore.Delay / 1000).ToString());
            //设置工号
            if (ConfigCore.InputId.Equals("1"))
            {
                ThunderRT6Frame = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "ThunderRT6Frame", "会员");
                hwndEx = HwndUtil.FindWindowEx(ThunderRT6Frame, IntPtr.Zero, "ThunderRT6TextBox", null);
                HwndUtil.setText(hwndEx, ConfigCore.Id);
            }
            //开始投票
            hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "自动投票");
            HwndThread.createHwndThread(hwndEx);
            taskCore.FinishStart();
        }

        //JT到票检测
        public static bool OverCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow(null, "VOTETOOL");
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
                ConfigCore.WriteOver();
                return true;
            }

            return false;
        }

        public static void StopAndUpload()
        {
            IntPtr hwnd = HwndUtil.FindWindow("ThunderRT6FormDC", null);
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "停止投票");
                while (!Net.IsOnline())
                {
                    Thread.Sleep(500);
                }

                HwndThread.createHwndThread(hwndEx);
                Thread.Sleep(5000);
            }
        }
    }
}