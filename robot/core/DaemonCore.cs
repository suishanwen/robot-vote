
namespace robot.core
{
    class DaemonCore
    {
        private static bool alive;

        public static bool Alive { get => alive; set => alive = value; }

        public static void Protect()
        {
            if (!Alive)
            {
                LogCore.Write("【守护线程】发现3分钟无活动，重置监控线程");
                ProgressCore.KillProcess(false);
                ConfigCore.SwitchNetTest();
                Form1.MainRestart();
            }
            Alive = false;
        }
    }
}
