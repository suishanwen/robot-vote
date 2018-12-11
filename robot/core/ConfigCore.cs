using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using robot.util;

namespace robot.core
{
    public class ConfigCore
    {
        private static Form1 _form1;

        public static string BaseConfig = @"./handler.ini";
        public static string PathShareConfig;
        private static string _pathShareTask;
        private static string _pathShareTaskPlus;
        private static string _pathShareAutoVote;

        public static int Sort;
        public static int Delay;
        public static string AdslName;

        public static string InputId = "1";
        public static string Tail = "1";
        public static string Id;

        public static void InitConfig(Form1 form1)
        {
            if (File.Exists(BaseConfig))
            {
                InitPathShare();
                Sort = int.Parse(IniReadWriter.ReadIniKeys("Command", "bianhao", BaseConfig));
                Delay = int.Parse(IniReadWriter.ReadIniKeys("Command", "yanchi", BaseConfig));
                _form1 = form1;
                TaskCore.InitForm(form1);
                _form1.Sort = Sort.ToString();
                _form1.Delay = Delay.ToString();
                _form1.button1_Click(null, null);
            }
            else
            {
                MessageBox.Show(@"请设置handler.ini");
            }

            AdslName = RasName.GetAdslName();
        }

        public static void InitPathShare()
        {
            String pathShare = IniReadWriter.ReadIniKeys("Command", "gongxiang", BaseConfig);
            PathShareConfig = pathShare + "/CF.ini";
            _pathShareTask = pathShare + "/Task.ini";
            _pathShareTaskPlus = pathShare + "/TaskPlus.ini";
            _pathShareAutoVote = pathShare + "/AutoVote.ini";
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
            MessageBox.Show(PathShareConfig);
            MessageBox.Show(IniReadWriter.ReadIniKeys("Command", "cishu", PathShareConfig));
            return int.Parse(IniReadWriter.ReadIniKeys("Command", "cishu", PathShareConfig));
        }

        public static String GetComputerRename()
        {
            return IniReadWriter.ReadIniKeys("Command", "computerRename", PathShareConfig);
        }

        public static String GetTaskChange()
        {
            return IniReadWriter.ReadIniKeys("Command", "TaskChange" + Sort, _pathShareTask);
        }

        public static void SetTaskChange(String taskChange)
        {
            IniReadWriter.WriteIniKeys("Command", "TaskChange" + Sort, taskChange, _pathShareTask);
        }

        public static String GetTaskName()
        {
            return IniReadWriter.ReadIniKeys("Command", "TaskName" + Sort, _pathShareTask);
        }

        public static String GetCustomPath()
        {
            return IniReadWriter.ReadIniKeys("Command", "customPath" + Sort, _pathShareTaskPlus);
        }

        public static String GetCacheMemory()
        {
            return IniReadWriter.ReadIniKeys("Command", "CacheMemory" + Sort, _pathShareTaskPlus);
        }

        public static void ClearCacheMemory()
        {
            IniReadWriter.WriteIniKeys("Command", "CacheMemory" + Sort, "", _pathShareTaskPlus);
        }

        public static void ClearTask()
        {
            IniReadWriter.WriteIniKeys("Command", "TaskChange" + Sort, "0", _pathShareTask);
            IniReadWriter.WriteIniKeys("Command", "CustomPath" + Sort, "", _pathShareTaskPlus);
        }

        public static void WriteTaskName(String task)
        {
            IniReadWriter.WriteIniKeys("Command", "TaskName" + Sort, task,
                _pathShareTask);
        }

        //缓存
        public static void Cache()
        {
            if (TaskCore.IsSysTask())
            {
                return;
            }

            string path = "";
            if (TaskCore.CustomPath.Equals(TaskCore.TaskPath))
            {
                path = TaskCore.TaskPath;
            }
            else
            {
                path = "Writein";
            }

            string cacheMemory = "TaskName-" + TaskCore.TaskName + "`TaskPath-" + path + "`Worker:" + Id;
            IniReadWriter.WriteIniKeys("Command", "CacheMemory" + Sort, cacheMemory, _pathShareTaskPlus);
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

                _form1.MainThreadClose();
            }
            else
            {
                Thread.Sleep(2000);
                NetError(type);
            }
        }

        //添加黑名单项目
        public static void AddVoteProjectNameDroped(bool isAllProject)
        {
            string projectName = IniReadWriter.ReadIniKeys("Command", "ProjectName", _pathShareAutoVote);
            if (isAllProject)
            {
                projectName = projectName.Substring(0, projectName.IndexOf("_"));
            }

            string voteProjectNameDroped =
                IniReadWriter.ReadIniKeys("Command", "voteProjectNameDroped", _pathShareAutoVote);
            int dropVote = 0;
            try
            {
                dropVote = int.Parse(IniReadWriter.ReadIniKeys("Command", "dropVote", _pathShareAutoVote));
            }
            catch (Exception)
            {
            }
            finally
            {
                dropVote++;
            }

            IniReadWriter.WriteIniKeys("Command", "dropVote", dropVote.ToString(), _pathShareAutoVote);
            if (StringUtil.isEmpty(voteProjectNameDroped) || voteProjectNameDroped.IndexOf(projectName) == -1)
            {
                int validDrop;
                try
                {
                    validDrop = int.Parse(IniReadWriter.ReadIniKeys("Command", "validDrop", _pathShareAutoVote));
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
                        _pathShareAutoVote);
                }
            }
        }
    }
}