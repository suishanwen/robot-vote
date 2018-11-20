using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using robot.util;

namespace robot.core
{
    public class ProjectAnalyze
    {
        public static VoteProject activeVoteProject;
        public static List<VoteProject> voteProjectMonitorList = new List<VoteProject>();
        public static string voteProjectNameDroped;
        public static string voteProjectNameGreen;
        public static int downLoadCount;
        public static List<VoteProject> GetVoteProjects()
        {
            HttpManager httpUtil = HttpManager.getInstance();
            string result = "";
            do
            {
                try
                {
                    result = httpUtil.requestHttpGet(
                        "http://butingzhuan.com/tasks.php?t=" + DateTime.Now.Millisecond.ToString(), "", "");
                }
                catch (Exception e)
                {
                    LogCore.Write("Request Fail!Retry in 10s...");
                    Thread.Sleep(10000);
                }
            } while (result == "");

            result = result.Substring(result.IndexOf("时间</td>"));
            result = result.Substring(0, result.IndexOf("qzd_yj"));
            result = result.Substring(result.IndexOf("<tr class='blank'>"));
            result = result.Substring(0, result.LastIndexOf("<tr class='blank'>"));
            if (DateTime.Now.Minute % 30 == 0)
            {
                LogCore.Write("AutoVote: Keep Alive! Finished Request!     " + DateTime.Now.ToString());
            }

            Regex regTR = new Regex(@"(?is)<tr[^>]*>(?:(?!</tr>).)*</tr>");
            Regex regTD = new Regex(@"(?is)<t[dh][^>]*>((?:(?!</td>).)*)</t[dh]>");
            MatchCollection mcTR = regTR.Matches(result);
            List<VoteProject> voteProjectList = new List<VoteProject>();
            foreach (Match mTR in mcTR)
            {
                if (mTR.Value.IndexOf("不换") == -1)
                {
                    MatchCollection mcTD = regTD.Matches(mTR.Value);
                    int index = 0;
                    VoteProject voteProject = new VoteProject();
                    foreach (Match mTD in mcTD)
                    {
                        string innerTd = mTD.Groups[1].Value;
                        //Log.writeLogs("./log.txt", "***"+index);
                        //Log.writeLogs("./log.txt", mTD.Value);
                        switch (index)
                        {
                            case 2:
                                voteProject.ProjectName = HtmlMatch.GetContent(innerTd, "a");
                                break;
                            case 5:
                                voteProject.Price = double.Parse(innerTd);
                                break;
                            case 7:
                                String[] quantityInfo = mTD.Value.Split('"');
                                quantityInfo = quantityInfo[1].Split('/');
                                try
                                {
                                    voteProject.Remains = long.Parse(innerTd.Trim());
                                    if (!StringUtil.isEmpty(quantityInfo[0].Trim()))
                                    {
                                        voteProject.FinishQuantity = long.Parse(quantityInfo[0]);
                                    }

                                    voteProject.TotalRequire =
                                        long.Parse(quantityInfo[1].Substring(0, quantityInfo[1].IndexOf(" ")));
                                }
                                catch (Exception e)
                                {
                                }

                                break;
                            case 8:
                                voteProject.BackgroundAddress = HtmlMatch.GetAttr(innerTd, "a", "href");
                                break;
                            case 9:
                                voteProject.DownloadAddress = HtmlMatch.GetAttr(innerTd, "a", "href");
                                break;
                            case 10:
                                try
                                {
                                    voteProject.IdType = HtmlMatch.GetAttr(innerTd, "input", "value").Substring(0, 2);
                                }
                                catch (Exception e)
                                {
                                    if (innerTd.IndexOf("BT-") != -1)
                                    {
                                        voteProject.IdType = "BT";
                                    }
                                    else if (innerTd.IndexOf("AQ-") != -1)
                                    {
                                        voteProject.IdType = "AQ";
                                    }
                                    else if (innerTd.IndexOf("Q7-") != -1)
                                    {
                                        voteProject.IdType = "Q7";
                                    }
                                }

                                break;
                            case 12:
                                voteProject.BackgroundNo = innerTd;
                                break;
                            case 13:
                                voteProject.RefreshDate = Convert.ToDateTime("2018-" + innerTd + ":00");
                                break;
                        }

                        index++;
                    }

                    voteProject.IsRestrict = voteProject.BackgroundNo.IndexOf("限制") != -1;
                    voteProjectList.Add(voteProject);
                }
            }

            return voteProjectList;
        }


        public static bool isDropedProject(string project, int checkType)
        {
            voteProjectNameDroped = IniReadWriter.ReadIniKeys("Command", "voteProjectNameDroped", "c:/AutoVote.ini");
            if (checkType == 1)
            {
                string[] dropedProjectList = voteProjectNameDroped.Split('|');
                foreach (string dropedProject in dropedProjectList)
                {
                    if (project.IndexOf(dropedProject) != -1)
                    {
                        return true;
                    }
                }

                return false;
            }

            return voteProjectNameDroped.IndexOf(project) != -1;
        }

        public static void testVoteProjectMonitorList()
        {
            for (int i = 0; i < voteProjectMonitorList.Count; i++)
            {
                VoteProject voteProject = voteProjectMonitorList[i];
                if (voteProject.Remains > 0 && (voteProject.Remains * voteProject.Price) > 100 &&
                    !voteProject.IsRestrict)
                {
                    Console.WriteLine("projectName：" + voteProject.ProjectName + ",price：" + voteProject.Price +
                                      ",remains：" + voteProject.Remains);
                    HttpManager httpManager = HttpManager.getInstance();
                    string pathName = "c:/downloads/" +
                                      voteProject.DownloadAddress.Substring(
                                          voteProject.DownloadAddress.LastIndexOf("/") + 1);
                    string url = voteProject.DownloadAddress;
                    string now = DateTime.Now.ToLocalTime().ToString();
                    LogCore.Write("开始下载:" + url);
                    downLoadCount = 0;
                    bool isDownloading = true;
                    do
                    {
                        try
                        {
                            httpManager.HttpDownloadFile(url, pathName);
                            isDownloading = false;
                        }
                        catch (Exception)
                        {
                            LogCore.Write(voteProject.ProjectName + "  下载异常，重新下载");
                            File.Delete(pathName);
                            Thread.Sleep(1000);
                        }
                    } while (isDownloading);

                    LogCore.Write(pathName + "  下载完成");
                    Winrar.UnCompressRar("c:/projects/" + voteProject.ProjectName,
                        pathName,
                        voteProject.DownloadAddress.Substring(voteProject.DownloadAddress.LastIndexOf("/") + 1));
                    if (!File.Exists("c:/projects/" + voteProject.ProjectName + "/启动九天.bat"))
                    {
                        String[] Lines = {@"start vote.exe"};
                        File.WriteAllLines("c:/projects/" + voteProject.ProjectName + "/启动九天.bat",
                            Lines, Encoding.GetEncoding("GBK"));
                    }

                    try
                    {
                        File.Delete(pathName);
                    }
                    catch (IOException)
                    {
                        LogCore.Write(pathName + "-->文件占用中，无法删除!");
                    }

                    activeVoteProject = voteProject;
                    LogCore.Write("AutoVote: " + voteProject.ProjectName + " " + voteProject.BackgroundNo + "    " +
                                  DateTime.Now.ToLocalTime().ToString());
                    break;
                }
            }
        }
    }
}