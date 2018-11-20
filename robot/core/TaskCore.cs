namespace handler.core
{
    public class TaskCore
    {
        private const string TASK_SYS_UPDATE = "Update";
        private const string TASK_SYS_WAIT_ORDER = "待命";
        private const string TASK_SYS_SHUTDOWN = "关机";
        private const string TASK_SYS_RESTART = "重启";
        private const string TASK_SYS_NET_TEST = "网络测试";
        private const string TASK_VOTE_JIUTIAN = "九天";
        private const string TASK_VOTE_YUANQIU = "圆球";
        private const string TASK_VOTE_MM = "MM";
        private const string TASK_VOTE_JT = "JT";
        private const string TASK_VOTE_ML = "ML";
        private const string TASK_VOTE_DM = "DM";
        private const string TASK_VOTE_JZ = "JZ";
        private const string TASK_VOTE_OUTDO = "Outdo";

        public static string TaskName;

        //判断当前是否为系统任务
        public static bool IsSysTask()
        {
            return TaskName.Equals(TASK_SYS_UPDATE) || TaskName.Equals(TASK_SYS_WAIT_ORDER) ||
                   TaskName.Equals(TASK_SYS_SHUTDOWN) || TaskName.Equals(TASK_SYS_RESTART) ||
                   TaskName.Equals(TASK_SYS_NET_TEST);
        }

        //判断当前是否为投票项目
        public static bool IsVoteTask()
        {
            return TaskName.Equals(TASK_VOTE_JIUTIAN) || TaskName.Equals(TASK_VOTE_YUANQIU) ||
                   TaskName.Equals(TASK_VOTE_MM) || TaskName.Equals(TASK_VOTE_ML) || TaskName.Equals(TASK_VOTE_JZ) ||
                   TaskName.Equals(TASK_VOTE_JT) || TaskName.Equals(TASK_VOTE_DM) || TaskName.Equals(TASK_VOTE_OUTDO);
        }

        public static void StopAndUpload()
        {
            if (TaskName.Equals(TASK_VOTE_JIUTIAN))
            {
            }
            else if (TaskName.Equals(TASK_VOTE_YUANQIU))
            {
            }
            else if (TaskName.Equals(TASK_VOTE_JZ))
            {
            }
            else if (TaskName.Equals(TASK_VOTE_JT))
            {
            }
            else if (TaskName.Equals(TASK_VOTE_MM))
            {
            }
        }
    }
}