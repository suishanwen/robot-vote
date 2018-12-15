using System;
using System.Threading;
using robot.core;
using robot.util;

namespace robot.module
{
    public class YuanQiu
    {
        //圆球启动
        public static void Start()
        {

            TaskCore taskCore = MonitorCore.GetTaskCore();
            taskCore.ProjectName = TaskCore.TASK_VOTE_YUANQIU;
            IntPtr hwnd = IntPtr.Zero;
            do
            {
                if (!taskCore.NameCheck())
                {
                    return;
                }
                hwnd = HwndUtil.FindWindow("TForm1", null);
                Thread.Sleep(1000);
            } while (hwnd == IntPtr.Zero);
            Thread.Sleep(1000);
            //设置拨号延迟
            IntPtr hwndTGroupBox = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "设置");
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, IntPtr.Zero, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            HwndUtil.setText(hwndEx, (ConfigCore.Delay / 1000).ToString());
            //设置工号
            if (ConfigCore.InputId.Equals("1"))
            {
                hwndTGroupBox = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "会员");
                hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, IntPtr.Zero, "TEdit", null);
                HwndUtil.setText(hwndEx, ConfigCore.Id);
                hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
                HwndUtil.setText(hwndEx, ConfigCore.Id);
                hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
                HwndUtil.setText(hwndEx, ConfigCore.Id);
                hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            }
            //开始投票
            hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TButton", "开始");
            HwndThread.createHwndThread(hwndEx);
            taskCore.FinishStart();
        }

        //圆球到票检测
        public static bool OverCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow("TMessageForm", "register");
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
                return true;
            }

            return false;
        }

        //获取圆球成功数
        public static int GetSucc()
        {
            IntPtr hwnd = HwndUtil.FindWindow("TForm1", null);
            IntPtr hwndTGroupBox = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "状态");
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, IntPtr.Zero, "TEdit", null);
            try
            {
                return int.Parse(HwndUtil.GetControlText(hwndEx));
            }
            catch (Exception)
            {
                LogCore.Write( "获取圆球成功失败！");
            }
            return 0;

        }

        public static void StopAndUpload()
        {
            IntPtr hwnd = HwndUtil.FindWindow("TForm1", null);
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TButton", "停止");
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