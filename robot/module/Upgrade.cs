using System;
using System.IO;
using System.Text;
using System.Threading;
using robot.core;
using robot.util;

namespace robot.module
{
    public class Upgrade
    {
        //升级程序
        public static void Update()
        {
            Download();
            Execute();
            Environment.Exit(0);
        }

        //下载更新
        private static void Download()
        {
            if (!NetCore.IsRealOnline())
            {
                NetCore.Connect();
            }

            LogCore.Write("开始下载更新");
            string url = "http://bitcoinrobot.cn/file/handler.exe";
            string dlPath = string.Format("{0}\\handler-new.exe", PathCore.WorkingPath);
            string path = string.Format("{0}\\handler.exe", PathCore.WorkingPath);
            bool result;
            do
            {
                result = HttpDownLoad.Download(url, dlPath);
                if (!result)
                {
                    LogCore.Write("下载更新异常，1秒后重新下载");
                    Thread.Sleep(1000);
                }
            } while (!result);
        }

        //执行更新
        private static void Execute()
        {
            if (!File.Exists(string.Format("{0}\\update.bat", PathCore.WorkingPath)))
            {
                string line1 = "Taskkill /F /IM handler.exe";
                string line2 = "ping -n 3 127.0.0.1>nul";
                string line3 = "del /s /Q " + @"""" + Environment.CurrentDirectory + "\\handler.exe" + @"""";
                string line4 = "ping -n 3 127.0.0.1>nul";
                string line5 = @"ren """ + Environment.CurrentDirectory + @"\\handler-new.exe"" ""handler.exe""";
                string line6 = "ping -n 3 127.0.0.1>nul";
                string line7 = "start " + @""""" " + @"""" + Environment.CurrentDirectory + "\\handler.exe" + @"""";
                string[] lines = {"@echo off", line1, line2, line3, line4, line5, line6, line7};
                File.WriteAllLines(@"./update.bat", lines, Encoding.GetEncoding("GBK"));
            }

            ProgressCore.StartProcess(string.Format("{0}\\update.bat", PathCore.WorkingPath));
        }
    }
}