#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SrsManageCommon
{
    /// <summary>
    /// client type
    /// </summary>
    [Serializable]
    public enum ClientType
    {
        Monitor,
        User,
    }

    /// <summary>
    /// camera type
    /// </summary>
    [Serializable]
    public enum MonitorType
    {
        Onvif,
        GBT28181,
        Webcast,
        Unknow,
    }

    public static class Common
    {
        public static string WorkPath = Environment.CurrentDirectory + "/";
        public static SystemConfig SystemConfig = null!;
        public static Object LockDbObjForOnlineClient = new object();
        public static Object LockDbObjForDvrVideo = new object();
        public static Object LockDbObjForStreamDvrPlan = new object();
        public static Object LockDbObjForHeartbeat = new object();
        public static Object LockDbObjForLivePlan = new object();
        public static readonly string LogPath = WorkPath + "logs/";
        /// <summary>
        /// The executable file address of ffmpeg
        /// </summary>
        public static string FFmpegBinPath = "./ffmpeg";

        public static byte? FFmpegThreadCount = 2;


        static Common()
        {
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
            if (!Directory.Exists(WorkPath + "CutMergeFile"))
            {
                Directory.CreateDirectory(WorkPath + "CutMergeFile");
            }
            if (!Directory.Exists(WorkPath + "CutMergeDir"))
            {
                Directory.CreateDirectory(WorkPath + "CutMergeDir");
            }
        }

        /// <summary>
        /// Is it Url
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUrl(string str)
        {
            try
            {
                var Url = @"^http(s)?://([\w-]+\.)+[\w-]+(:\d*)?(/[\w- ./?%&=]*)?$";
                return Regex.IsMatch(str, Url);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Delete null records in List<T>
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveNull<T>(List<T> list)
        {
            // Find the first empty element O(n)
            var count = list.Count;
            for (var i = 0; i < count; i++)
                if (list[i] == null)
                {
                    // record current location
                    var newCount = i++;

                    // For each non-empty element, copy to the current position O(n)
                    for (; i < count; i++)
                        if (list[i] != null)
                            list[newCount++] = list[i];

                    //remove redundant elements  O(n)
                    list.RemoveRange(newCount, count - newCount);
                    break;
                }
        }

        /// <summary>
        /// Regular access to content
        /// </summary>
        /// <param name="str"></param>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetValue(string str, string s, string e)
        {
            var rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))",
                RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(str).Value;
        }


        /// <summary>
        /// Get the milliseconds of the difference between two times
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public static long GetTimeGoneMilliseconds(DateTime starttime,DateTime endtime)
        {
            var ts = endtime.Subtract(starttime);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// end yourself
        /// </summary>
        public static void KillSelf()
        {
            LogWriter.WriteLog("process ended abnormally...");
            var fileName= Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            var ret = GetProcessPid(fileName);
            if (ret > 0)
            {
                KillProcess(ret);
            }
        }
        public static void KillProcess(int pid)
        {
            var cmd = "kill -9 " + pid.ToString();
            LinuxShell.Run(cmd, 1000);
        }
        /// <summary>
        /// get pid
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static int GetProcessPid(string processName)
        {
            var cmd = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                cmd = "ps -aux |grep " + processName + "|grep -v grep|awk \'{print $2}\'";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                cmd = "ps -A |grep " + processName + "|grep -v grep|awk \'{print $1}\'";
            }

            LinuxShell.Run(cmd, 1000, out var std, out var err);
            if (string.IsNullOrEmpty(std) && string.IsNullOrEmpty(err))
            {
                return -1;
            }

            var pid = -1;
            if (!string.IsNullOrEmpty(std))
            {
                int.TryParse(std, out pid);
            }
            if (!string.IsNullOrEmpty(err))
            {
                int.TryParse(err, out pid);
            }
            return pid;
        }
        public static string? GetIngestRtspMonitorUrlIpAddress(string url)
        {
            try
            {
                var link = new Uri(url);
                return link.Host;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Check if it is an ip address
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIpAddr(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static string? AddDoubleQuotation(string s)
        {
            return "\"" + s + "\"";
        }

        public static string? RemoveDoubleQuotation(string s)
        {
            return s.Replace("\"", "").Replace("{", "").Replace("}", "");
        }


        /// <summary>
        /// generate guid
        /// </summary>
        /// <returns></returns>
        public static string? CreateUuid()
        {
            return Guid.NewGuid().ToString("D");
        }

        /// <summary>
        /// Is it GUID
        /// </summary>
        /// <param name="strSrc"></param>
        /// <returns></returns>
        public static bool IsUuidByError(string strSrc)
        {
            if (String.IsNullOrEmpty(strSrc))
            {
                return false;
            }

            var _result = false;
            try
            {
                var _t = new Guid(strSrc);
                _result = true;
            }
            catch
            {
            }

            return _result;
        }
    }
}