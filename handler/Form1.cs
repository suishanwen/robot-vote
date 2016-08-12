using handler.util;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace handler
{
    public partial class form1 : Form
    {
        private string pathShare;   //共享路径
        private int no;     //编号
        private int delay;  //拨号延时
        private int overTime;   //超时
        private Thread main;    //主线程
        private string taskName;    //任务名
        private string taskPath;    //任务路径
        private string customPath;  //本地任务路径
        private string taskChange;  //任务切换标识
        private string cacheMemory; //项目缓存
        private string workerId;    //工号
        private string inputId;     //输入工号标识
        private string tail;    //分号标识

        private const string TASK_SYS_UPDATE = "Update";
        private const string TASK_SYS_WAIT_ORDER = "待命";
        private const string TASK_SYS_SHUTDOWN = "关机";
        private const string TASK_SYS_RESTART = "重启";
        private const string TASK_SYS_NET_TEST = "网络测试";
        private const string TASK_HANGUP_MM2 = "mm2";
        private const string TASK_HANGUP_YUKUAI = "yukuai";
        private const string TASK_VOTE_JIUTIAN = "九天";
        private const string TASK_VOTE_YUANQIU = "圆球";
        private const string TASK_VOTE_MM = "MM";
        private const string TASK_VOTE_JT = "JT";
        private const string TASK_VOTE_ML = "ML";
        private const string TASK_VOTE_DM = "DM";
        private const string TASK_VOTE_JZ = "JZ";
        private const string TASK_VOTE_OUTDO = "Outdo";


        public form1()
        {
            InitializeComponent();
        }

        //启动程序，检查配置文件，启动主线程
        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(@".\handler.ini"))
            {
                pathShare = IniReadWriter.ReadIniKeys("Command", "gongxiang", "./handler.ini");
                no = int.Parse(IniReadWriter.ReadIniKeys("Command", "bianhao", "./handler.ini"));
                try
                {
                    delay = int.Parse(IniReadWriter.ReadIniKeys("Command", "yanchi", "./handler.ini"));
                }
                catch (Exception)
                {
                    delay = 0;
                }
                textBox1.Text = no.ToString();
                button1_Click(null, null);
            }
            else
            {
                MessageBox.Show("请设置handler.ini");
                Close();
            }
        }

        //改变编号
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            IniReadWriter.WriteIniKeys("Command", "bianhao", textBox1.Text, "./handler.ini");
        }

        //启动主线程
        public void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            main = new Thread(_main);
            main.Start();
        }

        //终止主线程
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
            main.Abort();
        }

        //关闭程序，缓存项目
        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cache();
        }

        //双击托盘
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }


        private bool isSysTask()
        {
            return taskName.Equals(TASK_SYS_UPDATE) || taskName.Equals(TASK_SYS_WAIT_ORDER) || taskName.Equals(TASK_SYS_SHUTDOWN) || taskName.Equals(TASK_SYS_RESTART) || taskName.Equals(TASK_SYS_NET_TEST);
        }

        //缓存
        private void cache()
        {
            if (isSysTask())
            {
                return;
            }
            string path = "";
            if (customPath.Equals(taskPath))
            {
                path = taskPath;
            }
            else
            {
                path = "Writein";
            }
            cacheMemory = "TaskName-" + taskName + "`TaskPath-" + path + "`Worker:" + workerId;
            IniReadWriter.WriteIniKeys("Command", "CacheMemory" + no, cacheMemory, pathShare + "/CF.ini");
        }


        //切换任务流程
        private void taskChangeProcess()
        {

        }

        //切换任务
        private void changeTask()
        {

        }

        //获取进程
        private Process[] getProcess(string proName)
        {
            if (StringUtil.isEmpty(proName))
            {
                proName = taskPath.Substring(taskPath.LastIndexOf("/") + 1);
            }
            return Process.GetProcessesByName(proName);
        }

        //网络检测
        private bool netCheck()
        {
            return Net.isOnline();
            //IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            //Console.WriteLine(hwnd);
            //if (hwnd != IntPtr.Zero)
            //{
            //    IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
            //    Console.WriteLine(hwndEx);
            //    hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Button", "");
            //    Console.WriteLine(hwndEx);
            //    hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Button", "开始投票");
            //    Console.WriteLine(hwndEx);
            //    HwndUtil.clickHwnd(hwndEx);
            //}
        }

        //任务监控
        private void taskMonitor()
        {
            overTime = int.Parse(IniReadWriter.ReadIniKeys("Command", "cishu", pathShare + "/CF.ini"));
            if (taskName.Equals(TASK_HANGUP_MM2) || taskName.Equals(TASK_HANGUP_YUKUAI))
            {
                overTime -= 2;
            }
            if (overTime % 2 == 1)
            {
                overTime += 1;
            }
            overTime = overTime / 2 - 1;
            int p = 0;
            bool isOnline = false;
            do
                try
                {
                    isOnline = netCheck();
                    taskChange = IniReadWriter.ReadIniKeys("Command", "taskChange", pathShare + "/CF.ini");
                    if (taskChange.Equals("1"))
                    {
                        //Goto 切换任务流程
                    }
                    if(isSysTask())
                    {
                        p = 0;
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    if (isOnline)
                    {
                        p = p < 0 ? 1 : p++;
                    }
                    else
                    {
                        p = p > 0 ? -1 : p--;

                    }
                    Thread.Sleep(2000);
                }
            while (p < overTime || p > -overTime);
            taskMonitor();
        }

        //主线程
        public void _main()
        {
            //进程初始化部分
            string now = DateTime.Now.ToLocalTime().ToString();
            taskChange = IniReadWriter.ReadIniKeys("Command", "TaskChange" + no, pathShare + "/Task.ini");
            if (taskChange.Equals("1"))
            {
                taskChangeProcess();
            }
            else
            {
                cacheMemory = IniReadWriter.ReadIniKeys("Command", "CacheMemory" + no, pathShare + "/TaskPlus.ini");
                if (!StringUtil.isEmpty(cacheMemory))
                {
                    string[] arr = cacheMemory.Split('`');
                    taskName = arr[0].Substring(9);
                    taskPath = arr[1].Substring(9);
                    workerId = arr[2].Substring(7);
                    if (!StringUtil.isEmpty(workerId))
                    {
                        inputId = "1";
                        tail = "1";
                    }
                    if (taskPath.Equals("Writein"))
                    {
                        if (taskName.Equals(TASK_HANGUP_MM2))
                        {
                            taskPath = IniReadWriter.ReadIniKeys("Command", "mm2", pathShare + "/CF.ini");
                        }
                        else if (taskName.Equals(TASK_HANGUP_YUKUAI))
                        {
                            taskPath = IniReadWriter.ReadIniKeys("Command", "yukuai", pathShare + "/CF.ini");
                        }
                    }
                    else
                    {
                        customPath = taskPath;
                    }

                    Process[] pros = getProcess(null);
                    if (pros.Length > 0)
                    {
                        notifyIcon1.ShowBalloonTip(0, now, taskName + "运行中,进入维护状态", ToolTipIcon.Info);
                    }
                    else
                    {
                        if (taskPath.Equals("Writein"))
                        {
                            notifyIcon1.ShowBalloonTip(0, now, "发现项目缓存,通过写入路径启动" + taskName, ToolTipIcon.Info);
                        }
                        else
                        {
                            notifyIcon1.ShowBalloonTip(0, now, "发现项目缓存,通过自定义路径启动" + taskName, ToolTipIcon.Info);
                        }
                        changeTask();
                    }
                    IniReadWriter.WriteIniKeys("Command", "CacheMemory" + no, "", pathShare + "/TaskPlus.ini");
                }
                else
                {
                    //无缓存待命
                    netCheck();
                    taskName = TASK_SYS_WAIT_ORDER;
                    notifyIcon1.ShowBalloonTip(0, now, "未发现项目缓存,待命中...\n请通过控制与监控端启动" + no + "号虚拟机", ToolTipIcon.Info);
                    taskMonitor();
                }
            }
        }
    }
}
