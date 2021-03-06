using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using robot.util;

namespace robot.core
{
    public class ConfigCore
    {
        public static string BaseConfig = @"./handler.ini";
        public static string PathShareConfig;
        public static string PathShareTask;
        public static string PathShareTaskPlus;
        public static string PathShareAutoVote;
        public static string PathShareStatistic;


        public static int Sort;
        public static int Delay;
        public static string AdslName;
        public static bool IsAdsl;

        public static string InputId = "1";
        public static string Tail = "1";
        public static string Id;

        public static bool InitConfig()
        {
            if (File.Exists(BaseConfig))
            {
                string pathShare = InitPathShare();
                if (StringUtil.isEmpty(pathShare))
                {
                    MessageBox.Show("共享不能为空！");
                    return false;
                }

                string sort = IniReadWriter.ReadIniKeys("Command", "bianhao", BaseConfig);
                string delay = IniReadWriter.ReadIniKeys("Command", "yanchi", BaseConfig);
                if (StringUtil.isEmpty(sort))
                {
                    MessageBox.Show("编号不能为空！");
                    return false;
                }

                try
                {
                    Sort = int.Parse(sort);
                    Delay = int.Parse(delay);
                }
                catch (Exception)
                {
                    MessageBox.Show("编号及延时只能是数字，请重新设置！");
                    return false;
                }

                Form1.SetFormData(Sort, Delay, pathShare);
                AdslName = RasName.GetAdslName();
                IsAdsl = AdslName == "宽带连接";
                LogCore.Write($"获取默认ADSL拨号名称:{AdslName}");
                return true;
            }
            else
            {
                MessageBox.Show("请设置后在启动！");
                return false;
            }
        }

        public static string InitPathShare()
        {
            String pathShare = IniReadWriter.ReadIniKeys("Command", "gongxiang", BaseConfig);
            PathShareConfig = pathShare + "/CF.ini";
            PathShareTask = pathShare + "/Task.ini";
            PathShareTaskPlus = pathShare + "/TaskPlus.ini";
            PathShareAutoVote = pathShare + "/AutoVote.ini";
            PathShareStatistic = pathShare + "/Statistic.ini";
            return pathShare;
        }

        public static void InitWorker(String workerId)
        {
            if (StringUtil.isEmpty(workerId))
            {
                workerId = IniReadWriter.ReadIniKeys("Command", "worker", PathShareConfig);
            }

            InputId = IniReadWriter.ReadIniKeys("Command", "printgonghao", PathShareConfig);
            Tail = IniReadWriter.ReadIniKeys("Command", "tail", PathShareConfig);
            if (Tail.Equals("1"))
            {
                Id = workerId + "-" + (Sort > 9 ? Sort.ToString() : "0" + Sort);
            }
            else
            {
                Id = workerId;
            }
        }

        public static int GetOverTime()
        {
            return int.Parse(IniReadWriter.ReadIniKeys("Command", "cishu", PathShareConfig));
        }

        public static int GetMaxKb()
        {
            return int.Parse(IniReadWriter.ReadIniKeys("Command", "maxKb", PathShareConfig));
        }

        public static String GetComputerRename()
        {
            return IniReadWriter.ReadIniKeys("Command", "computerRename", PathShareConfig);
        }

        public static String GetTaskChange()
        {
            return IniReadWriter.ReadIniKeys("Command", "TaskChange" + Sort, PathShareTask);
        }

        public static void SetTaskChange(String taskChange)
        {
            IniReadWriter.WriteIniKeys("Command", "TaskChange" + Sort, taskChange, PathShareTask);
        }

        public static String GetTaskName()
        {
            return IniReadWriter.ReadIniKeys("Command", "TaskName" + Sort, PathShareTask);
        }

        public static String GetCustomPath()
        {
            return IniReadWriter.ReadIniKeys("Command", "customPath" + Sort, PathShareTaskPlus);
        }

        public static String GetCacheMemory()
        {
            return IniReadWriter.ReadIniKeys("Command", "CacheMemory" + Sort, PathShareTaskPlus);
        }

