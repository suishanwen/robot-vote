using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using robot.util;

namespace robot.core
{
    public class ProgressCore
    {
        //通过进程名获取进程
        public static Process[] GetProcess(string proName)
        {
            return Process.GetProcessesByName(proName);
        }

        //获取项目进程
        private static Process[] ProcessCheck()
        {
            Process[] pros = GetProcess("AutoUpdate.dll");
            if (pros.Length > 0)
            {
                foreach (Process p in pros)
                {
                    p.Kill();
                }
            }

            string process1 = "vote.exe";
            string process2 = "register.exe";
            Process[] process = GetProcess(process1);
            if (process.Length > 0)
            {
                return process;
            }

            process = GetProcess(process2);
            if (process.Length > 0)
            {
                return process;
            }

            if (process.Length > 0)
            {
                return process;
            }

            return GetProcess("");
        }

        //关闭进程
        public static void KillProcess(bool stopIndicator)
        {
            TaskCore taskCore = MonitorCore.GetTaskCore();
            string taskName = taskCore.TaskName;
            //传票结束
            if (stopIndicator && taskCore.IsVoteTask())
            {
                LogCore.Write($"{taskCore.ProjectName}传票结束!");
                if (taskName.Equals(TaskCore.TASK_VOTE_JIUTIAN))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                        hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "");
                        hwnd = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "结束投票");
                        HwndUtil.clickHwnd(hwnd);
                        int s = 0;
                        IntPtr hwndEx = IntPtr.Zero;
                        do
                        {
                            Thread.Sleep(500);
                            hwnd = HwndUtil.FindWindow("WTWindow", null);
                            hwndEx = HwndUtil.FindWindow("#32770", "信息：");
                            if (s % 10 == 0&& hwnd != IntPtr.Zero)
                            {
                                IntPtr hwnd0 = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "SysTabControl32", "");
                                hwnd0 = HwndUtil.FindWindowEx(hwnd0, IntPtr.Zero, "Button", "");
                                hwnd0 = HwndUtil.FindWindowEx(hwnd0, IntPtr.Zero, "Button", "结束投票");
                                HwndUtil.clickHwnd(hwnd0);
                            }
                            if (hwndEx != IntPtr.Zero)
                            {
                                HwndUtil.closeHwnd(hwndEx);
                                s = 90;
                            }
                            s++;
                        } while (hwnd != IntPtr.Zero && s < 90);
                    }
                }else if (taskName.Equals(TaskCore.TASK_VOTE_YUANQIU))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("TForm1", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TButton", "停止");
                        while (!Net.IsOnline())
                        {
                            Thread.Sleep(500);
                        }
                        HwndThread.createHwndThread(hwndEx);
                        Thread.Sleep(5000);
                    }
                    
                }
                else if (taskName.Equals(TaskCore.TASK_VOTE_JZ))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("TMainForm", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr hwndTGroupBox = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "TGroupBox", "当前状态");
                        IntPtr hwndEx = HwndUtil.FindWindowEx(hwndTGroupBox, IntPtr.Zero, "TButton", "停 止");
                        while (!Net.IsOnline())
                        {
                            Thread.Sleep(500);
                        }
                        HwndThread.createHwndThread(hwndEx);
                        Thread.Sleep(5000);
                    }
                }
                else if (taskName.Equals(TaskCore.TASK_VOTE_JT))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("ThunderRT6FormDC", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "停止投票");
                        while (!Net.IsOnline())
                        {
                            Thread.Sleep(500);
                        }
                        HwndThread.createHwndThread(hwndEx);
                        Thread.Sleep(5000);
                    }
                }
                else if (taskName.Equals(TaskCore.TASK_VOTE_HY))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, "Button", "停止");
                        while (!Net.IsOnline())
                        {
                            Thread.Sleep(500);
                        }
                        HwndThread.createHwndThread(hwndEx);
                        Thread.Sleep(5000);
                    }
                }
                else if (taskName.Equals(TaskCore.TASK_VOTE_MM))
                {
                    IntPtr hwnd = HwndUtil.FindWindow("WTWindow", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr hwndEx = HwndUtil.FindWindowEx(hwnd, IntPtr.Zero, null, "停止投票");
                        while (!Net.IsOnline())
                        {
                            Thread.Sleep(500);
                        }
                        HwndThread.createHwndThread(hwndEx);
                        int s = 0;
                        IntPtr hwndTip;
                        do
                        {
                            s++;
                            hwndTip = HwndUtil.FindWindow(null, "投票软件提示");
                            Thread.Sleep(500);
                        } while (hwndTip == IntPtr.Zero && s < 60);
                    }
                }
            }
            Process[] process = ProcessCheck();
            if (process.Length > 0)
            {
                if (taskCore.IsVoteTask())
                {
                    int counter = 1;
                    while (!Net.IsOnline() && counter < 60)
                    {
                        counter++;
                        Thread.Sleep(500);
                    }
                }

                foreach (Process p in process)
                {
                    LogCore.Write("结束进程  :" + p);
                    try
                    {
                        p.Kill();
                    }
                    catch (Exception e)
                    {
                        LogCore.Write("结束进程失败  :" + e);
                    }
                }
            }
        }


        public static string InvokeCmd(string cmdArgs)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(cmdArgs);
            p.StandardInput.WriteLine("exit");
            string tstr = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return tstr;
        }

        //复制投票项目到虚拟机并更改启动路径
        private static void CopyAndChangePath(ref string pathName)
        {
            if (pathName.IndexOf("投票项目") != -1)
            {
                string exeName = pathName.Substring(pathName.LastIndexOf("\\") + 1);
                string sourcePath = pathName.Substring(0, pathName.LastIndexOf("\\"));
                string targetPath = PathCore.WorkingPath + "\\投票项目\\" +
                                    sourcePath.Substring(sourcePath.IndexOf("投票项目") + 5);
                if (false == Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                DirectoryInfo dir = new DirectoryInfo(sourcePath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos(); //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is FileInfo) //判断是文件
                    {
                        try
                        {
                            File.Copy(i.FullName, targetPath + "\\" + i.Name, false); //复制文件，true表示可以覆盖同名文件
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            i.Refresh();
                        }
                    }
                }

                //更新九天自动更新文件
                if (exeName == "启动九天.bat")
                {
                    DirectoryInfo d = new DirectoryInfo(targetPath);
                    FileSystemInfo[] fsi = d.GetFileSystemInfos(); //获取目录下（不包含子目录）的文件和子目录
                    foreach (FileSystemInfo i in fsi)
                    {
                        if (i.Name.IndexOf(".exe") != -1 && i.Name != "vote.exe")
                        {
                            FileInfo fi = new FileInfo(i.FullName);
                            try
                            {
                                File.Delete(targetPath + "\\vote.exe");
                                fi.MoveTo(targetPath + "\\vote.exe");
                                i.Delete();
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                i.Refresh();
                                fi.Refresh();
                            }
                        }
                    }
                }

                pathName = targetPath + "\\" + exeName;
            }
        }

        private static void ThreadProcess(object obj)
        {
            string pathName = obj.ToString();
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = pathName;
                info.Arguments = "";
                info.WorkingDirectory = pathName.Substring(0, pathName.LastIndexOf("\\"));
                info.WindowStyle = ProcessWindowStyle.Normal;
                Process pro = Process.Start(info);
                pro.WaitForExit();
                pro.Close();
                Thread.Sleep(500);
            }
            catch (Exception e)
            {
                MessageBox.Show("拒绝访问");
                MonitorCore.GetTaskCore().WaitOrder();
                //Process.Start("shutdown.exe", "-r -t 0");
            }
        }

        //通过路径启动进程
        public static void StartProcess(string pathName)
        {
            CopyAndChangePath(ref pathName);
            Thread t = new Thread(new ParameterizedThreadStart(ThreadProcess));
            t.Start(pathName);
        }
    }
}