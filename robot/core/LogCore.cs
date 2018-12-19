using System;
using System.IO;

namespace robot.core
{
    public class LogCore
    {
        //写txt
        public static void Write(string content)
        {
            StreamWriter sw = File.AppendText($"{PathCore.WorkingPath}\\log.txt");
            sw.WriteLine($"{content} {DateTime.Now.ToLocalTime().ToString()}");
            sw.Close();
        }

        public static void Clear()
        {
            StreamWriter sw = new StreamWriter($"{PathCore.WorkingPath}\\log.txt");
            sw.Write("");
            sw.Close();
        }
    }
}