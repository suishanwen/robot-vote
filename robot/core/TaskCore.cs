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
        public static string TASK_SYS_UPDATE = "Update";
        public static string TASK_SYS_WAIT_ORDER = "待命";
        public static string TASK_SYS_SHUTDOWN = "关机";
        public static string TASK_SYS_RESTART = "重启";
        public static string TASK_SYS_NET_TEST = "网络测试";
        public static string TASK_SYS_CLEAN = "CLEAN";
        public static string TASK_VOTE_JIUTIAN = "九天";
        public static string TASK_VOTE_YUANQIU = "圆球";
        public static string TASK_VOTE_PROJECT = "投票项目";
        public static string TASK_VOTE_MM = "MM";
        public static string TASK_VOTE_JT = "JT";
        public static string TASK_VOTE_ML = "ML";
        public static string TASK_VOTE_DM = "DM";
        public static string TASK_VOTE_JZ = "JZ";
        public static string TASK_VOTE_HY = "HY";
        public static string TASK_VOTE_OUTDO = "Outdo";

        public string ProjectName { get; set; }
        public string TaskPath { get; set; }
        public string CustomPath { get; set; }
        public bool IsAutoVote { get; set; }
        public string TaskChange { get; set; }
        public string TaskName { get; set; }
        public int TimerChecked { get; set; }
        public int SuccCount { get; set; }
        public int overTimeCount { get; set; }
        public bool FailTooMuch { get; set; }

        //判断当前是否为系统任务
        public bool IsSysTask()
        {
            return TaskName.Equals(TASK_SYS_UPDATE) || TaskName.Equals(TASK_SYS_WAIT_ORDER) ||
                   TaskName.Equals(TASK_SYS_SHUTDOWN) || TaskName.Equals(TASK_SYS_RESTART) ||
                   TaskName.Equals(TASK_SYS_NET_TEST) || TaskName.Equals(TASK_SYS_CLEAN);
        }

        //判断当前是否为投票项目
        public bool IsVoteTask()
        {
            String projectName = ConfigCore.GetAutoVote("ProjectName");
            IsAutoVote = !StringUtil.isEmpty(projectName);
            return TaskName.Equals(TASK_VOTE_PROJECT) || TaskName.Equals(TASK_VOTE_JIUTIAN) ||
                   TaskName.Equals(TASK_VOTE_YUANQIU) ||
                   TaskName.Equals(TASK_VOTE_MM) || TaskName.Equals(TASK_VOTE_ML) || TaskName.Equals(TASK_VOTE_JZ) ||
                   TaskName.Equals(TASK_VOTE_JT) || TaskName.Equals(TASK_VOTE_DM) || TaskName.Equals(TASK_VOTE_OUTDO);
        }


        //获取 是否需要传票关闭
        private bool GetStopIndicator()
        {
            if (TaskName == null)
            {
                TaskName = ConfigCore.GetTaskName();
            }
            if (TaskName.Equals(TASK_VOTE_JIUTIAN))
            {
                return JiuTian.GetSucc() > 0;
            }

            if (TaskName.Equals(TASK_VOTE_MM))
            {
                return MM.GetSucc() > 0;
            }

            if (TaskName.Equals(TASK_VOTE_YUANQIU))
            {
                return YuanQiu.GetSucc() > 0;
            }

            return true;
        }

        //待命
        public void WaitOrder()
        {
            ConfigCore.SwitchWaitOrder("0");
        }

        //切换待命
        public void SwitchWaitOrder()
        {
            ConfigCore.SwitchWaitOrder("1");
        }

        //切换任务
        private void ChangeTask()
        {
            if (TaskChange.Equals("1"))
            {
                overTimeCount = 0;
                ConfigCore.InitWorker("");
                CustomPath = ConfigCore.GetCustomPath();
                if (CustomPath != "")
                {
                    LogCore.Write($"切换任务:{CustomPath}");
                }

                if (IsVoteTask() && IsAutoVote)
                {
                    string projectName = ConfigCore.GetAutoVote("ProjectName");
                    string drop = "";
                    try
                    {
                        drop = IniReadWriter.ReadIniKeys("Command", "drop", "./handler.ini");
                    }
                    catch (Exception)
                    {
                    }

                    if (drop != projectName)
                    {
                        IniReadWriter.WriteIniKeys("Command", "drop", "", "./handler.ini");
                    }
                }
            }

            if (TaskName.Equals(TASK_SYS_WAIT_ORDER)) //待命
            {
                NetCore.DisConnect();
                TaskName = ConfigCore.GetTaskName();
                if (TaskName.Equals(TASK_SYS_WAIT_ORDER))
                {
                    WaitOrder();
                }
            }
            else if (TaskName.Equals(TASK_SYS_NET_TEST)) //网络TEST
            {
                if (Net.IsOnline())
                {
                    NetCore.DisConnect();
                }

                Thread.Sleep(500);
                NetCore.Connect();
                Thread.Sleep(500);
                if (!Net.IsOnline())
                {
                    NetCore.Connect();
                    Thread.Sleep(1000);
                }

                if (Net.IsOnline())
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
                Form1.MainClose();
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
                Form1.MainClose();
            }
            else if (TaskName.Equals(TASK_SYS_UPDATE)) //升级
            {
                WaitOrder();
                Upgrade.Update();
                Form1.MainClose();
            }
            else if (TaskName.Equals(TASK_SYS_CLEAN)) //清理
            {
                WaitOrder();
                FileUtil.DeleteFolder(PathCore.WorkingPath + "\\投票项目");
            }
            else if (IsVoteTask()) //投票
            {
                NetCore.NetCheck();
                if (CustomPath.Equals(""))
                {
                    WaitOrder();
                    TaskChangeProcess();
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
                        catch (Exception)
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

                TaskPath = CustomPath;
            }
            else
            {
                TaskName = TASK_SYS_WAIT_ORDER;
            }
        }

        //添加黑名单项目 临
        public void addVoteProjectNameDropedTemp(bool isAllProject)
        {
            string projectName = IniReadWriter.ReadIniKeys("Command", "ProjectName", ConfigCore.PathShareAutoVote);
            if (isAllProject)
            {
                projectName = projectName.Substring(0, projectName.IndexOf("_"));
            }

            string voteProjectNameDroped =
                IniReadWriter.ReadIniKeys("Command", "voteProjectNameDropedTemp", ConfigCore.PathShareAutoVote);
            int dropVote = 0;
            try
            {
                dropVote = int.Parse(IniReadWriter.ReadIniKeys("Command", "dropVote", ConfigCore.PathShareAutoVote));
            }
            catch (Exception)
            {
            }
            finally
            {
                dropVote++;
            }

            IniReadWriter.WriteIniKeys("Command", "dropVote", dropVote.ToString(), ConfigCore.PathShareAutoVote);
            if (StringUtil.isEmpty(voteProjectNameDroped) || voteProjectNameDroped.IndexOf(projectName) == -1)
            {
                int validDrop;
                try
                {
                    validDrop = int.Parse(IniReadWriter.ReadIniKeys("Command", "validDrop",
                        ConfigCore.PathShareAutoVote));
                }
                catch (Exception)
                {
                    validDrop = 1;
                }

                if (dropVote >= validDrop)
                {
                    voteProjectNameDroped +=
                        StringUtil.isEmpty(voteProjectNameDroped) ? projectName : "|" + projectName;
                    IniReadWriter.WriteIniKeys("Command", "voteProjectNameDropedTemp", voteProjectNameDroped,
                        ConfigCore.PathShareAutoVote);
                }
            }
        }

        //添加黑名单项目
        public void AddVoteProjectNameDroped(bool isAllProject)
        {
            string projectName = IniReadWriter.ReadIniKeys("Command", "ProjectName", ConfigCore.PathShareAutoVote);
            if (isAllProject)
            {
                projectName = projectName.Substring(0, projectName.IndexOf("_"));
            }

            string voteProjectNameDroped =
                IniReadWriter.ReadIniKeys("Command", "voteProjectNameDroped", ConfigCore.PathShareAutoVote);
            int dropVote = 0;
            try
            {
                dropVote = int.Parse(IniReadWriter.ReadIniKeys("Command", "dropVote", ConfigCore.PathShareAutoVote));
            }
            catch (Exception)
            {
            }
            finally
            {
                dropVote++;
            }

            IniReadWriter.WriteIniKeys("Command", "dropVote", dropVote.ToString(), ConfigCore.PathShareAutoVote);
            if (StringUtil.isEmpty(voteProjectNameDroped) || voteProjectNameDroped.IndexOf(projectName) == -1)
            {
                int validDrop;
                try
                {
                    validDrop = int.Parse(IniReadWriter.ReadIniKeys("Command", "validDrop",
                        ConfigCore.PathShareAutoVote));
                }
                catch (Exception)
                {
                    validDrop = 1;
                }

                if (dropVote >= validDrop)
                {
                    voteProjectNameDroped +=
                        StringUtil.isEmpty(voteProjectNameDroped) ? projectName : "|" + projectName;
                    IniReadWriter.WriteIniKeys("Command", "voteProjectNameDroped", voteProjectNameDroped,
                        ConfigCore.PathShareAutoVote);
                }
            }
        }


        public void CheckFailTooMuch()
        {
            if (IsVoteTask())
            {
                int succ = 0;
                if (TaskName.Equals(TASK_VOTE_JIUTIAN))
                {
                    TimerChecked++;
                    succ = JiuTian.GetSucc();
                }
                else if (TaskName.Equals(TASK_VOTE_MM))
                {
                    TimerChecked++;
                    succ = MM.GetSucc();
                }
                else if (TaskName.Equals(TASK_VOTE_YUANQIU))
                {
                    TimerChecked++;
                    succ = YuanQiu.GetSucc();
                }

                if (succ - SuccCount < 2 && TimerChecked >= 2)
                {
                    FailTooMuch = true;
                }

                LogCore.Write("success:" + succ + " last:" + SuccCount);
                SuccCount = succ;
            }
        }

        //切换任务流程
        public void TaskChangeProcess()
        {
            //重启资源管理器
            Notification.Refresh();
            AutoVote.Init();
            ProgressCore.KillProcess(GetStopIndicator());
            NetCore.DisConnect();
            TaskName = ConfigCore.GetTaskName();
            TaskChange = ConfigCore.GetTaskChange();
            ChangeTask();
        }

        //NAME检测
        public bool NameCheck()
        {
            TaskChange = ConfigCore.GetTaskChange();
            if (TaskChange.Equals("1"))
            {
                TaskName = ConfigCore.GetTaskName();
                if (!TaskName.Equals(ProjectName))
                {
                    TaskChangeProcess();
                    return false;
                }
            }

            return true;
        }

        //结束启动
        public void FinishStart()
        {
            if (!NameCheck())
            {
                return;
            }

            ConfigCore.SetTaskChange("0");
            AutoVote.Init();
        }

        //任务初始化
        public void InitTask()
        {
            string now = DateTime.Now.ToLocalTime().ToString();
            String cacheMemory = ConfigCore.GetCacheMemory();
            if (ConfigCore.GetTaskChange().Equals("1"))
            {
                //切换任务
                TaskChangeProcess();
            }
            else if (!StringUtil.isEmpty(cacheMemory))
            {
                //缓存任务启动
                string[] arr = cacheMemory.Split('`');
                TaskChange = "0";
                TaskName = arr[0].Substring(9);
                TaskPath = arr[1].Substring(9);
                String workerId = arr[2].Substring(7);
                if (!StringUtil.isEmpty(workerId))
                {
                    ConfigCore.InitWorker("");
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
                    return;
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


        //任务监控
        public void TaskMonitor()
        {
            //最大超时数
            int overTime = 30;
            //最大流量
            int maxKb = 200;
            try
            {
                overTime = ConfigCore.GetOverTime();
                maxKb = ConfigCore.GetMaxKb();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //延时
            int delay = 1000;
            if (ConfigCore.IsAdsl)
            {
                overTime *= 2;
                delay /= 2;
            }

            //连续在线、离线次数
            int p = 0;
            //计数
            int s = 0;
            //拨号次数
            int circle = 0;
            bool isOnline = false;
            do
            {
                isOnline = Net.IsOnline();
                NetCore.CloseException();
                if (isOnline && !ConfigCore.IsAdsl && IsAutoVote)
                {
                    if (Net.GetNetStatic(ConfigCore.AdslName) > maxKb)
                    {
                        LogCore.Write($"{TaskName}流量大于{maxKb}KB,拉黑！");
                        AutoVote.AddVoteProjectNameDroped(false);
                        SwitchWaitOrder();
                    }
                }

                if (ConfigCore.GetTaskChange().Equals("1"))
                {
                    TaskChangeProcess();
                    return;
                }

                if (IsSysTask())
                {
                    if (ConfigCore.IsAdsl && !isOnline)
                    {
                        LogCore.Write("ADSL待命断网拨号！");
                        NetCore.Connect();
                    }

                    p = 0;
                }

                if (IsAutoVote && (overTimeCount >= 2 || AutoVote.FailTooMuch))
                {
                    LogCore.Write("超时2次或连续两分钟成功过低,拉黑！");
                    AutoVote.AddVoteProjectNameDroped(false);
                    SwitchWaitOrder();
                }

                if (TaskName.Equals(TASK_VOTE_JIUTIAN) && p > 0)
                {
                    if (JiuTian.OverCheck(ref s) || JiuTian.RestrictCheck() || JiuTian.IsIdentifyCode() ||
                        JiuTian.VmBanCheck())
                    {
                        SwitchWaitOrder();
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_MM))
                {
                    if (MM.OverCheck() || MM.ExcpCheck())
                    {
                        ProgressCore.KillProcess(false);
                        SwitchWaitOrder();
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_YUANQIU))
                {
                    if (YuanQiu.OverCheck())
                    {
                        ProgressCore.KillProcess(false);
                        SwitchWaitOrder();
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_JT))
                {
                    if (JT.OverCheck())
                    {
                        ProgressCore.KillProcess(false);
                        SwitchWaitOrder();
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
                        ProgressCore.KillProcess(false);
                        SwitchWaitOrder();
                    }
                }
                else if (TaskName.Equals(TASK_VOTE_HY))
                {
                    if (HY.OverCheck())
                    {
                        ProgressCore.KillProcess(false);
                        SwitchWaitOrder();
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

                Thread.Sleep(delay);
            } while (p == 0 || (p > 0 && p < overTime) || (p < 0 && p > -overTime));

            overTimeCount++;
            LogCore.Write($"超时{overTimeCount}次！");
            TaskChangeProcess();
        }
    }
}