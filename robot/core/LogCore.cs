using System.IO;

namespace handler.core
{
    public class LogCore
    {
        //写txt
        public static void Write(string content)
        {
            StreamWriter sw = File.AppendText(@"./log.txt");
            sw.WriteLine(content);
            sw.Close();
        }

        public static void Clear()
        {
            StreamWriter sw = new StreamWriter(@"./log.txt");
            sw.Write("");
            sw.Close();
        }
    }
}