using System;
using System.Collections.Generic;
using System.Text;

namespace robot.core
{
    class DaemonCore
    {
        private static bool alive;

        public static bool Alive { get => alive; set => alive = value; }

        public static void protect()
        {
            if (!alive)
            {
                LogCore.Write("【守护线程】发现3分钟无活动，重置监控线程");
                ProgressCore.KillProcess(false);
                MonitorCore.GetTaskCore().SwitchWaitOrder();
                Form1.MainRestart();
            }
            alive = false;
        }
    }
}
