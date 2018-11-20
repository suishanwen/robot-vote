using System;
using System.Windows.Forms;

namespace robot.core
{
    public class Notification
    {
        private static NotifyIcon notifyIcon;

        public static void Init(NotifyIcon notifyIcon1)
        {
            notifyIcon = notifyIcon1;
        }

        //显示通知
        public static void Show(string content, ToolTipIcon toolTipIcon)
        {
            notifyIcon.ShowBalloonTip(0, content, DateTime.Now.ToLocalTime().ToString(), toolTipIcon);
        }
        
    }
}