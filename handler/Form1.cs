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
        

        public form1()
        {
            InitializeComponent(); 
        }

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


        public void _main()
        {
            while (true)
            {
                IntPtr hwnd = HwndUtil.FindWindow(null, "PuTTY Configuration");
                Console.WriteLine(hwnd);
                Thread.Sleep(2000);
            }
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
    }
}
