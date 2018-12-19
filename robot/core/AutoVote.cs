using System;
using robot.module;
using robot.util;

namespace robot.core
{
    public class AutoVote
    {
        private static int timerChecked;
        private static int succCount;
        public static bool FailTooMuch;


        public static void Init()
        {
            timerChecked = 0;
            succCount = 0;
            FailTooMuch = false;
        }

        public static void CheckSucc()
        {
            TaskCore taskCore = MonitorCore.GetTaskCore();
            string taskName = taskCore.TaskName;
            if (taskCore.IsVoteTask())
            {
                int succ = 0;
                if (taskName.Equals(TaskCore.TASK_VOTE_JIUTIAN))
                {
                    succ = JiuTian.GetSucc();
                }
                else if (taskName.Equals(TaskCore.TASK_VOTE_MM))
                {
                    succ = MM.GetSucc();
                }
                else if (taskName.Equals(TaskCore.TASK_VOTE_YUANQIU))
                {
                    succ = YuanQiu.GetSucc();
                }

                double price = 0;
                try
                {
                    price = double.Parse(ConfigCore.GetAutoVote("Price"));
                }
                catch (Exception)
                {
                }

                int validCount = price >= 1 ? 1 : 2;
                if (succ - succCount < validCount)
                {
                    timerChecked++;
                    if (timerChecked >= 2)
                    {
                        FailTooMuch = true;
                    }
                }
                else
                {
                    timerChecked = 0;
                }

                LogCore.Write("成功:" + succ + " 上次成功:" + succCount);
                succCount = succ;
            }
        }

        //添加黑名单项目 临
        public static void AddVoteProjectNameDropedTemp(bool isAllProject)
        {
            string projectName = ConfigCore.GetAutoVote("ProjectName");
            TaskInfo taskInfo = TaskInfos.Get();
            if (taskInfo != null && taskInfo.ProjectName != projectName)
            {
                return;
            }

            if (isAllProject)
            {
                projectName = projectName.Substring(0, projectName.IndexOf("_"));
            }

            string voteProjectNameDroped = ConfigCore.GetAutoVote("voteProjectNameDropedTemp");
            int dropVote = 0;
            try
            {
                dropVote = int.Parse(ConfigCore.GetAutoVote("dropVote"));
            }
            catch (Exception)
            {
            }
            finally
            {
                dropVote++;
            }

            ConfigCore.WriteAutoVote("dropVote", dropVote.ToString());
            if (StringUtil.isEmpty(voteProjectNameDroped) || voteProjectNameDroped.IndexOf(projectName) == -1)
            {
                int validDrop;
                try
                {
                    validDrop = int.Parse(ConfigCore.GetAutoVote("validDrop"));
                }
                catch (Exception)
                {
                    validDrop = 1;
                }

                if (dropVote >= validDrop)
                {
                    LogCore.Write($"{projectName}到票临时拉黑5分钟");
                    voteProjectNameDroped +=
                        StringUtil.isEmpty(voteProjectNameDroped) ? projectName : "|" + projectName;
                    ConfigCore.WriteAutoVote("voteProjectNameDropedTemp", voteProjectNameDroped);
                }
            }
        }

        //添加黑名单项目
        public static void AddVoteProjectNameDroped(bool isAllProject)
        {
            string projectName = ConfigCore.GetAutoVote("ProjectName");
            //一机器只允许拉黑投票一次
            string drop = IniReadWriter.ReadIniKeys("Command", "drop", "./handler.ini");
            TaskInfo taskInfo = TaskInfos.Get();
            if ((taskInfo != null && taskInfo.ProjectName != projectName) || drop == projectName)
            {
                return;
            }

            if (isAllProject)
            {
                projectName = projectName.Substring(0, projectName.IndexOf("_"));
            }

            IniReadWriter.WriteIniKeys("Command", "drop", projectName, "./handler.ini");
            string voteProjectNameDroped = ConfigCore.GetAutoVote("voteProjectNameDroped");
            int dropVote = 0;
            try
            {
                dropVote = int.Parse(ConfigCore.GetAutoVote("dropVote"));
            }
            catch (Exception)
            {
            }
            finally
            {
                dropVote++;
            }

            ConfigCore.WriteAutoVote("dropVote", dropVote.ToString());
            if (StringUtil.isEmpty(voteProjectNameDroped) || voteProjectNameDroped.IndexOf(projectName) == -1)
            {
                int validDrop;
                double blackRate;
                try
                {
                    validDrop = int.Parse(ConfigCore.GetAutoVote("validDrop"));
                    blackRate = double.Parse(ConfigCore.GetAutoVote("blackRate"));
                }
                catch (Exception)
                {
                    validDrop = 1;
                    blackRate = 1;
                }

                if (dropVote >= validDrop)
                {
                    LogCore.Write($"{projectName}拉黑{blackRate * 30}分钟");
                    voteProjectNameDroped +=
                        StringUtil.isEmpty(voteProjectNameDroped) ? projectName : "|" + projectName;
                    ConfigCore.WriteAutoVote("voteProjectNameDroped", voteProjectNameDroped);
                }
            }
        }
    }
}