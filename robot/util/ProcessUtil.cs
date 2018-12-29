using System.Diagnostics;
using robot.core;

namespace robot.util
{
    public class ProcessUtil
    {
        /// <summary>
        /// 根据“精确进程名”结束进程
        /// </summary>
        /// <param name="strProcName">精确进程名</param>
        public static void KillProc(string strProcName)
        {
            try
            {
                //精确进程名  用GetProcessesByName
                foreach (Process p in Process.GetProcessesByName(strProcName))
                {
                        p.Kill();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 根据 模糊进程名 结束进程
        /// </summary>
        /// <param name="strProcName">模糊进程名</param>
        public static void KillProcA(string strProcName)
        {
            try
            {
                //模糊进程名  枚举
                //Process[] ps = Process.GetProcesses();  //进程集合
                foreach (Process p in Process.GetProcesses())
                {
                    LogCore.Write($"结束进程:{p.ProcessName}  {p.Id}");
                    if (p.ProcessName.IndexOf(strProcName) > -1) //第一个字符匹配的话为0，这与VB不同
                    {
                            p.Kill();
                    }
                }
            }
            catch
            {
            }
        }


        /// <summary>
        /// 判断是否包含此字串的进程   模糊
        /// </summary>
        /// <param name="strProcName">进程字符串</param>
        /// <returns>是否包含</returns>
        public static bool SearchProcA(string strProcName)
        {
            try
            {
                //模糊进程名  枚举
                //Process[] ps = Process.GetProcesses();  //进程集合
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName.IndexOf(strProcName) > -1) //第一个字符匹配的话为0，这与VB不同
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 判断是否存在进程  精确
        /// </summary>
        /// <param name="strProcName">精确进程名</param>
        /// <returns>是否包含</returns>
        public static bool SearchProc(string strProcName)
        {
            try
            {
                //精确进程名  用GetProcessesByName
                Process[] ps = Process.GetProcessesByName(strProcName);
                if (ps.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}