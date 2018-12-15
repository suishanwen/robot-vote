using System;
using System.Threading;
using System.Windows.Forms;

namespace robot.core
{
    public class MonitorCore
    {
        private static Thread _monitorThread;
        private static TaskCore _taskCore;
        public static void Start()
        {
            _monitorThread = new Thread(MonitorThread);
            _monitorThread.Start();
        }

        public static TaskCore GetTaskCore()
        {
            if(_taskCore == null)
            {
                _taskCore = new TaskCore();
            }
            return _taskCore;
        }

        public static void Stop()
        {
            if (_monitorThread.IsAlive)
            {
                _monitorThread.Abort();
            }
            ConfigCore.Cache();
            Notification.Show("结束监控程序", ToolTipIcon.Info);
        }

        public static void MonitorThread()
        {
            Notification.Show("启动监控程序", ToolTipIcon.Info);
            try
            {
                TaskCore taskCore = GetTaskCore();
                taskCore.InitTask();
                while (true)
                {
                    taskCore.TaskMonitor();
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}