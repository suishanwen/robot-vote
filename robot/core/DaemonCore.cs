
using robot.util;

namespace robot.core
{
    class DaemonCore
    {
        private static bool alive;
        private static int count;


        public static void KeepAlive()
        {
            alive = true;
            count = 0;
        }

        public static void Protect()
        {
            if (!alive && count < 3)
            {
                count += 1;
                LogCore.Write("【守护线程】超过2分钟无活动，重置监控线程");
                ComCore.ReMake();
                ProgressCore.KillProcess(false);
                ConfigCore.SwitchNetTest();
                TaskInfos.Clear();
                Form1.MainRestart();
            }
            alive = false;
        }
    }
}
