using handler.util;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace handler
{
    public partial class form1 : Form
    {
        private string pathShare;
        private int no;
        private int delay;
        private int overTime;
        private Thread main;
        private string taskName;
        private string taskPath;
        private string taskChange;
        private string cacheMemory;
        private string workerId;
        private string inputId;
        private string tail;


        

        public form1()
        {
            InitializeComponent(); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Net.isOnline();
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
                overTime = int.Parse(IniReadWriter.ReadIniKeys("Command", "cishu", pathShare + "/CF.ini"));
                button1_Click(null, null);
            }
            else
            {
                MessageBox.Show("请设置handler.ini");
                Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            IniReadWriter.WriteIniKeys("Command", "bianhao", textBox1.Text, "./handler.ini");
        }
        private void taskChangeProcess()
        {

        }
        private void changeTask()
        {

        }
        private bool processCheck()
        {
            return true;
        }
        private void netCheck() {

        }
        private void netMonitor()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            Console.WriteLine(hwnd);
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                Console.WriteLine(hwndEx);
                hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Button", "");
                Console.WriteLine(hwndEx);
                hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Button", "开始投票");
                Console.WriteLine(hwndEx);
                HwndUtil.clickHwnd(hwndEx);
            }
            Thread.Sleep(2000);
            netMonitor();
        }
        public void _main()
        {
            //进程初始化部分
            string now = DateTime.Now.ToLocalTime().ToString();
            taskChange = IniReadWriter.ReadIniKeys("Command", "TaskChange"+no, pathShare + "/Task.ini");
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
                    taskName = arr[0].Substring(9, arr[0].Length);
                    taskPath = arr[1].Substring(9, arr[1].Length);
                    workerId = arr[2].Substring(0, arr[2].Length);
                    if (!StringUtil.isEmpty(workerId))
                    {
                        inputId = "1";
                        tail = "1";
                    }
                }
                IniReadWriter.WriteIniKeys("Command", "CacheMemory" + no, "", pathShare + "/TaskPlus.ini");
                if (processCheck())
                {
                    notifyIcon1.ShowBalloonTip(0, now, taskName + "运行中,进入维护状态", ToolTipIcon.Info);
                }
                else
                {
                    if (taskPath.Equals("Writein"))
                    {
                        notifyIcon1.ShowBalloonTip(0, now, "发现项目缓存,通过写入路径启动"+ taskName, ToolTipIcon.Info);
                    }
                    else
                    {
                        notifyIcon1.ShowBalloonTip(0, now, "发现项目缓存,通过自定义路径启动" + taskName, ToolTipIcon.Info);
                    }
                    changeTask();
                }

            }
            //无缓存待命
            netCheck();
            taskName = "待命";
            notifyIcon1.ShowBalloonTip(0, now, "未发现项目缓存,待命中...\n请通过控制与监控端启动"+no+"号虚拟机", ToolTipIcon.Info);
            netMonitor();
            
        }

        public void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            main = new Thread(_main);
            main.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
            main.Abort();
        }

        private void cache()
        {

        }

        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cache();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
    }
}
