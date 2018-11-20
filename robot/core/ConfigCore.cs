using robot.util;

namespace robot.core
{
    public class ConfigCore
    {
        public static string Sort;
        public static string WorkerId;
        public static string AdslName;
        public static string Delay;
        public static string Id;

        public static void InitConfig()
        {
            Sort = IniReadWriter.ReadIniKeys("Base", "sort", "./cf.ini");
            WorkerId = IniReadWriter.ReadIniKeys("Base", "workerId", "./cf.ini");
            AdslName = IniReadWriter.ReadIniKeys("Base", "adslName", "./cf.ini");
            Delay = IniReadWriter.ReadIniKeys("Base", "delay", "./cf.ini");
            Id = WorkerId + "-" + Sort;
        }
    }
}