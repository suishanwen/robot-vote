using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using robot.module;
using robot.util;

namespace robot.core
{
    public class TaskCore
    {
        private static Form1 _form1;
        private const string TASK_SYS_UPDATE = "Update";
        private const string TASK_SYS_WAIT_ORDER = "待命";
        private const string TASK_SYS_SHUTDOWN = "关机";
        private const string TASK_SYS_RESTART = "重启";
        private const string TASK_SYS_NET_TEST = "网络测试";
        private const string TASK_VOTE_JIUTIAN = "九天";
        private const string TASK_VOTE_YUANQIU = "圆球";
        private const string TASK_VOTE_PROJECT = "投票项目";
        private const string TASK_VOTE_MM = "MM";
        private const string TASK_VOTE_JT = "JT";
        private const string TASK_VOTE_ML = "ML";
        private const string TASK_VOTE_DM = "DM";
        private const string TASK_VOTE_JZ = "JZ";
        private const string TASK_VOTE_HY = "HY";
        private const string TASK_VOTE_OUTDO = "Outdo";

        public static string TaskChange;
        public static string TaskName;
        public static string ProjectName;
        public static string TaskPath;
        public static string CustomPath;
        public static bool IsAutoVote;

        public static void InitForm(Form1 form1)
        {
            _form1 = form1;
        }

        //判断当前是否为系统任务
        public static bool IsSysTask()
        {
            return TaskName.Equals(TASK_SYS_UPDATE) || TaskName.Equals(TASK_SYS_WAIT_ORDER) ||
                   TaskName.Equals(TASK_SYS_SHUTDOWN) || TaskName.Equals(TASK_SYS_RESTART) ||
                   TaskName.Equals(TASK_SYS_NET_TEST);
        }

        //判断当前是否为投票项目
        public static bool IsVoteTask()
        {
            return TaskName.Equals(TASK_VOTE_JIUTIAN) || TaskName.Equals(TASK_VOTE_YUANQIU) ||
                   TaskName.Equals(TASK_VOTE_MM) || TaskName.Equals(TASK_VOTE_ML) || TaskName.Equals(TASK_VOTE_JZ) ||
                   TaskName.Equals(TASK_VOTE_JT) || TaskName.Equals(TASK_VOTE_DM) || TaskName.Equals(TASK_VOTE_OUTDO);
        }

        public static void StopAndUpload()
        {
            if (TaskName.Equals(TASK_VOTE_JIUTIAN))
            {
            }
            else if (TaskName.Equals(TASK_VOTE_YUANQIU))
            {
            }
            else if (TaskName.Equals(TASK_VOTE_JZ))
            {
            }
            else if (TaskName.Equals(TASK_VOTE_JT))
            {
            }
            else if (TaskName.Equals(TASK_VOTE_MM))
            {
            }
        }

        //获取 是否需要传票关闭
        private static bool GetStopIndicator()
        {
            if (TaskName.Equals(TASK_VOTE_JIUTIAN))
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

            return true;
        }

        //关闭进程
        private static void KillProcess(bool stopIndicator)
        {
            LogCore.Write("KillProcess");
            //传票结束
            if (stopIndicator && IsVoteTask())
            {
                LogCore.Write("stop vote!");
                if (TaskName.Equals(TASK_VOTE_JIUTIAN))
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
                else if (TaskName.Equals(TASK_VOTE_YUANQIU))
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
                else if (TaskName.Equals(TASK_VOTE_JZ))
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
                else if (TaskName.Equals(TASK_VOTE_JT))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("ThunderRT6FormDC", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "停止投票");
                        while (!Net.isOnline())
                        {
                            Thread.Sleep(500);
                        }

                        HwndThread.createHwndThread(hwndEx);
                        Thread.Sleep(5000);
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_HY))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "停止");
                        while (!Net.isOnline())
                        {
                            Thread.Sleep(500);
                        }

