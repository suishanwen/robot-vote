using System;
using System.Threading;
using Microsoft.Win32;
using robot.util;

namespace robot.core
{
    public class NetCore
    {
        private static RASDisplay ras; //ADSL对象

        private static readonly bool ie8 = isIE8();

        //检测IE版本
        private static bool isIE8()
        {
            RegistryKey mreg;
            mreg = Registry.LocalMachine;
            mreg = mreg.CreateSubKey("software\\Microsoft\\Internet Explorer");
            return mreg.GetValue("Version").ToString().Substring(0, 1) == "8";
        }

        //ADSL操作
        public static void RasOperate(string type)
        {
            if (type.Equals("connect"))
            {
                if (ie8)
                {
                    Thread.Sleep(200);
                    Thread rasThread = new Thread(RasConnect);
                    rasThread.Start();
                    bool online = false;
                    bool err = false;
                    int count = 0;
                    do
                    {
                        online = Net.isOnline();
                        if (!online)
                        {
                            Thread.Sleep(500);
                            IntPtr hwnd = HwndUtil.FindWindow("#32770", "连接到 " + ConfigCore.AdslName + " 时出错");
                            if (hwnd != IntPtr.Zero)
                            {
                                HwndUtil.closeHwnd(hwnd);
                                err = true;
                            }
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
            LogCore.Write("rasConnect"); //清空日志
            ras = new RASDisplay();
            ras.Disconnect();
            ras.Connect(ConfigCore.AdslName);
        }

        //网络检测
        public static bool NetCheck()
        {
            bool online = Net.isOnline();
            if (!online)
            {
                RasOperate("connect");
                Thread.Sleep(1000);
                online = Net.isOnline();
                if (!online)
                {
                    RasOperate("connect");
                    Thread.Sleep(1000);
                    online = Net.isOnline();
                    if (!online)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}