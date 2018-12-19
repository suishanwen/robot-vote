using System;
using System.Text;
using System.Threading;
using robot.core;
using robot.util;

namespace robot.module
{
    public class JiuTian
    {
        private string workingPath = Environment.CurrentDirectory; //当前工作路径
        private static string jiutianCode = "Afx:400000:b:10011:1900015:0";

        //九天启动
        public static void Start()
        {
            TaskCore taskCore = MonitorCore.GetTaskCore();
            taskCore.ProjectName = TaskCore.TASK_VOTE_JIUTIAN;
            IntPtr hwnd = IntPtr.Zero;
            IntPtr hwndSysTabControl32 = IntPtr.Zero;
            IntPtr workCondition = IntPtr.Zero;
            IntPtr preparedCheck = IntPtr.Zero;
            IntPtr startButton = IntPtr.Zero;
            do
            {
                if (!taskCore.NameCheck())
                {
                    return;
                }
                hwnd = HwndUtil.FindWindow("WTWindow", null);
                hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                workCondition = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "工作情况");
                jiutianCode = "Afx:400000:b:10011:1900015:0";
                preparedCheck = HwndUtil.FindWindowEx(workCondition, IntPtr.Zero, jiutianCode,"加载成功 可开始投票");
                if (preparedCheck == IntPtr.Zero)
                {
                    //不换
                    preparedCheck = HwndUtil.FindWindowEx(workCondition, IntPtr.Zero, "_EL_Label", "加载成功 可开始投票");
                    jiutianCode = "_EL_Label";
                }
                if (preparedCheck == IntPtr.Zero)
                {
                    //WIN7
                    jiutianCode = "Afx:400000:b:10003:1900015:0";

                    preparedCheck = HwndUtil.FindWindowEx(workCondition, IntPtr.Zero, jiutianCode, "加载成功 可开始投票");
                }
                if (preparedCheck == IntPtr.Zero)
                {
                    //WIN10
                    jiutianCode = "Afx:400000:b:10003:900015:0";
                    preparedCheck = HwndUtil.FindWindowEx(workCondition, IntPtr.Zero, jiutianCode, "加载成功 可开始投票");
                }
                startButton = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "");
                startButton = HwndUtil.FindWindowEx(startButton, IntPtr.Zero, "Button", "开始投票");
                Thread.Sleep(500);
            } while (preparedCheck == IntPtr.Zero || startButton == IntPtr.Zero);
            //设置拨号延迟
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "拨号设置");
            hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "SysTabControl32", "");
            hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Edit", null);
            HwndUtil.setText(hwndEx, ConfigCore.Delay.ToString());
            //设置工号
            if (ConfigCore.InputId.Equals("1"))
            {
                hwndEx = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "请输入工号");
                hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Edit", null);
                HwndUtil.setText(hwndEx, ConfigCore.Id);
            }
            HwndUtil.clickHwnd(startButton);
            Thread.Sleep(500);
            taskCore.FinishStart();
        }

        //九天到票检测
        public static bool OverCheck(ref int s)
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            if (hwnd == IntPtr.Zero)
            {
                s++;
            }
            else
            {
                s = 0;
            }

            if (s > 5)
            {
                if (MonitorCore.GetTaskCore().IsAutoVote)
                {
                    AutoVote.AddVoteProjectNameDropedTemp(false);
                }
                ConfigCore.WriteOver();
                return true;
            }

            return false;
        }


        //九天限人检测
        public static bool RestrictCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow("#32770", "信息提示");
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

        //九天验证码输入检测
        public static bool IsIdentifyCode()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            IntPtr hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
            IntPtr testHwnd = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "输入验证码后回车,看不清直接回车切换");
            if (testHwnd != IntPtr.Zero)
            {
                if (MonitorCore.GetTaskCore().IsAutoVote)
                {
                    AutoVote.AddVoteProjectNameDroped(false);
                }
                ProgressCore.KillProcess(false);
                return true;
            }

            return false;
        }

        //九天禁止虚拟机检测
        public static bool VmBanCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow("#32770", "信息：");
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Static", "本任务禁止在虚拟机内运行");
            if (hwndEx != IntPtr.Zero)
            {
                TaskCore taskCore = MonitorCore.GetTaskCore();
                if (taskCore.IsAutoVote)
                {
                    taskCore.AddVoteProjectNameDroped(false);
                }
                HwndUtil.closeHwnd(hwnd);
                return true;
            }
            return false;
        }


        //获取九天成功数
        public static int GetSucc()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            IntPtr hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
            IntPtr hwndStat = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "投票统计");
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndStat, IntPtr.Zero, jiutianCode, "超时票数");
            hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, jiutianCode, null);
            try
            {
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, jiutianCode, null);
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, jiutianCode, null);
                return int.Parse(HwndUtil.GetControlText(hwndEx));
            }
            catch (Exception)
            {
                LogCore.Write("获取九天成功失败！");
            }
            return 0;

        }

        public static void StopAndUpload()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            if (hwnd != IntPtr.Zero)
            {
                hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "");
                hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "结束投票");
                HwndUtil.clickHwnd(hwnd);
                int s = 0;
                IntPtr hwndEx = IntPtr.Zero;
                do
                {
                    Thread.Sleep(500);
                    hwnd = HwndUtil.FindWindow("WTWindow", null);
                    hwndEx = HwndUtil.FindWindow("#32770", "信息：");
                    if (s % 10 == 0&& hwnd != IntPtr.Zero)
                    {
                        IntPtr hwnd0 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                        hwnd0 = HwndUtil.FindWindowEx(hwnd0, IntPtr.Zero, "Button", "");
                        hwnd0 = HwndUtil.FindWindowEx(hwnd0, IntPtr.Zero, "Button", "结束投票");
                        HwndUtil.clickHwnd(hwnd0);
                    }
                    if (hwndEx != IntPtr.Zero)
                    {
                        HwndUtil.closeHwnd(hwndEx);
                        s = 90;
                    }
                    s++;
                } while (hwnd != IntPtr.Zero && s < 90);
            }
        }
    }
}