using handler.util;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace handler
{
    public partial class form1 : Form
    {
        private string sort;     //编号
        private string adslName;    //拨号名称
        private string workerId;    //工号
        private string id;
        private int delay;  //拨号延时
        private int overTime;   //超时
        private Thread main;    //主线程
        private Thread hwndThread;  //用来处理句柄阻塞事件线程
        private IntPtr threadHwnd;  //线程句柄
        private string taskName;    //任务名
        private string taskPath;    //任务路径
        private RASDisplay ras; //ADSL对象
        private bool ie8 = isIE8();

        private string workingPath = Environment.CurrentDirectory; //当前工作路径

        private const string TASK_SYS_UPDATE = "Update";
        private const string TASK_SYS_WAIT_ORDER = "待命";
        private const string TASK_SYS_SHUTDOWN = "关机";
        private const string TASK_SYS_RESTART = "重启";
        private const string TASK_SYS_NET_TEST = "网络测试";
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
            Hotkey.RegisterHotKey(this.Handle, 10, Hotkey.MODKEY.None, Keys.F10);
            Hotkey.RegisterHotKey(this.Handle, 11, Hotkey.MODKEY.None, Keys.F9);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                switch (m.WParam.ToInt32())
                {
                    case 10:
                        button2_Click(null, null);
                        button1_Click(null, null);
                        break;
                    case 11:
                        button2_Click(null, null);
                        break;
                    default:
                        break;
                }
                return;
            }
            base.WndProc(ref m);
        }
        

        //启动程序，检查配置文件，启动主线程
        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(@".\handler.ini"))
            {
                sort = IniReadWriter.ReadIniKeys("Base", "sort", "./cf.ini");
                adslName = IniReadWriter.ReadIniKeys("Base", "adslName", "./cf.ini");
                workerId = IniReadWriter.ReadIniKeys("Base", "workerId", "./cf.ini");
                try
                {
                    delay = int.Parse(IniReadWriter.ReadIniKeys("Base", "delay", "./cf.ini"));
                }
                catch (Exception)
                {
                    delay = 0;
                }
                textBox1.Text = sort.ToString();
                button1_Click(null, null);
            }
            else
            {
                IniReadWriter.WriteIniKeys("Base", "sort", textBox1.Text, "./cf.ini");
                IniReadWriter.WriteIniKeys("Base", "workerId", textBox2.Text, "./cf.ini");
                IniReadWriter.WriteIniKeys("Base", "adslName", textBox3.Text, "./cf.ini");
                IniReadWriter.WriteIniKeys("Base", "delay", textBox4.Text, "./cf.ini");
            }
            id = workerId + "-" + sort;
        }

        //改变编号
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            IniReadWriter.WriteIniKeys("Base", "sort", textBox1.Text, "./cf.ini");
        }

        //改变工号
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            IniReadWriter.WriteIniKeys("Base", "worker", textBox2.Text, "./cf.ini");
        }

        //改变拨号
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            IniReadWriter.WriteIniKeys("Base", "adslName", textBox3.Text, "./cf.ini");
        }

        //改变延时
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            IniReadWriter.WriteIniKeys("Base", "delay", textBox4.Text, "./cf.ini");
        }

        //启动主线程
        public void button1_Click(object sender, EventArgs e)
        {
            notifyIcon1.Icon =(Icon) Properties.Resources.ResourceManager.GetObject("running");
            button1.Enabled = false;
            button2.Enabled = true;
            writeLogs(workingPath + "/log.txt", "");//清空日志
            this.WindowState = FormWindowState.Minimized;
            main = new Thread(_main);
            main.Start();
        }


        //终止主线程

        private void mainThreadClose()
        {
            notifyIcon1.Icon = (Icon)Properties.Resources.ResourceManager.GetObject("stop");
            button2.Enabled = false;
            button1.Enabled = true;
            main.Abort();
        }

        //点击停止
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
            mainThreadClose();
        }

        //关闭程序
        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainThreadClose();
        }

        //双击托盘
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        //写txt
        private void writeLogs(string pathName, string content)
        {
            if (content.Equals(""))
            {
                StreamWriter sw = new StreamWriter(pathName);
                sw.Write("");
                sw.Close();
            }
            else
            {
                StreamWriter sw = File.AppendText(pathName);
                sw.WriteLine(content);
                sw.Close();
            }
        }


        //判断当前是否为系统任务
        private bool isSysTask()
        {
            return taskName.Equals(TASK_SYS_UPDATE) || taskName.Equals(TASK_SYS_WAIT_ORDER) || taskName.Equals(TASK_SYS_SHUTDOWN) || taskName.Equals(TASK_SYS_RESTART) || taskName.Equals(TASK_SYS_NET_TEST);
        }

        //判断当前是否为投票项目
        private bool isVoteTask()
        {
            return taskName.Equals(TASK_VOTE_JIUTIAN) || taskName.Equals(TASK_VOTE_YUANQIU) || taskName.Equals(TASK_VOTE_MM) || taskName.Equals(TASK_VOTE_ML) || taskName.Equals(TASK_VOTE_JZ) || taskName.Equals(TASK_VOTE_JT) || taskName.Equals(TASK_VOTE_DM) || taskName.Equals(TASK_VOTE_OUTDO);
        }


        //通过进程名获取进程
        private Process[] getProcess(string proName)
        {
            //writeLogs(workingPath + "/log.txt", "getProcess:" + proName);
            if (StringUtil.isEmpty(proName) && !StringUtil.isEmpty(taskPath))
            {
                proName = taskPath.Substring(taskPath.LastIndexOf("\\") + 1);
            }
            proName = proName.Replace(".exe", "");
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
        private void killProcess(bool stopIndicator)
        {
            writeLogs(workingPath + "/log.txt", "killProcess");
            //传票结束
            if (stopIndicator && isVoteTask())
            {
                writeLogs(workingPath + "/log.txt", "stop vote!");
                if (taskName.Equals(TASK_VOTE_JIUTIAN))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                        hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "");
                        hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "结束投票");
                        writeLogs(workingPath + "/log.txt", "九天结束 句柄为" + hwnd);
                        HwndUtil.clickHwnd(hwnd);
                        int s = 0;
                        IntPtr hwndEx = IntPtr.Zero;
                        do
                        {
                            Thread.Sleep(500);
                            hwnd = HwndUtil.FindWindow("WTWindow", null);
                            hwndEx = HwndUtil.FindWindow("#32770", "信息：");
                            if (s % 10 == 0&& hwnd != IntPtr.Zero)
                            {
                                IntPtr hwnd0 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                                hwnd0 = HwndUtil.FindWindowEx(hwnd0, IntPtr.Zero, "Button", "");
                                hwnd0 = HwndUtil.FindWindowEx(hwnd0, IntPtr.Zero, "Button", "结束投票");
                                HwndUtil.clickHwnd(hwnd0);
                            }
                            if (hwndEx != IntPtr.Zero)
                            {
                                HwndUtil.closeHwnd(hwndEx);
                                s = 90;
                            }
                            s++;
                        } while (hwnd != IntPtr.Zero && s < 90);
                    }
                }else if (taskName.Equals(TASK_VOTE_YUANQIU))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("TForm1", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TButton", "停止");
                        while (!Net.isOnline())
                        {
                            Thread.Sleep(500);
                        }
                        createHwndThread(hwndEx);
                        Thread.Sleep(5000);
                    }
                    
                }
                else if (taskName.Equals(TASK_VOTE_JZ))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("TMainForm", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr hwndTGroupBox = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "当前状态");
                        IntPtr hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, IntPtr.Zero, "TButton", "停 止");
                        while (!Net.isOnline())
                        {
                            Thread.Sleep(500);
                        }
                        createHwndThread(hwndEx);
                        Thread.Sleep(5000);
                    }
                }
                else if (taskName.Equals(TASK_VOTE_JT))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("ThunderRT6FormDC", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "停止投票");
                        while (!Net.isOnline())
                        {
                            Thread.Sleep(500);
                        }
                        createHwndThread(hwndEx);
                        Thread.Sleep(5000);
                    }
                }
                else if (taskName.Equals(TASK_VOTE_MM))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "停止投票");
                        while (!Net.isOnline())
                        {
                            Thread.Sleep(500);
                        }
                        createHwndThread(hwndEx);
                        int s = 0;
                        IntPtr hwndTip;
                        do
                        {
                            s++;
                            hwndTip = HwndUtil.FindWindow(null, "投票软件提示");
                            Thread.Sleep(500);
                        } while (hwndTip == IntPtr.Zero && s < 60);
                    }
                }
            }
            Process[] process = processCheck();
            if (process.Length > 0)
            {
                if (isVoteTask())
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
                    writeLogs(workingPath + "/log.txt", "killProcess  :" + p.ToString());
                    try
                    {
                        p.Kill();
                    }
                    catch (Exception e) {
                        writeLogs(workingPath + "/log.txt", "killProcess  :" + e.ToString());
                    }
                }
            }
        }

        //切换任务流程
        private void taskChangeProcess(bool stopIndicator)
        {
            writeLogs(workingPath + "/log.txt", "taskChangeProcess");
            killProcess(getStopIndicator());
            rasOperate("disconnect");
            changeTask();
        }

        //切换任务
        private void changeTask()
        {
           
            taskMonitor();
        }


        //hwndThread创建
        private void createHwndThread(IntPtr hwnd)
        {
            threadHwnd = hwnd;
            hwndThread = new Thread(clickHwndByThread);
            hwndThread.Start();
        }

        //处理句柄操作线程
        private void clickHwndByThread()
        {
            HwndUtil.clickHwnd(threadHwnd);
        }

        //圆球启动
        private void yuanqiuStart()
        {
            IntPtr hwnd = IntPtr.Zero;
            do
            {
                hwnd = HwndUtil.FindWindow("TForm1", null);
                Thread.Sleep(1000);
            }
            while (hwnd == IntPtr.Zero);
            Thread.Sleep(1000);
            //设置拨号延迟
            IntPtr hwndTGroupBox = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "设置");
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, IntPtr.Zero, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            HwndUtil.setText(hwndEx, (delay / 1000).ToString());
            //设置工号
            hwndTGroupBox = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "会员");
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, IntPtr.Zero, "TEdit", null);
            HwndUtil.setText(hwndEx, id);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            HwndUtil.setText(hwndEx, id);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            HwndUtil.setText(hwndEx, id);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, hwndEx, "TEdit", null);
            //开始投票
            hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TButton", "开始");
            createHwndThread(hwndEx);
        }

        //JZ启动
        private void jzStart()
        {
            IntPtr hwnd = IntPtr.Zero;
            do
            {
                hwnd = HwndUtil.FindWindow("TMainForm", null);
                Thread.Sleep(500);
            }
            while (hwnd == IntPtr.Zero);
            //设置拨号延迟

            IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwnd, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwnd, hwndEx, "TEdit", null);
            HwndUtil.setText(hwndEx, (delay / 1000).ToString());
            //设置工号
            IntPtr hwndTGroupBox0 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "会员选项");
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox0, IntPtr.Zero, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox0, hwndEx, "TEdit", null);
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox0, hwndEx, "TEdit", null);
            HwndUtil.setText(hwndEx, id);
            //开始投票
            IntPtr hwndTGroupBox = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "当前状态");
            hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, IntPtr.Zero, "TButton", "开 始");
            createHwndThread(hwndEx);
        }

        //JT启动
        private void jtStart()
        {
            IntPtr hwnd = IntPtr.Zero;
            do
            {
                hwnd = HwndUtil.FindWindow("ThunderRT6FormDC", null);
                Thread.Sleep(500);
            }
            while (hwnd == IntPtr.Zero);
            //设置拨号延迟
            IntPtr ThunderRT6Frame = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "ThunderRT6Frame", "设置");
            IntPtr hwndEx = HwndUtil.FindWindowEx(ThunderRT6Frame, IntPtr.Zero, "ThunderRT6TextBox", null);
            hwndEx = HwndUtil.FindWindowEx(ThunderRT6Frame, hwndEx, "ThunderRT6TextBox", null);
            HwndUtil.setText(hwndEx, (delay / 1000).ToString());
            //设置工号
            ThunderRT6Frame = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "ThunderRT6Frame", "会员");
            hwndEx = HwndUtil.FindWindowEx(ThunderRT6Frame, IntPtr.Zero, "ThunderRT6TextBox", null);
            HwndUtil.setText(hwndEx, id);
            //开始投票
            hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "自动投票");
            createHwndThread(hwndEx);
        }

        //MM启动
        private void mmStart()
        {
            IntPtr hwnd = IntPtr.Zero;
            do
            {
                hwnd = HwndUtil.FindWindow("WTWindow", null);
                Thread.Sleep(500);
            }
            while (hwnd == IntPtr.Zero);
            //设置拨号延迟
            IntPtr ButtonHwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "设置");
            IntPtr hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, IntPtr.Zero, "Edit", "3");
            if (hwndEx == IntPtr.Zero)
            {
                hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, IntPtr.Zero, "Edit", "4");
            }
            HwndUtil.setText(hwndEx, (delay / 1000).ToString());
            //设置工号
            ButtonHwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "会员");
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, IntPtr.Zero, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            hwndEx = HwndUtil.FindWindowEx(ButtonHwnd, hwndEx, "Edit", null);
            HwndUtil.setText(hwndEx, id);
            //开始投票
            hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "自动投票");
            createHwndThread(hwndEx);
        }

        //九天启动
        private void jiutianStart()
        {
            IntPtr hwnd = IntPtr.Zero;
            IntPtr hwndSysTabControl32 = IntPtr.Zero;
            IntPtr preparedCheck = IntPtr.Zero;
            IntPtr startButton = IntPtr.Zero;
            do
            {
                hwnd = HwndUtil.FindWindow("WTWindow", null);
                hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                preparedCheck = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "工作情况");
                preparedCheck = HwndUtil.FindWindowEx(preparedCheck, IntPtr.Zero, "Afx:400000:b:10011:1900015:0", "加载成功 可开始投票");
                startButton = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "");
                startButton = HwndUtil.FindWindowEx(startButton, IntPtr.Zero, "Button", "开始投票");
                Thread.Sleep(500);
            }
            while (preparedCheck == IntPtr.Zero || startButton == IntPtr.Zero);
            //设置拨号延迟
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "拨号设置");
            hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "SysTabControl32", "");
            hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Edit", null);
            HwndUtil.setText(hwndEx, delay.ToString());
            //设置工号
            hwndEx = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "请输入工号");
            hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Edit", null);
            HwndUtil.setText(hwndEx, id);
            HwndUtil.clickHwnd(startButton);
            Thread.Sleep(500);
        }

        //通过路径启动进程
        private void startProcess(string pathName)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = pathName;
            info.Arguments = "";
            info.WorkingDirectory = pathName.Substring(0, pathName.LastIndexOf("\\"));
            info.WindowStyle = ProcessWindowStyle.Normal;
            Process pro = Process.Start(info);
            writeLogs(workingPath + "/log.txt", "startProcess:" + pathName);//清空日志
            Thread.Sleep(500);
            //pro.WaitForExit();
        }

        //升级程序
        private void updateSoft()
        {
            string path = "";
            string line1 = "Taskkill /F /IM " + path.Substring(path.LastIndexOf("\\") + 1);
            string line2 = "ping -n 3 127.0.0.1>nul";
            string line3 = "copy / y " + path + @" """ + workingPath + @"""";
            string line4 = "ping -n 3 127.0.0.1>nul";
            string line5 = "start " + path.Substring(path.LastIndexOf("\\") + 1);
            string[] lines = { "@echo off", line1, line2, line3, line4, line5};
            try
            {
                File.WriteAllLines(@"./自动升级.bat", lines, Encoding.GetEncoding("GBK"));
                startProcess(workingPath + @"\自动升级.bat");
                mainThreadClose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //显示通知
        private void showNotification(string content, ToolTipIcon toolTipIcon)
        {
            notifyIcon1.ShowBalloonTip(0, content, DateTime.Now.ToLocalTime().ToString(), toolTipIcon);

        }


        //检测IE版本
        private static bool isIE8()
        {
            RegistryKey mreg;
            mreg = Registry.LocalMachine;
            mreg = mreg.CreateSubKey("software\\Microsoft\\Internet Explorer");
            return mreg.GetValue("Version").ToString().Substring(0,1)=="8";
        }

        //ADSL操作
        private void rasOperate(string type)
        {

            if (type.Equals("connect"))
            {
                if (ie8)
                {
                    Thread.Sleep(200);
                    Thread rasThread = new Thread(rasConnect);
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
                            IntPtr hwnd = HwndUtil.FindWindow("#32770", "连接到 " + adslName + " 时出错");
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
                    ras.Connect(adslName);
                }
            }
            else
            {
                ras = new RASDisplay();
                ras.Disconnect();
            }
        }

        //ras子线程，处理IE8线程阻塞
        private void rasConnect()
        {
            writeLogs(workingPath + "/log.txt", "rasConnect");//清空日志
            ras = new RASDisplay();
            ras.Disconnect();
            ras.Connect(adslName);

        }

        //网络检测
        private void netCheck()
        {
            showNotification("正在初始化网络...", ToolTipIcon.Info);
            bool online = Net.isOnline();
            if (!online)
            {
                rasOperate("connect");
                Thread.Sleep(1000);
                online = Net.isOnline();
                if (!online)
                {
                    rasOperate("connect");
                    Thread.Sleep(1000);
                    online = Net.isOnline();
                    if (!online)
                    {
                    }
                }
            }
        }

        //到票待命
        private void switchWatiOrder()
        {
        }

        //圆球到票检测
        private bool yuanqiuOverCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow("TMessageForm", "register");
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
                return true;
            }
            return false;
        }

        //JZ到票检测
        private bool jzOverCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow("TMessageForm", null);
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
                return true;
            }
            return false;
        }

        //JT到票检测
        private bool jtOverCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow(null, "VOTETOOL");
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
                return true;
            }
            return false;
        }

        //MM到票检测
        private bool mmOverCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow(null, "投票软件提示");
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
                return true;
            }
            return false;
        }

        //九天到票检测
        private bool jiutianOverCheck(ref int s)
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            if (hwnd == IntPtr.Zero)
            {
                s++;
            }
            else
            {
                s = 0;
            }
            if (s > 5)
            {
                return true;
            }
            return false;
        }

      
        //九天限人检测
        private bool jiutianRestrictCheck()
        {
            IntPtr hwnd = HwndUtil.FindWindow("#32770", "信息提示");
            if (hwnd != IntPtr.Zero)
            {
                HwndUtil.closeHwnd(hwnd);
                return true;
            }
            return false;
        }
        //九天验证码输入检测
        private bool isIdentifyCode()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            IntPtr hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
            IntPtr testHwnd = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "输入验证码后回车,看不清直接回车切换");
            if (testHwnd != IntPtr.Zero)
            {
                killProcess(false);
                rasOperate("disconnect");
                return true;
            }
            return false;
        }

        //获取 是否需要传票关闭
        private bool getStopIndicator()
        {
            if (taskName.Equals(TASK_VOTE_JIUTIAN))
            {
                IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
                if (hwnd != IntPtr.Zero)
                {
                    IntPtr hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                    IntPtr hwndStat = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "投票统计");
                    IntPtr hwndEx = HwndUtil.FindWindowEx(hwndStat, IntPtr.Zero, "Afx:400000:b:10011:1900015:0", "运行时间");
                    hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                    hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                    hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                    StringBuilder unUpload = new StringBuilder(512);
                    HwndUtil.GetWindowText(hwndEx, unUpload, unUpload.Capacity);
                    return int.Parse(unUpload.ToString()) > 0;
                }
                return false;
                
            }else
            {
                return true;
            }
            
        }

        //获取九天成功数
        private int getJiutianSucc()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            IntPtr hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
            IntPtr hwndStat = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "投票统计");
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndStat, IntPtr.Zero, "Afx:400000:b:10011:1900015:0", "超时票数");
            hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
            try
            {
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                StringBuilder succ = new StringBuilder(512);
                HwndUtil.GetWindowText(hwndEx, succ, 512);
                return int.Parse(succ.ToString());
            }
            catch (Exception) { }
            return 0;

        }

        //九天成功检测
        private bool jiutianFailTooMuch()
        {
            IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
            IntPtr hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
            IntPtr hwndStat = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "投票统计");
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndStat, IntPtr.Zero, "Afx:400000:b:10011:1900015:0", "超时票数");
            hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
            StringBuilder duration = new StringBuilder(512);
            HwndUtil.GetWindowText(hwndEx, duration, duration.Capacity);
            int min;
            try
            {
                min = int.Parse(duration.ToString().Split('：')[1]);
            }
            catch (Exception)
            {
                min=0;
            }
            if (min >= 2)
            {
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                hwndEx = HwndUtil.FindWindowEx(hwndStat, hwndEx, "Afx:400000:b:10011:1900015:0", null);
                StringBuilder succ = new StringBuilder(512);
                HwndUtil.GetWindowText(hwndEx, succ, 512);
                int success = int.Parse(succ.ToString());
                if (success / min < 2)
                {
                    writeLogs(workingPath + "/log.txt", "Fail Too Much ---> success:" + success + ",min:" + min);//清空日志
                    return true;
                }
            }
            return false;
        }

        //清理托盘
        private void refreshIcon()
        {
            Rectangle rect = new Rectangle();
            rect = Screen.GetWorkingArea(this);
            int startX = rect.Width - 200;//屏幕高
            int startY = rect.Height + 20;//屏幕高
            for (; startX < rect.Width; startX += 3)
            {
                MouseKeyboard.SetCursorPos(startX, startY);
            }
        }

        //任务监控
        private void taskMonitor()
        {
            refreshIcon();
            overTime =30;
            if (overTime % 2 == 1)
            {
                overTime += 1;
            }
            overTime = overTime / 2 - 1;
            int p = 0;
            int s = 0;
            bool isOnline = false;
            int circle = 0;
            do
            {
                if (ie8)
                {
                    IntPtr adslErr = HwndUtil.FindWindow("#32770", "连接到 " + adslName + " 时出错");
                    if (adslErr != IntPtr.Zero)
                    {
                        HwndUtil.closeHwnd(adslErr);
                    }
                }
                isOnline = Net.isOnline();
                if ("taskChange".Equals("1"))
                {
                    taskChangeProcess(true);
                    return;
                }
                if (isSysTask())
                {
                    p = 0;
                }
                if (taskName.Equals(TASK_VOTE_JIUTIAN) && p > 0)
                {
                    if (jiutianOverCheck(ref s) || jiutianRestrictCheck())
                    {
                        switchWatiOrder();
                    }
                }
                else if (taskName.Equals(TASK_VOTE_MM))
                {
                    if (mmOverCheck())
                    {
                        killProcess(false);
                        switchWatiOrder();
                    }
                }
                else if (taskName.Equals(TASK_VOTE_YUANQIU))
                {
                    if (yuanqiuOverCheck())
                    {
                        killProcess(false);
                        switchWatiOrder();
                    }
                }
                else if (taskName.Equals(TASK_VOTE_JT))
                {
                    if (jtOverCheck())
                    {
                        killProcess(false);
                        switchWatiOrder();
                    }
                }
                else if (taskName.Equals(TASK_VOTE_ML))
                {
                    //ML到票检测
                }
                else if (taskName.Equals(TASK_VOTE_DM))
                {
                    //DM到票检测
                }
                else if (taskName.Equals(TASK_VOTE_JZ))
                {
                    if (jzOverCheck())
                    {
                        killProcess(false);
                        switchWatiOrder();
                    }
                }
                else if (taskName.Equals(TASK_VOTE_OUTDO))
                {
                    //OUTDO到票检测
                }
                if (isOnline)
                {
                    p = p < 0 ? 1 : ++p;
                }
                else
                {
                    circle++;
                    p = p > 0 ? -1 : --p;

                }
                label2.Text = p.ToString();
                Thread.Sleep(2000);
            }
            while (p == 0 || (p > 0 && p < overTime) || (p < 0 && p > -overTime));
            if (taskName.Equals(TASK_VOTE_MM))
            {
                rasOperate("disconnect");
                //启动拨号定时timer
                rasOperate("connect");
            }
            else
            {
                taskChangeProcess(false);
                return;
            }


            taskMonitor();
        }

        //主线程
        public void _main()
        {
            //进程初始化部分
            string now = DateTime.Now.ToLocalTime().ToString();
                try
                {
                    taskMonitor();
                }
                catch (ThreadAbortException)
                {

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
        }
}
