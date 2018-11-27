using System;
using System.Threading;
using System.Windows.Forms;

namespace robot.core
{
    public class Monitor
    {
        private static Thread _monitorThread;

        public static void Start()
        {
            _monitorThread = new Thread(MonitorThread);
            _monitorThread.Start();
        }

        public static void Stop()
        {
            if (_monitorThread.IsAlive)
            {
                _monitorThread.Abort();
            }

            Notification.Show("结束监控程序", ToolTipIcon.Info);
        }

        public static void MonitorThread()
        {
            Notification.Show("启动监控程序", ToolTipIcon.Info);
//            NetCore.NetCheck();
//            List<VoteProject> voteProjects = ProjectAnalyze.GetVoteProjects();
            TaskCore.InitTask();
            try
            {
                TaskMonitor();
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }


        //主线程
        private static void TaskMonitor()
        {
        }
    }
}