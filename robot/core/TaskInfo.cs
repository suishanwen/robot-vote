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
    }
}