using robot.util;

namespace robot.core
{
    public class Statistics
    {
        public static void Add(string name, double price, int succ)
        {
            var cf = $"{name}|{price}";
            var val = ConfigCore.GetStatistic(cf);
            var oldVal = 0;
            if (!StringUtil.isEmpty(val))
            {
                oldVal = int.Parse(val);
            }
            ConfigCore.WriteStatistic(cf, $"{oldVal + succ}");
        }
    }
}