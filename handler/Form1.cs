using controller.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace handler
{
    public partial class Form1 : Form
    {
        private string pathShare;
        private int no;
        private int delay;
        private int overTime;


        public Form1()
        {
            InitializeComponent();
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
                textBox1.Text =  no.ToString();
                overTime = int.Parse(IniReadWriter.ReadIniKeys("Command", "cishu", pathShare+"/CF.ini"));

            }
            else
            {
                MessageBox.Show("请设置handler.ini");
                Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            IniReadWriter.WriteIniKeys("Command", "bianhao", textBox1.Text, "./handler.ini");
        }


        public void _main()
        {

        }
    }
}