        public static void ClearCacheMemory()
        {
            IniReadWriter.WriteIniKeys("Command", "CacheMemory" + Sort, "", PathShareTaskPlus);
        }


        public static void SwitchWaitOrder(string taskChange)
        {
            TaskInfos.Clear();
            IniReadWriter.WriteIniKeys("Command", "TaskChange" + Sort, taskChange, PathShareTask);
            IniReadWriter.WriteIniKeys("Command", "TaskName" + Sort, TaskCore.TASK_SYS_WAIT_ORDER, PathShareTask);
            IniReadWriter.WriteIniKeys("Command", "CustomPath" + Sort, "", PathShareTaskPlus);
        }

        public static void SwitchNetTest()
        {
            IniReadWriter.WriteIniKeys("Command", "TaskChange" + Sort, "1", PathShareTask);
            IniReadWriter.WriteIniKeys("Command", "TaskName" + Sort, TaskCore.TASK_SYS_NET_TEST, PathShareTask);
            IniReadWriter.WriteIniKeys("Command", "CustomPath" + Sort, "", PathShareTaskPlus);
        }


        public static void WriteTaskName(String task)
        {
            IniReadWriter.WriteIniKeys("Command", "TaskName" + Sort, task,
                PathShareTask);
        }

        public static string GetConfig(string name)
        {
            return IniReadWriter.ReadIniKeys("Command", name, PathShareConfig);
        }

        public static void WriteConfig(string name, string value)
        {
            IniReadWriter.WriteIniKeys("Command", name, value, PathShareConfig);
        }

        public static string GetBaseConfig(string name)
        {
            return IniReadWriter.ReadIniKeys("Command", name, BaseConfig);
        }

        public static void WriteBaseConfig(string name, string value)
        {
            IniReadWriter.WriteIniKeys("Command", name, value, BaseConfig);
        }

        public static string GetAutoVote(string name)
        {
            return IniReadWriter.ReadIniKeys("Command", name, PathShareAutoVote);
        }

        public static void WriteAutoVote(string name, string value)
        {
            IniReadWriter.WriteIniKeys("Command", name, value, PathShareAutoVote);
        }

        public static string GetStatistic(string name)
        {
            return IniReadWriter.ReadIniKeys(Sort.ToString(), name, PathShareStatistic);
        }

        public static void WriteStatistic(string name, string value)
        {
            IniReadWriter.WriteIniKeys(Sort.ToString(), name, value, PathShareStatistic);
        }

        public static void WriteOver()
        {
            IniReadWriter.WriteIniKeys("Command", "OVER", "1", PathShareConfig);
        }


        //缓存
        public static void Cache()
        {
            TaskCore taskCore = MonitorCore.GetTaskCore();
            if (taskCore.TaskName == null || taskCore.IsSysTask())
            {
                return;
            }

            string path = "";
            if (taskCore.CustomPath.Equals(taskCore.TaskPath))
            {
                path = taskCore.TaskPath;
            }
            else
            {
                path = "Writein";
            }

            string cacheMemory = "TaskName-" + taskCore.TaskName + "`TaskPath-" + path + "`Worker:" + Id;
            IniReadWriter.WriteIniKeys("Command", "CacheMemory" + Sort, cacheMemory, PathShareTaskPlus);
        }

        //网络异常处理
        public static void NetError(string type)
        {
            String val = IniReadWriter.ReadIniKeys("Command", "Val", PathShareConfig);
            if (StringUtil.isEmpty(val))
            {
                if (type.Equals("exception"))
                {
                    IniReadWriter.WriteIniKeys("Command", "Val", Sort.ToString(), PathShareConfig); //正数 异常
                }
                else
                {
                    IniReadWriter.WriteIniKeys("Command", "Val", (-Sort).ToString(), PathShareConfig); //负数 掉线
                }

                Form1.MainClose();
            }
            else
            {
                Thread.Sleep(2000);
                NetError(type);
            }
        }
    }
}