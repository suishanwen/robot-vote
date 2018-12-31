using System;
using System.Collections.Generic;
using robot.core;

namespace robot.util
{
    class TaskInfo
    {
        public string ProjectName { get; set; }
        public double Price { get; set; }

        public TaskInfo(string projectName, double price)
        {
            ProjectName = projectName;
            Price = price;
        }
    }

    class TaskInfos
    {
        public static TaskInfo Get()
        {
            Dictionary<int, TaskInfo> taskInfoDict = new Dictionary<int, TaskInfo>();
            try
            {
                string taskInfos = ConfigCore.GetAutoVote("TaskInfos").Trim();
                if (taskInfos.Length > 0)
                {
                    string[] taskInfoArray = taskInfos.Split('|');
                    foreach (string taskInfo in taskInfoArray)
                    {
                        int key = int.Parse(taskInfo.Substring(0, taskInfo.IndexOf(":")));
                        string[] task = taskInfo.Substring(taskInfo.IndexOf(":") + 1).Split('-');
                        taskInfoDict.Add(key, new TaskInfo(task[0], double.Parse(task[1])));
                    }
                }
            }
            catch (Exception)
            {
            }

            if (taskInfoDict.ContainsKey(ConfigCore.Sort))
            {
                return taskInfoDict[ConfigCore.Sort];
            }
            return null;
        }
        
        public static Dictionary<int, TaskInfo> GetDict()
        {
            Dictionary<int, TaskInfo> taskInfoDict = new Dictionary<int, TaskInfo>();
            try
            {
                string taskInfos = ConfigCore.GetAutoVote("TaskInfos").Trim();
                if (taskInfos.Length > 0)
                {
                    string[] taskInfoArray = taskInfos.Split('|');
                    foreach (string taskInfo in taskInfoArray)
                    {
                        int key = int.Parse(taskInfo.Substring(0, taskInfo.IndexOf(":")));
                        string[] task = taskInfo.Substring(taskInfo.IndexOf(":") + 1).Split('-');
                        taskInfoDict.Add(key, new TaskInfo(task[0], double.Parse(task[1])));
                    }
                }
            }
            catch (Exception) { }
            return taskInfoDict;
        }
        
        public static void Clear()
        {
            Dictionary<int, TaskInfo> taskInfoDict = GetDict();
            if (taskInfoDict.ContainsKey(ConfigCore.Sort))
            {
                taskInfoDict.Remove(ConfigCore.Sort);
            }
            Set(taskInfoDict);
        }

        public static void Set(Dictionary<int, TaskInfo> taskInfoDict)
        {
            string taskInfos = "";
            foreach (int key in taskInfoDict.Keys)
            {
                taskInfos += string.Format("{0}:{1}-{2}|", key.ToString(), taskInfoDict[key].ProjectName, taskInfoDict[key].Price.ToString());
            }
            if (taskInfos.Length > 0)
            {
                taskInfos = taskInfos.Substring(0, taskInfos.Length - 1);
            }
            ConfigCore.WriteAutoVote("TaskInfos", taskInfos);
        }
    }
}