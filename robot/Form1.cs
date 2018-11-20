using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using robot.core;
using robot.util;

namespace robot
{
    public partial class form1 : Form
    {
        private Thread main; //主线程

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
            if (!File.Exists(@".\handler.ini"))
            {
                IniReadWriter.WriteIniKeys("Base", "sort", textBox1.Text, "./cf.ini");
                IniReadWriter.WriteIniKeys("Base", "workerId", textBox2.Text, "./cf.ini");
                IniReadWriter.WriteIniKeys("Base", "adslName", textBox3.Text, "./cf.ini");
                IniReadWriter.WriteIniKeys("Base", "delay", textBox4.Text, "./cf.ini");
            }

            ConfigCore.InitConfig();
            textBox1.Text = ConfigCore.Sort;
            textBox2.Text = ConfigCore.WorkerId;
            textBox3.Text = ConfigCore.AdslName;
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
            notifyIcon1.Icon = (Icon) Properties.Resources.ResourceManager.GetObject("running");
            button1.Enabled = false;
            button2.Enabled = true;
            LogCore.Clear(); //清空日志
            this.WindowState = FormWindowState.Minimized;
            main = new Thread(_main);
            main.Start();
        }


        //终止主线程
        private void MainThreadClose()
        {
            notifyIcon1.Icon = (Icon) Properties.Resources.ResourceManager.GetObject("stop");
            button2.Enabled = false;
            button1.Enabled = true;
            main.Abort();
        }

        //点击停止
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
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


        //显示通知
        private void ShowNotification(string content, ToolTipIcon toolTipIcon)
        {
            notifyIcon1.ShowBalloonTip(0, content, DateTime.Now.ToLocalTime().ToString(), toolTipIcon);
        }


        //清理托盘
        private void refreshIcon()
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

        //主线程
        public void _main()
        {
            ShowNotification("启动程序", ToolTipIcon.Info);
            //进程初始化部分
            string now = DateTime.Now.ToLocalTime().ToString();
            try
            {
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            MainThreadClose();
        }
    }
}