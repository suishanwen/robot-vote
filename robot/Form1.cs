using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using robot.core;
using robot.util;
using Monitor = robot.core.Monitor;

namespace robot
{
    public partial class form1 : Form
    {
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
                }

                return;
            }

            base.WndProc(ref m);
        }


        //启动程序，检查配置文件，启动主线程
        private void Form1_Load(object sender, EventArgs e)
        {
            Notification.Init(notifyIcon1);
            if (!File.Exists(@".\cf.ini"))
            {
                IniReadWriter.WriteIniKeys("Base", "sort", textBox1.Text, "./cf.ini");
                IniReadWriter.WriteIniKeys("Base", "workerId", textBox2.Text, "./cf.ini");
                IniReadWriter.WriteIniKeys("Base", "delay", textBox4.Text, "./cf.ini");
            }

            ConfigCore.InitConfig();
            textBox1.Text = ConfigCore.Sort;
            textBox2.Text = ConfigCore.WorkerId;
            textBox4.Text = ConfigCore.Delay;
            button1_Click(null, null);
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

        //改变延时
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            IniReadWriter.WriteIniKeys("Base", "delay", textBox4.Text, "./cf.ini");
        }

        //启动监控线程
        public void button1_Click(object sender, EventArgs e)
        {
            notifyIcon1.Icon = (Icon) Properties.Resources.ResourceManager.GetObject("running");
            button1.Enabled = false;
            button2.Enabled = true;
            LogCore.Clear(); //清空日志
//            this.WindowState = FormWindowState.Minimized;
            Monitor.Start();
        }

        //终止监控线程
        private void MainThreadClose()
        {
            notifyIcon1.Icon = (Icon) Properties.Resources.ResourceManager.GetObject("stop");
            button2.Enabled = false;
            button1.Enabled = true;
            Monitor.Stop();
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
        private void RefreshIcon()
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