using handler.util;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        private string projectName;     //用于核对taskName
        private string taskPath;    //任务路径
        private string customPath;  //本地任务路径
        private string taskChange;  //任务切换标识
        private string cacheMemory; //项目缓存
        private string workerId;    //工号
        private string inputId;     //输入工号标识
        private string tail;    //分号标识
        private RASDisplay ras; //ADSL对象
        private string adslName;    //拨号名称

        private string workingPath = Environment.CurrentDirectory; //当前工作路径

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
        private const string TASK_VOTE_PROJECT = "投票项目";


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
                adslName = IniReadWriter.ReadIniKeys("Command", "adslName", "./handler.ini");
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

        private void mainThreadClose()
        {
            cache();
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

        //关闭程序，缓存项目
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

        //判断当前是否为系统任务
        private bool isSysTask()
        {
            return taskName.Equals(TASK_SYS_UPDATE) || taskName.Equals(TASK_SYS_WAIT_ORDER) || taskName.Equals(TASK_SYS_SHUTDOWN) || taskName.Equals(TASK_SYS_RESTART) || taskName.Equals(TASK_SYS_NET_TEST);
        }

        //判断当前是否为投票项目
        private bool isVoteTask()
        {
            return taskName.Equals(TASK_VOTE_JIUTIAN) || taskName.Equals(TASK_VOTE_YUANQIU) || taskName.Equals(TASK_VOTE_MM) || taskName.Equals(TASK_VOTE_ML) || taskName.Equals(TASK_VOTE_JZ) || taskName.Equals(TASK_VOTE_JT) || taskName.Equals(TASK_VOTE_DM) || taskName.Equals(TASK_VOTE_OUTDO) || taskName.Equals(TASK_VOTE_PROJECT);
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
            IniReadWriter.WriteIniKeys("Command", "CacheMemory" + no, cacheMemory, pathShare + "/TaskPlus.ini");
        }

        //通过进程名获取进程
        private Process[] getProcess(string proName)
        {
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
            return getProcess(null);
        }

        //关闭进程
        private void killProcess()
        {
            Process[] process = processCheck();
            if (process.Length > 0)
            {
                foreach (Process p in process)
                {
                    p.Kill();
                }
            }
        }

        //切换任务流程
        private void taskChangeProcess()
        {
            killProcess();
            rasOperate("disconnect");
            taskName = IniReadWriter.ReadIniKeys("Command", "TaskName" + no, pathShare + "/Task.ini");
            changeTask();
        }

        //切换任务
        private void changeTask()
        {
            if (taskChange.Equals("1"))
            {
                workerId = IniReadWriter.ReadIniKeys("Command", "worker", pathShare + "/CF.ini");
                inputId = IniReadWriter.ReadIniKeys("Command", "printgonghao", pathShare + "/CF.ini");
                tail = IniReadWriter.ReadIniKeys("Command", "tail", pathShare + "/CF.ini");
                customPath = IniReadWriter.ReadIniKeys("Command", "customPath" + no, pathShare + "/TaskPlus.ini");
            }
            if (StringUtil.isEmpty(workerId))
            {
                workerId = IniReadWriter.ReadIniKeys("Command", "worker", pathShare + "/CF.ini");
            }
            if (taskName.Equals(TASK_SYS_WAIT_ORDER))//待命
            {
                ras.Disconnect();
                taskName = IniReadWriter.ReadIniKeys("Command", "TaskName" + no, pathShare + "/Task.ini");
                if (taskName.Equals(TASK_SYS_WAIT_ORDER))
                {
                    IniReadWriter.WriteIniKeys("Command", "TaskChange" + no, "0", pathShare + "/Task.ini");
                }
            }
            else if (taskName.Equals(TASK_SYS_NET_TEST))//网络TEST
            {
                if (Net.isOnline())
                {
                    rasOperate("disconnest");
                }
                Thread.Sleep(500);
                rasOperate("connect");
                Thread.Sleep(500);
                if (!Net.isOnline())
                {
                    rasOperate("connect");
                    Thread.Sleep(1000);
                }
                if (Net.isOnline())
                {
                    rasOperate("disconnest");
                    IniReadWriter.WriteIniKeys("Command", "TaskName" + no, TASK_SYS_WAIT_ORDER, pathShare + "/Task.ini");
                    IniReadWriter.WriteIniKeys("Command", "TaskChange" + no, "0", pathShare + "/Task.ini");
                    IniReadWriter.WriteIniKeys("Command", "customPath" + no, "", pathShare + "/TaskPlus.ini");
                }
                else
                {
                    netError("error");
                }
            }
            else if (taskName.Equals(TASK_SYS_SHUTDOWN))//关机
            {
                IniReadWriter.WriteIniKeys("Command", "TaskChange" + no, "0", pathShare + "/Task.ini");
                IniReadWriter.WriteIniKeys("Command", "customPath" + no, "", pathShare + "/TaskPlus.ini");
                Process.Start("shutdown.exe", "-s -t 0");
                mainThreadClose();
            }
            else if (taskName.Equals(TASK_SYS_RESTART))//重启
            {
                IniReadWriter.WriteIniKeys("Command", "TaskChange" + no, "0", pathShare + "/Task.ini");
                IniReadWriter.WriteIniKeys("Command", "customPath" + no, "", pathShare + "/TaskPlus.ini");
                Process.Start("shutdown.exe", "-r -t 0");
                mainThreadClose();
            }
            else if (taskName.Equals(TASK_SYS_UPDATE))
            {
                IniReadWriter.WriteIniKeys("Command", "customPath" + no, "", pathShare + "/TaskPlus.ini");
                updateSoft();
                mainThreadClose();
            }
            else if (taskName.Equals(TASK_HANGUP_MM2))//MM2挂机
            {
                if (taskChange.Equals("1"))
                {
                    taskPath = IniReadWriter.ReadIniKeys("Command", "mm2", pathShare + "/CF.ini");
                }
                // start MM2 function
            }
            else if (taskName.Equals(TASK_HANGUP_YUKUAI))//愉快挂机
            {
                if (taskChange.Equals("1"))
                {
                    taskPath = IniReadWriter.ReadIniKeys("Command", "yukuai", pathShare + "/CF.ini");
                }
                // start YUKUAI function
            }
            else if (isVoteTask())//投票
            {
                netCheck();
                if (customPath.Equals(""))
                {
                    IniReadWriter.WriteIniKeys("Command", "TaskName" + no, TASK_SYS_WAIT_ORDER, pathShare + "/Task.ini");
                    IniReadWriter.WriteIniKeys("Command", "CacheMemory" + no, "", pathShare + "/TaskPlus.ini");
                    taskChangeProcess();
                    return;
                }
                if (taskChange.Equals("1"))
                {
                    if (customPath.Substring(customPath.LastIndexOf("\\") + 1) == "vote.exe")
                    {
                        String[] Lines = { @"start vote.exe" };
                        File.WriteAllLines(customPath.Substring(0, customPath.Length - 9) + @"\启动九天.bat", Lines, Encoding.GetEncoding("GBK"));
                        startProcess(customPath.Substring(0, customPath.Length - 9) + @"\启动九天.bat");
                        taskName = TASK_VOTE_JIUTIAN;
                    }
                    else
                    {
                        startProcess(customPath);
                        taskName = TASK_VOTE_PROJECT;
                        IntPtr hwnd0, hwnd1, hwnd2, hwnd3, hwnd4;
                        do
                            try
                            {
                                hwnd0 = HwndUtil.FindWindow("WTWindow", null);
                                hwnd1 = HwndUtil.FindWindow("TForm1", null);
                                hwnd2 = HwndUtil.FindWindow("ThunderRT6FormDC", null);
                                hwnd3 = HwndUtil.FindWindow("obj_Form", null);
                                hwnd4 = HwndUtil.FindWindow("TMainForm", null);
                                if (hwnd0 != IntPtr.Zero)
                                {
                                    String title = "";
                                    int i = HwndUtil.GetWindowText(hwnd0.ToInt32(), title, 512);
                                    if (title.Substring(0, 6) == "自动投票工具")
                                    {
                                        taskName = TASK_VOTE_MM;
                                    }
                                    else if (title.Substring(0, 8) == "VOTE2016")
                                    {
                                        taskName = TASK_VOTE_ML;
                                    }
                                }
                                else if (hwnd1 != IntPtr.Zero)
                                {
                                    taskName = TASK_VOTE_YUANQIU;
                                }
                                else if (hwnd2 != IntPtr.Zero)
                                {
                                    taskName = TASK_VOTE_JT;
                                }
                                else if (hwnd3 != IntPtr.Zero)
                                {
                                    taskName = TASK_VOTE_DM;
                                }
                                else if (hwnd4 != IntPtr.Zero)
                                {
                                    taskName = TASK_VOTE_JZ;
                                }
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                                mainThreadClose();
                            }
                            finally
                            {
                                Thread.Sleep(500);
                            }
                        while (!taskName.Equals(TASK_VOTE_PROJECT));
                    }
                    IniReadWriter.WriteIniKeys("Command", "TaskName" + no, taskName, pathShare + "/Task.ini");
                    Thread.Sleep(3000);
                }
                if (taskName.Equals(TASK_VOTE_JIUTIAN))
                {
                    if (!taskChange.Equals("1"))
                    {
                        startProcess(customPath.Substring(0, customPath.Length - 9) + @"\启动九天.bat");
                        Thread.Sleep(500);
                    }
                    jiutianStart();
                }
                else
                {
                    if (!taskChange.Equals("1"))
                    {
                        startProcess(customPath);
                        Thread.Sleep(500);
                    }
                    if (taskName.Equals(TASK_VOTE_MM))
                    {
                        //MM开始程序
                    }
                    else if (taskName.Equals(TASK_VOTE_ML))
                    {
                        //ML开始程序
                    }
                    else if (taskName.Equals(TASK_VOTE_YUANQIU))
                    {
                        //圆球开始程序
                    }
                    else if (taskName.Equals(TASK_VOTE_JT))
                    {
                        //JT开始程序
                    }
                    else if (taskName.Equals(TASK_VOTE_DM))
                    {
                        //DM开始程序
                    }
                    else if (taskName.Equals(TASK_VOTE_JZ))
                    {
                        //J开始程序
                    }
                }
                taskPath = customPath;
            }
            taskMonitor();
        }

        //NAME检测
        private bool nameCheck()
        {
            taskChange = IniReadWriter.ReadIniKeys("Command", "TaskChange" + no, pathShare + "/Task.ini");
            if (taskChange.Equals("1"))
            {
                taskName = IniReadWriter.ReadIniKeys("Command", "TaskName" + no, pathShare + "/Task.ini");
                if (!taskName.Equals(projectName))
                {
                    IniReadWriter.WriteIniKeys("Command", "Make" + no, "1", pathShare + "/Task.ini");
                    taskChangeProcess();
                    return false;
                }
            }
            return true;
        }

        //结束启动
        private void finishStart()
        {
            if (!nameCheck())
            {
                return;
            }
            IniReadWriter.WriteIniKeys("Command", "TaskChange" + no, "0", pathShare + "/Task.ini");
        }

        //九天启动
        private void jiutianStart()
        {
            IntPtr hwnd = IntPtr.Zero;
            projectName = TASK_VOTE_JIUTIAN;
            do
                try
                {
                    if (!nameCheck())
                    {
                        return;
                    }
                    hwnd = HwndUtil.FindWindow("WTWindow", null);
                    Thread.Sleep(500);
                }
                catch (Exception) { }
            while (hwnd == IntPtr.Zero);
            Thread.Sleep(2000);
            IntPtr hwndSysTabControl32 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
            //设置拨号延迟为0
            IntPtr hwndEx = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "拨号设置");
            hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "SysTabControl32", "");
            hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Edit", null);
            HwndUtil.SendMessage(hwndEx, 0x0C, IntPtr.Zero, "0");
            //设置工号
            if (inputId.Equals("1"))
            {
                String id = workerId;
                if (tail.Equals("1"))
                {
                    id = workerId + "-" + (no > 9 ? no.ToString() : "0" + no);
                }
                hwndEx = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "请输入工号");
                hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Edit", null);
                HwndUtil.SendMessage(hwndEx, 0x0C, IntPtr.Zero, id);
            }

            //开始投票
            hwndEx = HwndUtil.FindWindowEx(hwndSysTabControl32, IntPtr.Zero, "Button", "");
            hwndEx = HwndUtil.FindWindowEx(hwndEx, IntPtr.Zero, "Button", "开始投票");
            HwndUtil.clickHwnd(hwndEx);
            finishStart();
        }

        //通过路径启动进程
        private void startProcess(string pathName)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = pathName;
            info.Arguments = "";
            info.WorkingDirectory = pathName.Substring(0, pathName.LastIndexOf(@"\"));
            info.WindowStyle = ProcessWindowStyle.Minimized;
            Process pro = Process.Start(info);
            pro.WaitForExit();
        }

        //升级程序
        private void updateSoft()
        {
            string path = IniReadWriter.ReadIniKeys("Command", "Path0", pathShare + "/CF.ini");
            string line1 = "Taskkill /F /IM " + path.Substring(path.LastIndexOf("\\") + 1);
            string line2 = "ping -n 3 127.0.0.1>nul";
            string line3 = "copy / y " + path + @" """ + workingPath + @"""";
            string line4 = "ping -n 3 127.0.0.1>nul";
            string line5 = "start " + path.Substring(path.LastIndexOf("\\") + 1);
            string[] lines = { "@echo off", line1, line2, line3, line4, line5 };
            try
            {
                File.WriteAllLines(@"./自动升级.bat", lines, Encoding.GetEncoding("GBK"));
                IniReadWriter.WriteIniKeys("Command", "TaskChange" + no, "0", pathShare + "/Task.ini");
                startProcess(@"./自动升级.bat");
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

        //ADSL操作
        private void rasOperate(string type)
        {
            ras = new RASDisplay();
            if (type.Equals("connect"))
            {
                ras.Disconnect();
                Thread.Sleep(500);
                ras.Connect(adslName);
            }
            else
            {
                ras.Disconnect();
            }
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
                        netError("error");
                    }
                }
            }
            string arrDrop = IniReadWriter.ReadIniKeys("Command", "ArrDrop", pathShare + "/CF.ini");
            if (!StringUtil.isEmpty(arrDrop))
            {
                arrDrop = " " + arrDrop;
            }
            if (arrDrop.IndexOf(" " + no + " |") != -1)
            {
                IniReadWriter.WriteIniKeys("Command", "ArrDrop", arrDrop.Replace(" " + no + " |", ""), pathShare + "/CF.ini");
            }
        }

        //网络异常处理
        private void netError(string type)
        {
            String val = IniReadWriter.ReadIniKeys("Command", "Val", pathShare + "/CF.ini");
            if (StringUtil.isEmpty(val))
            {
                if (type.Equals("exception"))
                {
                    IniReadWriter.WriteIniKeys("Command", "Val", no.ToString(), pathShare + "/CF.ini");//正数 异常
                }
                else
                {
                    IniReadWriter.WriteIniKeys("Command", "Val", (-no).ToString(), pathShare + "/CF.ini");//负数 掉线
                }
                mainThreadClose();
            }
            else
            {
                Thread.Sleep(2000);
                netError(type);
            }

        }

        //到票待命
        private void switchWatiOrder()
        {
            IniReadWriter.WriteIniKeys("Command", "CustomPath" + no, "", pathShare + @"\TaskPlus.ini");
            IniReadWriter.WriteIniKeys("Command", "OVER", "1", pathShare + @"\CF.ini");
            IniReadWriter.WriteIniKeys("Command", "TaskName" + no, TASK_SYS_WAIT_ORDER, pathShare + @"\Task.ini");
            IniReadWriter.WriteIniKeys("Command", "TaskChange" + no, "1", pathShare + @"\Task.ini");
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
            return s > 15;
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
            int s = 0;
            bool isOnline = false;
            do
                try
                {
                    isOnline = Net.isOnline();
                    taskChange = IniReadWriter.ReadIniKeys("Command", "taskChange" + no, pathShare + "/Task.ini");
                    if (taskChange.Equals("1"))
                    {
                        taskChangeProcess();
                        return;
                    }
                    if (isSysTask())
                    {
                        p = 0;
                    }
                    if (taskName.Equals(TASK_VOTE_JIUTIAN) && p > 0)
                    {
                        if (jiutianOverCheck(ref s))
                        {
                            switchWatiOrder();
                        }
                    }
                    else if (taskName.Equals(TASK_VOTE_MM))
                    {
                        //MM到票检测
                    }
                    else if (taskName.Equals(TASK_VOTE_YUANQIU))
                    {
                        //圆球到票检测
                    }
                    else if (taskName.Equals(TASK_VOTE_JT))
                    {
                        //JT到票检测
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
                        //JZ到票检测
                    }
                    else if (taskName.Equals(TASK_VOTE_OUTDO))
                    {
                        //OUTDO到票检测
                    }
                }
                catch (Exception e)
                {
                }
                finally
                {
                    if (isOnline)
                    {
                        p = p < 0 ? 1 : ++p;
                    }
                    else
                    {
                        p = p > 0 ? -1 : --p;

                    }
                    label2.Text = p.ToString();
                    Thread.Sleep(2000);
                }
            while (p == 0 || (p > 0 && p < overTime) || (p < 0 && p > -overTime));
            if (taskName.Equals(TASK_VOTE_MM))
            {
                //MM2 check
                ras.Disconnect();//断开连接
                Thread.Sleep(500);
                //启动拨号定时timer
                ras.Connect(adslName);//重新拨号
                Thread.Sleep(500);
            }
            else
            {
                taskChangeProcess();
                return;
            }


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
                    taskName = TASK_SYS_WAIT_ORDER;
                    netCheck();
                    Thread.Sleep(1000);
                    rasOperate("disconnect");
                    notifyIcon1.ShowBalloonTip(0, now, "未发现项目缓存,待命中...\n请通过控制与监控端启动" + no + "号虚拟机", ToolTipIcon.Info);
                }
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
}
