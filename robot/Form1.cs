using System;
using System.Drawing;
using System.Windows.Forms;
using robot.core;
using robot.util;
using Monitor = robot.core.Monitor;

namespace robot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Hotkey.RegisterHotKey(Handle, 10, Hotkey.MODKEY.None, Keys.F10);
            Hotkey.RegisterHotKey(Handle, 11, Hotkey.MODKEY.None, Keys.F9);
        }

        public string Sort
        {
            set { textBox1.Text = value; }
        }

        public string Delay
        {
            set { textBox2.Text = value; }
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
                }

                return;
            }

            base.WndProc(ref m);
        }


        //启动程序，检查配置文件，启动主线程
        private void Form1_Load(object sender, EventArgs e)
        {
            Notification.Init(notifyIcon1);
            ConfigCore.InitConfig(this);
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
        public void button1_Click(object sender, EventArgs e)
        {
            notifyIcon1.Icon = (Icon) Properties.Resources.ResourceManager.GetObject("running");
            button1.Enabled = false;
            button2.Enabled = true;
            LogCore.Clear(); //清空日志
            WindowState = FormWindowState.Minimized;
            Monitor.Start();
        }

        //终止监控线程
        public void MainThreadClose()
        {
            notifyIcon1.Icon = (Icon) Properties.Resources.ResourceManager.GetObject("stop");
            ConfigCore.Cache();
            button2.Enabled = false;
            button1.Enabled = true;
            Monitor.Stop();
            Environment.Exit(0);
        }

        //点击停止
        private void button2_Click(object sender, EventArgs e)
        {
            MainThreadClose();
        }

        //关闭程序
        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainThreadClose();
        }

        //双击托盘
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        //清理托盘
        public void RefreshIcon()
        {
            Rectangle rect = new Rectangle();
            rect = Screen.GetWorkingArea(this);
            int startX = rect.Width - 200; //屏幕高
            int startY = rect.Height + 20; //屏幕高
            for (; startX < rect.Width; startX += 3)
            {
                MouseKeyboard.SetCursorPos(startX, startY);
            }
        }
    }
}