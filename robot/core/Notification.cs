using System.Threading;
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

        //重启资源管理器
        private static void RestartExplorer()
        {
            ProgressCore.InvokeCmd("Taskkill /f /im explorer.exe & start explorer.exe & ping 127.1 -n 1 >nul");
        }

        //创建线程重启资源管理器
        public static void Refresh()
        {
            new Thread(RestartExplorer).Start();
        }
    }
}