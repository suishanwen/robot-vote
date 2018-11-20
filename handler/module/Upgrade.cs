using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using handler.core;

namespace handler.module
{
    public class Upgrade
    {
        //升级程序
        public static void Update()
        {
            string path = "";
            string line1 = "Taskkill /F /IM " + path.Substring(path.LastIndexOf("\\") + 1);
            string line2 = "ping -n 3 127.0.0.1>nul";
            string line3 = "copy / y " + path + @" """ + PathCore.WorkingPath + @"""";
            string line4 = "ping -n 3 127.0.0.1>nul";
            string line5 = "start " + path.Substring(path.LastIndexOf("\\") + 1);
            string[] lines = {"@echo off", line1, line2, line3, line4, line5};
            try
            {
                File.WriteAllLines(@"./自动升级.bat", lines, Encoding.GetEncoding("GBK"));
                ProgressCore.StartProcess(PathCore.WorkingPath + @"\自动升级.bat");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}