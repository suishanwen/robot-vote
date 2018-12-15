using System.Windows.Forms;

namespace robot.core
{
    public class Notification
    {

        //显示通知
        public static void Show(string content, ToolTipIcon toolTipIcon)
        {
            Form1.ShowTip(content, toolTipIcon);
        }
        

        //清理托盘
        public static void Refresh()
        {
            Form1.RefreshIcon();
        }
    }
}