using System;
using System.Diagnostics;
using System.Threading;
using robot.util;

namespace robot.core
{
    public class ProgressCore
    {
        //通过进程名获取进程
        public static Process[] GetProcess(string proName)
        {
            return Process.GetProcessesByName(proName);
        }

        //获取项目进程
        private static Process[] ProcessCheck()
        {
            Process[] pros = GetProcess("AutoUpdate.dll");
            if (pros.Length > 0)
            {
                foreach (Process p in pros)
                {
                    p.Kill();
                }
            }

            string process1 = "vote.exe";
            string process2 = "register.exe";
            Process[] process = GetProcess(process1);
            if (process.Length > 0)
            {
                return process;
            }

            process = GetProcess(process2);
            if (process.Length > 0)
            {
                return process;
            }

            if (process.Length > 0)
            {
                return process;
            }

            return GetProcess("");
        }

        //关闭进程
        public static void KillProcess()
        {
            TaskCore taskCore = MonitorCore.GetTaskCore();
            Process[] process = ProcessCheck();
            if (process.Length > 0)
            {
                if (taskCore.IsVoteTask())
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