                        HwndThread.createHwndThread(hwndEx);
                        Thread.Sleep(5000);
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_MM))
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

            ProgressCore.KillProcess();
        }

        //待命
        private static void WaitOrder()
        {
            ConfigCore.WriteTaskName(TASK_SYS_WAIT_ORDER);
            ConfigCore.ClearTask();
        }

        //任务监控
        public static void TaskMonitor()
        {
            _form1.RefreshIcon();
            int overTime = ConfigCore.GetOverTime();
            if (overTime % 2 == 1)
            {
                overTime += 1;
            }

            overTime = overTime / 2 - 1;
            int p = 0;
            int s = 0;
            bool isOnline = false;
            int circle = 0;
            do
            {
                isOnline = Net.isOnline();
                TaskChange = ConfigCore.GetTaskChange();
                if (TaskChange.Equals("1"))
                {
                    TaskChangeProcess(true);
                    return;
                }

                if (IsSysTask())
                {
                    p = 0;
                }

                if (TaskName.Equals(TASK_VOTE_JIUTIAN) && p > 0)
                {
                    if (JiuTian.OverCheck(ref s) || JiuTian.RestrictCheck())
                    {
                        WaitOrder();
                    }

                    if (IsAutoVote)
                    {
                        if (JiuTian.IsIdentifyCode())
                        {
                            WaitOrder();
                        }
                        else if ((circle == 0 && p == 20) || (circle > 0 && p == 15) ||
                                 (circle > 0 && circle % 3 == 0 && JiuTian.FailTooMuch()))
                        {
                            ConfigCore.AddVoteProjectNameDroped(false);
                            WaitOrder();
                        }
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_MM))
                {
                    if (MM.OverCheck())
                    {
                        KillProcess(false);
                        WaitOrder();
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_YUANQIU))
                {
                    if (YuanQiu.OverCheck())
                    {
                        KillProcess(false);
                        WaitOrder();
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_JT))
                {
                    if (JT.OverCheck())
                    {
                        KillProcess(false);
                        WaitOrder();
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_ML))
                {
                    //ML到票检测
                }
                else if (TaskName.Equals(TASK_VOTE_DM))
                {
                    //DM到票检测
                }
                else if (TaskName.Equals(TASK_VOTE_JZ))
                {
                    if (JZ.OverCheck())
                    {
                        KillProcess(false);
                        WaitOrder();
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_HY))
                {
                    if (HY.OverCheck())
                    {
                        KillProcess(false);
                        WaitOrder();
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_OUTDO))
                {
                    //OUTDO到票检测
                }

                if (isOnline)
                {
                    p = p < 0 ? 1 : ++p;
                }
                else
                {
                    circle++;
                    p = p > 0 ? -1 : --p;
                }

                Thread.Sleep(2000);
            } while (p == 0 || (p > 0 && p < overTime) || (p < 0 && p > -overTime));

            if (TaskName.Equals(TASK_VOTE_MM))
            {
                NetCore.DisConnect();
                //启动拨号定时timer
                NetCore.Connect();
            }
            else
            {
                TaskChangeProcess(false);
                return;
            }

            TaskMonitor();
        }


        //切换任务
        private static void ChangeTask()
        {
            if (TaskChange.Equals("1"))
            {
                ConfigCore.InitWorker("");
                CustomPath = ConfigCore.GetCustomPath();
                LogCore.Write("TaskChange:" + CustomPath);
            }

            if (TaskName.Equals(TASK_SYS_WAIT_ORDER)) //待命
            {
                NetCore.DisConnect();
                TaskName = ConfigCore.GetTaskName();
                if (TaskName.Equals(TASK_SYS_WAIT_ORDER))
                {
                    ConfigCore.ClearTask();
                }
            }
            else if (TaskName.Equals(TASK_SYS_NET_TEST)) //网络TEST
            {
                if (Net.isOnline())
                {
                    NetCore.DisConnect();
                }

                Thread.Sleep(500);
                NetCore.Connect();
                Thread.Sleep(500);
                if (!Net.isOnline())
                {
                    NetCore.Connect();
                    Thread.Sleep(1000);
                }

                if (Net.isOnline())
                {
                    NetCore.DisConnect();
                    WaitOrder();
                }
                else
                {
                    ConfigCore.NetError("error");
                }
            }
            else if (TaskName.Equals(TASK_SYS_SHUTDOWN)) //关机
            {
                WaitOrder();
                Process.Start("shutdown.exe", "-s -t 0");
                _form1.MainThreadClose();
            }
            else if (TaskName.Equals(TASK_SYS_RESTART)) //重启
            {
                string computerRename = ConfigCore.GetComputerRename();
                if (!StringUtil.isEmpty(computerRename))
                {
                    Computer.apiSetComputerNameEx(5, computerRename + "-" + ConfigCore.Sort);
                }

                WaitOrder();
                Process.Start("shutdown.exe", "-r -t 0");
                _form1.MainThreadClose();
            }
            else if (TaskName.Equals(TASK_SYS_UPDATE)) //升级
            {
                WaitOrder();
                Upgrade.Update();
                _form1.MainThreadClose();
            }
            else if (IsVoteTask()) //投票
            {
                NetCore.NetCheck();
                if (CustomPath.Equals(""))
                {
                    WaitOrder();
                    TaskChangeProcess(false);
                    return;
                }

                if (TaskChange.Equals("1"))
                {
                    if (CustomPath.Substring(CustomPath.LastIndexOf("\\") + 1) == "vote.exe")
                    {
                        ProgressCore.StartProcess(CustomPath.Substring(0, CustomPath.Length - 9) + @"\启动九天.bat");
                        TaskName = TASK_VOTE_JIUTIAN;
                    }
                    else
                    {
                        ProgressCore.StartProcess(CustomPath);
                        TaskName = TASK_VOTE_PROJECT;
                        IntPtr hwnd0, hwnd1, hwnd2, hwnd3, hwnd4;
                        do
                        {
                            hwnd0 = HwndUtil.FindWindow("WTWindow", null);
                            hwnd1 = HwndUtil.FindWindow("TForm1", null);
                            hwnd2 = HwndUtil.FindWindow("ThunderRT6FormDC", null);
                            hwnd3 = HwndUtil.FindWindow("obj_Form", null);
                            hwnd4 = HwndUtil.FindWindow("TMainForm", null);
                            if (hwnd0 != IntPtr.Zero)
                            {
                                StringBuilder title = new StringBuilder(512);
                                int i = HwndUtil.GetWindowText(hwnd0, title, 512);
                                if (title.ToString().Substring(0, 6) == "自动投票工具")
                                {
                                    TaskName = TASK_VOTE_MM;
                                }
                                else if (title.ToString().Substring(0, 8) == "VOTE2016")
                                {
                                    TaskName = TASK_VOTE_ML;
                                }
                                else if (title.ToString().IndexOf("自动投票软件") != -1)
                                {
                                    TaskName = TASK_VOTE_HY;
                                }
                            }
                            else if (hwnd1 != IntPtr.Zero)
                            {
                                TaskName = TASK_VOTE_YUANQIU;
                            }
                            else if (hwnd2 != IntPtr.Zero)
                            {
                                TaskName = TASK_VOTE_JT;
                            }
                            else if (hwnd3 != IntPtr.Zero)
                            {
                                TaskName = TASK_VOTE_DM;
                            }
                            else if (hwnd4 != IntPtr.Zero)
                            {
                                TaskName = TASK_VOTE_JZ;
                            }

                            Thread.Sleep(500);
                        } while (TaskName.Trim().Equals(TASK_VOTE_PROJECT));
                    }

                    bool safeWrite = false;
                    Thread.Sleep(ConfigCore.Sort % 10 * 50);
                    do
                    {
                        try
                        {
                            ConfigCore.WriteTaskName(TaskName);
                            Thread.Sleep(200);
                            string taskNameCheck = ConfigCore.GetTaskName();
                            if (StringUtil.isEmpty(taskNameCheck) || !taskNameCheck.Equals(TaskName))
                            {
                                LogCore.Write("TaskName Write Error!");
                                ConfigCore.WriteTaskName(TaskName);
                                throw new Exception();
                            }

                            safeWrite = true;
                        }
                        catch (Exception e)
                        {
                            Thread.Sleep(ConfigCore.Sort % 10 * 50);
                        }
                    } while (!safeWrite);
                }

                if (TaskName.Equals(TASK_VOTE_JIUTIAN))
                {
                    if (!TaskChange.Equals("1"))
                    {
                        ProgressCore.StartProcess(CustomPath.Substring(0, CustomPath.Length - 9) + @"\启动九天.bat");
                        Thread.Sleep(500);
                    }

                    JiuTian.Start();
                }
                else
                {
                    if (!TaskChange.Equals("1"))
                    {
                        ProgressCore.StartProcess(CustomPath);
                        Thread.Sleep(500);
                    }

                    if (TaskName.Equals(TASK_VOTE_MM))
                    {
                        MM.Start();
                    }
                    else if (TaskName.Equals(TASK_VOTE_ML))
                    {
                        //ML开始程序
                    }
                    else if (TaskName.Equals(TASK_VOTE_YUANQIU))
                    {
                        YuanQiu.Start();
                    }
                    else if (TaskName.Equals(TASK_VOTE_JT))
                    {
                        JT.start();
                    }
                    else if (TaskName.Equals(TASK_VOTE_DM))
                    {
                        //DM开始程序
                    }
                    else if (TaskName.Equals(TASK_VOTE_JZ))
                    {
                        JZ.start();
                    }
                    else if (TaskName.Equals(TASK_VOTE_HY))
                    {
                        HY.Start();
                    }
                }

                ProjectName = TaskName;
                TaskPath = CustomPath;
            }
            else
            {
                TaskName = TASK_SYS_WAIT_ORDER;
            }

            TaskMonitor();
        }

        //切换任务流程
        public static void TaskChangeProcess(bool stopIndicator)
        {
            LogCore.Write("taskChangeProcess");
            if (StringUtil.isEmpty(TaskName))
            {
                TaskName = ConfigCore.GetTaskName();
            }

            KillProcess(GetStopIndicator());
            NetCore.DisConnect();
            TaskName = ConfigCore.GetTaskName();
            TaskChange = ConfigCore.GetTaskChange();
            ChangeTask();
        }

        //NAME检测
        public static bool NameCheck()
        {
            TaskChange = ConfigCore.GetTaskChange();
            if (TaskChange.Equals("1"))
            {
                TaskName = ConfigCore.GetTaskName();
                if (!TaskName.Equals(ProjectName))
                {
                    TaskChangeProcess(false);
                    return false;
                }
            }

            return true;
        }

        //结束启动
        public static void FinishStart()
        {
            if (!NameCheck())
            {
                return;
            }

            ConfigCore.SetTaskChange("0");
        }

        public static void InitTask()
        {
            //进程初始化部分
            string now = DateTime.Now.ToLocalTime().ToString();
            if (ConfigCore.GetTaskChange().Equals("1"))
            {
                TaskChangeProcess(true);
            }
            else
            {
                String cacheMemory = ConfigCore.GetCacheMemory();
                if (!StringUtil.isEmpty(cacheMemory))
                {
                    string[] arr = cacheMemory.Split('`');
                    TaskName = arr[0].Substring(9);
                    TaskPath = arr[1].Substring(9);
                    String workerId = arr[2].Substring(7);
                    if (!StringUtil.isEmpty(workerId))
                    {
                        ConfigCore.InputId = "1";
                        ConfigCore.Tail = "1";
                    }

                    CustomPath = TaskPath;
                    Process[] pros = ProgressCore.GetProcess("");
                    if (pros.Length > 0)
                    {
                        Notification.Show(TaskName + "运行中,进入维护状态", ToolTipIcon.Info);
                    }
                    else
                    {
                        Notification.Show("发现项目缓存,通过自定义路径启动" + TaskName, ToolTipIcon.Info);
                        ChangeTask();
                    }

                    ConfigCore.ClearCacheMemory();
                }
                else
                {
                    //无缓存待命
                    TaskName = TASK_SYS_WAIT_ORDER;
                    NetCore.NetCheck();
                    Thread.Sleep(1000);
                    NetCore.DisConnect();
                    Notification.Show("未发现项目缓存,待命中...\n请通过控制与监控端启动" + ConfigCore.Sort + "号虚拟机", ToolTipIcon.Info);
                }
            }
        }
    }
}