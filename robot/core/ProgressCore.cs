using System;
using System.Diagnostics;
using System.Threading;
using robot.util;

namespace robot.core
{
    public class ProgressCore
    {
        private string workingPath = Environment.CurrentDirectory; //当前工作路径

        //通过进程名获取进程
        private Process[] getProcess(string proName)
        {
            return Process.GetProcessesByName(proName);
        }

        //获取项目进程
        private Process[] processCheck()
        {
            Process[] pros = getProcess("AutoUpdate.dll");
            if (pros.Length > 0)
            {
                foreach (Process p in pros)
                {
                    p.Kill();
                }
            }

            string process1 = "vote.exe";
            string process2 = "register.exe";
            Process[] process = getProcess(process1);
            if (process.Length > 0)
            {
                return process;
            }

            process = getProcess(process2);
            if (process.Length > 0)
            {
                return process;
            }

            if (process.Length > 0)
            {
                return process;
            }

            return getProcess("");
        }

        //关闭进程
        public void killProcess(bool stopIndicator)
        {
            LogCore.Write("killProcess");
            //传票结束
            if (stopIndicator && TaskCore.IsVoteTask())
            {
                LogCore.Write("stop vote!");
            }

            Process[] process = processCheck();
            if (process.Length > 0)
            {
                if (TaskCore.IsVoteTask())
                {
                    int counter = 1;
                    while (!Net.isOnline() && counter < 60)
                    {
                        counter++;
                        Thread.Sleep(500);
                    }
                }

                foreach (Process p in process)
                {
                    LogCore.Write("killProcess  :" + p);
                    try
                    {
                        p.Kill();
                    }
                    catch (Exception e)
                    {
                        LogCore.Write("killProcess  :" + e);
                    }
                }
            }
        }

        //切换任务流程
        private void taskChangeProcess(bool stopIndicator)
        {
            LogCore.Write("taskChangeProcess");
            killProcess(true);
            NetCore.RasOperate("disconnect");
            changeTask();
        }

        //切换任务
        private void changeTask()
        {
        }


        //通过路径启动进程
        public static void StartProcess(string pathName)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = pathName;
            info.Arguments = "";
            info.WorkingDirectory = pathName.Substring(0, pathName.LastIndexOf("\\"));
            info.WindowStyle = ProcessWindowStyle.Normal;
            Process pro = Process.Start(info);
            LogCore.Write("startProcess:" + pathName); //清空日志
            Thread.Sleep(500);
            //pro.WaitForExit();
        }
    }
}