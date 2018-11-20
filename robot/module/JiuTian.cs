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

        //九天启动
        public static void start(int delay, string id)
        {
            IntPtr hwnd = IntPtr.Zero;
            IntPtr hwndSysTabControl32 = IntPtr.Zero;
            IntPtr preparedCheck = IntPtr.Zero;
            IntPtr startButton = IntPtr.Zero;
            do
            {
                hwnd = HwndUtil.FindWindow("WTWindow", null);
                hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                preparedCheck = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "工作情况");
                preparedCheck = HwndUtil.FindWindowEx(preparedCheck, IntPtr.Zero, "Afx:400000:b:10011:1900015:0",
                    "加载成功 可开始投票");
                startButton = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "");
                startButton = HwndUtil.FindWindowEx(startButton, IntPtr.Zero, "Button", "开始投票");
                Thread.Sleep(500);
            } while (preparedCheck == IntPtr.Zero || startButton == IntPtr.Zero);

            //设置拨号延迟
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "拨号设置");
            hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "SysTabControl32", "");
            hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Edit", null);
            HwndUtil.setText(hwndEx, delay.ToString());
            //设置工号
            hwndEx = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "请输入工号");
            hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Edit", null);
            HwndUtil.setText(hwndEx, id);
            HwndUtil.clickHwnd(startButton);
            Thread.Sleep(500);
        }

        //九天到票检测
        private bool jiutianOverCheck(ref int s)
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
                return true;
            }

            return false;
        }


        //九天限人检测
        private bool jiutianRestrictCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow("#32770", "信息提示");
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
                return true;
            }

            return false;
        }

        //九天验证码输入检测
        private bool isIdentifyCode()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            IntPtr hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
            IntPtr testHwnd = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "输入验证码后回车,看不清直接回车切换");
            if (testHwnd != IntPtr.Zero)
            {
//                ProgressCore.killProcess(false);
                NetCore.RasOperate("disconnect");
                return true;
            }

            return false;
        }

        //获取 是否需要传票关闭
        private bool GetStopIndicator()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                IntPtr hwndStat = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "投票统计");
                IntPtr hwndEx =
                    HwndUtil.FindWindowEx(hwndStat, IntPtr.Zero, "Afx:400000:b:10011:1900015:0", "运行时间");
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                StringBuilder unUpload = new StringBuilder(512);
                HwndUtil.GetWindowText(hwndEx, unUpload, unUpload.Capacity);
                return int.Parse(unUpload.ToString()) > 0;
            }
            return false;
        }

        //获取九天成功数
        private int GetJiutianSucc()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            IntPtr hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
            IntPtr hwndStat = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "投票统计");
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndStat, IntPtr.Zero, "Afx:400000:b:10011:1900015:0", "超时票数");
            hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
            try
            {
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                StringBuilder succ = new StringBuilder(512);
                HwndUtil.GetWindowText(hwndEx, succ, 512);
                return int.Parse(succ.ToString());
            }
            catch (Exception)
            {
            }

            return 0;
        }

        //九天成功检测
        private bool jiutianFailTooMuch()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            IntPtr hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
            IntPtr hwndStat = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "投票统计");
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndStat, IntPtr.Zero, "Afx:400000:b:10011:1900015:0", "超时票数");
            hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
            StringBuilder duration = new StringBuilder(512);
            HwndUtil.GetWindowText(hwndEx, duration, duration.Capacity);
            int min;
            try
            {
                min = int.Parse(duration.ToString().Split('：')[1]);
            }
            catch (Exception)
            {
                min = 0;
            }

            if (min >= 2)
            {
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                StringBuilder succ = new StringBuilder(512);
                HwndUtil.GetWindowText(hwndEx, succ, 512);
                int success = int.Parse(succ.ToString());
                if (success / min < 2)
                {
                    LogCore.Write("Fail Too Much ---> success:" + success + ",min:" + min); //清空日志
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
                hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "");
                hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "结束投票");
                LogCore.Write("九天结束 句柄为" + hwnd);
                HwndUtil.clickHwnd(hwnd);
                int s = 0;
                IntPtr hwndEx = IntPtr.Zero;
                do
                {
                    Thread.Sleep(500);
                    hwnd = HwndUtil.FindWindow("WTWindow", null);
                    hwndEx = HwndUtil.FindWindow("#32770", "信息：");
                    if (s % 10 == 0 && hwnd != IntPtr.Zero)
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