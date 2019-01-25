using System;
using System.Threading;
using robot.core;
using robot.util;

namespace robot.module
{
    public class MM
    {
        //MM启动
        public static void Start()
        {
            TaskCore taskCore = MonitorCore.GetTaskCore();
            taskCore.ProjectName = TaskCore.TASK_VOTE_MM;
            IntPtr hwnd = IntPtr.Zero;
            do
            {
                if (!taskCore.NameCheck())
                {
                    return;
                }

                hwnd = HwndUtil.FindWindow("WTWindow", null);
                Thread.Sleep(500);
            } while (hwnd == IntPtr.Zero);

            //设置拨号延迟
            IntPtr ButtonHwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "设置");
            IntPtr hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, IntPtr.Zero, "Edit", "2");
            if (hwndEx == IntPtr.Zero)
            {
                hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, IntPtr.Zero, "Edit", "3");
            }

            if (hwndEx == IntPtr.Zero)
            {
                hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, IntPtr.Zero, "Edit", "4");
            }

            HwndUtil.setText(hwndEx, (ConfigCore.Delay / 1000).ToString());
            //设置工号
            if (ConfigCore.InputId.Equals("1"))
            {
                ButtonHwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "会员");
                hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, IntPtr.Zero, "Edit", null);
                hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
                hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
                hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
                hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
                HwndUtil.setText(hwndEx, ConfigCore.Id);
            }

            //开始投票
            hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "自动投票");
            HwndThread.createHwndThread(hwndEx);
            taskCore.FinishStart();
        }

        //获取MM成功数
        public static int GetSucc()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            IntPtr ButtonHwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "统计");
            IntPtr hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, IntPtr.Zero, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            try
            {
                return int.Parse(HwndUtil.GetControlText(hwndEx));
            }
            catch (Exception) {
                LogCore.Write("获取mm成功失败！");
            }
            return 0;
        }

        //MM到票检测
        public static bool OverCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow(null, "投票软件提示");
            if (hwnd != IntPtr.Zero)
            {
                if (MonitorCore.GetTaskCore().IsAutoVote)
                {
                    AutoVote.AddVoteProjectNameDropedTemp();
                }
                HwndUtil.closeHwnd(hwnd);
                ConfigCore.WriteOver();
                return true;
            }

            return false;
        }

        public static bool ExcpCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow("#32770", "信息：");
            if (hwnd != IntPtr.Zero)
            {
                if (MonitorCore.GetTaskCore().IsAutoVote)
                {
                    AutoVote.AddVoteProjectNameDroped(false);
                }
                HwndUtil.closeHwnd(hwnd);
                return true;
            }

            return false;
        }

        public static bool ErrCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow("#32770", null);
            if (hwnd != IntPtr.Zero)
            {
                string txt = HwndUtil.GetControlText(hwnd);
                if (txt.IndexOf("错误") != -1) 
                {
                    if (MonitorCore.GetTaskCore().IsAutoVote)
                    {
                        AutoVote.AddVoteProjectNameDroped(false);
                    }
                    HwndUtil.closeHwnd(hwnd);
                    return true;
                }
            }
            return false;
        }

        public static void StopAndUpload()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "停止投票");
                while (!Net.IsOnline())
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