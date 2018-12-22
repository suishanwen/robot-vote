using System;
using System.Drawing;
using System.Windows.Forms;
using robot.core;
using robot.util;
using MonitorCore = robot.core.MonitorCore;

namespace robot
{
    public partial class Form1 : Form
    {
        public static Form1 This;

        //初始化
        public Form1()
        {
            InitializeComponent();
            Hotkey.RegisterHotKey(Handle, 10, Hotkey.MODKEY.None, Keys.F10);
            Hotkey.RegisterHotKey(Handle, 11, Hotkey.MODKEY.None, Keys.F9);
            This = this;
            LogCore.Clear(); //清空日志
        }

        //委托 解决线程间操作问题
        delegate void SetFormDataDelegate(int sort, int delay, string share);

        //设置编号、延时、共享
        public static void SetFormData(int sort, int delay, string share)
        {
            if (This.InvokeRequired)
            {
                SetFormDataDelegate d = new SetFormDataDelegate(SetFormData);
                This.Invoke(d, new object[] {sort, delay, share});
            }
            else
            {
                This.textBox1.Text = sort.ToString();
                This.textBox2.Text = delay.ToString();
                This.textBox3.Text = share;
            }
        }

        //委托 解决线程间操作问题
        delegate void ShowTipDelegate(string content, ToolTipIcon toolTipIcon);

        //显示通知
        public static void ShowTip(string content, ToolTipIcon toolTipIcon)
        {
            if (This.InvokeRequired)
            {
                ShowTipDelegate d = new ShowTipDelegate(ShowTip);
                This.Invoke(d, new object[] {content, toolTipIcon});
            }
            else
            {
                This.notifyIcon1.ShowBalloonTip(0, content, DateTime.Now.ToLocalTime().ToString(), toolTipIcon);
            }
        }

        //绑定快捷键
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
                }

                return;
            }

            base.WndProc(ref m);
        }


        //启动程序，检查配置文件，启动主线程
        private void Form1_Load(object sender, EventArgs e)
        {
            if (ConfigCore.InitConfig())
            {
                MainStart();
            }
        }

        //改变编号
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            IniReadWriter.WriteIniKeys("Command", "bianhao", textBox1.Text, ConfigCore.BaseConfig);
        }

        //改变延时
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            IniReadWriter.WriteIniKeys("Command", "yanchi", textBox2.Text, ConfigCore.BaseConfig);
        }

        //改变延时
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            IniReadWriter.WriteIniKeys("Command", "gongxiang", textBox3.Text, ConfigCore.BaseConfig);
            ConfigCore.InitPathShare();
            IniReadWriter.WriteIniKeys("Command", "gongxiang", textBox3.Text, ConfigCore.PathShareConfig);
        }

        //选择共享button
        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = fd.SelectedPath;
            }
        }

        //启动监控线程
        private void MainStart()
        {
            notifyIcon1.Icon = (Icon) Properties.Resources.ResourceManager.GetObject("running");
            button1.Enabled = false;
            button2.Enabled = true;
            timer1.Enabled = true;
            timer2.Enabled = true;
            WindowState = FormWindowState.Minimized;
            MonitorCore.Start();
        }


        //委托 解决线程间操作问题
        delegate void MainCloseDelegate();

        //终止监控线程
        public static void MainClose()
        {
            if (This.InvokeRequired)
            {
                MainCloseDelegate d = new MainCloseDelegate(MainClose);
                This.Invoke(d, new object[] { });
            }
            else
            {
                This.notifyIcon1.Icon = (Icon) Properties.Resources.ResourceManager.GetObject("stop");
                ConfigCore.Cache();
                This.timer1.Enabled = false;
                This.timer2.Enabled = false;
                This.button2.Enabled = false;
                This.button1.Enabled = true;
                MonitorCore.Stop();
            }
        }

        //委托 解决线程间操作问题
        delegate void MainRestartDelegate();

        //终止监控线程
        public static void MainRestart()
        {
            if (This.InvokeRequired)
            {
                MainRestartDelegate d = new MainRestartDelegate(MainRestart);
                This.Invoke(d, new object[] { });
            }
            else
            {
                Form1.MainClose();
                This.MainStart();
            }
        }

        //点击启动
        public void button1_Click(object sender, EventArgs e)
        {
            if (ConfigCore.InitConfig())
            {
                MainStart();
            }
        }

        //点击停止
        private void button2_Click(object sender, EventArgs e)
        {
            MainClose();
        }

        //关闭程序
        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainClose();
            Environment.Exit(0);
        }

        //双击托盘
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }


        //timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            AutoVote.CheckSucc();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            DaemonCore.Protect();
        }
    }
}