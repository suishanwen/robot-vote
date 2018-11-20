using System.Threading;
using System.Windows.Forms;

namespace robot.core
{
    public class Monitor
    {
        private Thread monitorThread;

        public void Start()
        {
            monitorThread = new Thread(MonitorThread);
            monitorThread.Start();
        }

        public void Stop()
        {
            if (monitorThread.IsAlive)
            {
                monitorThread.Abort();
            }
        }

        public void MonitorThread()
        {
            Notification.Show("启动监控程序", ToolTipIcon.Info);
        }
    }
}