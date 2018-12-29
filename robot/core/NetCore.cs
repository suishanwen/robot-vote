using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using robot.util;

namespace robot.core
{
    public class NetCore
    {
        private static RASDisplay ras; //ADSL对象

        private static readonly bool ie8 = IsIE8();


        //检测IE版本
        private static bool IsIE8()
        {
            RegistryKey mreg;
            mreg = Registry.LocalMachine;
            try
            {
                mreg = mreg.CreateSubKey("software\\Microsoft\\Internet Explorer");
            }
            catch (Exception)
            {
                return false;
            }

            return mreg.GetValue("Version").ToString().Substring(0, 1) == "8";
        }


        private static void ErrReconnect()
        {
            IntPtr adslErr = HwndUtil.FindWindow("#32770", "连接到 " + ConfigCore.AdslName + " 时出错");
            if (adslErr != IntPtr.Zero)
            {
                IntPtr hwndEx = HwndUtil.FindWindowEx(adslErr, IntPtr.Zero, "Button", null);
                string title = HwndUtil.GetControlText(hwndEx);
                if (title.IndexOf("重拨") != -1)
                {
                    LogCore.Write($"{ConfigCore.AdslName}拨号出错，重播");
                    HwndUtil.clickHwnd(hwndEx);
                }
            }
        }
        
        public static void CloseException()
        {
            IntPtr adslExcp = HwndUtil.FindWindow("#32770", "网络连接");
            if (adslExcp != IntPtr.Zero)
            {
                IntPtr hwndEx = HwndUtil.FindWindowEx(adslExcp, IntPtr.Zero, "Button", "确定");
                if (hwndEx != IntPtr.Zero)
                {
                    HwndUtil.clickHwnd(hwndEx);
                }
            }

            ErrReconnect();
        }

        public static void Connect()
        {
            if (!Net.IsOnline())
            {
                RasOperate("connect");
            }
        }

        public static void DisConnect()
        {
            if (!ConfigCore.IsAdsl)
            {
                RasOperate("disconnect");
            }
        }

        //ADSL操作
        private static void RasOperate(string type)
        {
            if (type.Equals("connect"))
            {
                if (ie8)
                {
                    Thread.Sleep(200);
                    new Thread(RasConnect).Start();
                    bool online = false;
                    bool err = false;
                    int count = 0;
                    do
                    {
                        online = Net.IsOnline();
                        if (!online)
                        {
                            Thread.Sleep(500);
                            ErrReconnect();
                        }

                        count++;
                    } while (!online && !err);
                }
                else
                {
                    ras = new RASDisplay();
                    ras.Connect(ConfigCore.AdslName);
                }
            }
            else
            {
                ras = new RASDisplay();
                ras.Disconnect();
            }
        }

        //ras子线程，处理IE8线程阻塞
        private static void RasConnect()
        {
            ras = new RASDisplay();
            ras.Disconnect();
            ras.Connect(ConfigCore.AdslName);
        }

        //网络检测
        public static void NetCheck()
        {
            bool online = Net.IsOnline();
            if (!online)
            {
                RasOperate("connect");
                Thread.Sleep(1000);
                online = Net.IsOnline();
                if (!online)
                {
                    RasOperate("connect");
                    Thread.Sleep(1000);
                    online = Net.IsOnline();
                    if (!online)
                    {
                        string exception = ConfigCore.GetBaseConfig("exception");
                        if (exception == "1")
                        {
                            ConfigCore.NetError("error");
                        }
                        else
                        {
                            ConfigCore.WriteBaseConfig("exception", "1");
                            Process.Start("shutdown.exe", "-r -t 0");
                            Form1.MainClose();
                        }
                    }
                }

                ConfigCore.WriteBaseConfig("exception", "0");
                string arrDrop = ConfigCore.GetConfig("ArrDrop");
                if (!StringUtil.isEmpty(arrDrop))
                {
                    arrDrop = " " + arrDrop;
                }
                if (arrDrop.IndexOf(" " + ConfigCore.Sort + " |") != -1)
                {
                    ConfigCore.WriteConfig("ArrDrop", arrDrop.Replace(" " + ConfigCore.Sort + " |", ""));
                }
            }
        }

        //网络检测
        public static bool IsRealOnline()
        {
            return Net.IsRealOnline();
        }
    }
}