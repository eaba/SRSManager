using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using Common = SrsManageCommon.Common;

namespace SrsWebApi
{
    /// <summary>
    /// General class
    /// </summary>
    public class CommonFunctions

    {
        /// <summary>
        /// Check the input parameters of the controller
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static ResponseStruct CheckParams(object[] objs)
        {
            foreach (var obj in objs)
            {
                if (obj == null)
                {
                    return new ResponseStruct()
                    {
                        Code = ErrorNumber.FunctionInputParamsError,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                    };
                }
            }

            return new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
        }


        /// <summary>
        /// Authorization and session verification are not enabled in debug mode
        /// </summary>
        public readonly bool IsDebug = true;

        /// <summary>
        /// base routing address
        /// </summary>
        public string BaseUrl = null!;


        /// <summary>
        /// Configuration file address
        /// </summary>
        public string ConfPath = null!;

        /// <summary>
        /// Session manager
        /// </summary>
        public SessionManager SessionManager = null!;


        /// <summary>
        /// Work list
        /// </summary>
        public string WorkPath = null!;




        /// <summary>
        /// Get timestamp (millisecond level)
        /// </summary>
        /// <returns></returns>
        public long GetTimeStampMilliseconds()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        private bool CheckFFmpegBin(string ffpath = "")
        {
            if (string.IsNullOrEmpty(ffpath))
            {
                ffpath = "ffmpeg";
            }

            LinuxShell.Run(ffpath, 1000, out string std, out string err);
            if (!string.IsNullOrEmpty(std))
            {
                if (std.ToLower().Contains("ffmpeg version"))
                {
                    return true;
                }
            }

            if (!string.IsNullOrEmpty(err))
            {
                if (err.ToLower().Contains("ffmpeg version"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Generic class initialization
        /// </summary>
        public void CommonInit()
        {
            Common.SystemConfig = new SystemConfig();
            WorkPath = Environment.CurrentDirectory + "/";
            ConfPath = WorkPath + "srswebapi.wconf";

            if (Common.SystemConfig.LoadConfig(ConfPath))
            {
                if (!CheckFFmpegBin(Common.FFmpegBinPath))
                {
                    LogWriter.WriteLog("The FFmpeg executable file does not exist, the system exits, please ensure that the FFmpeg executable file exists", Common.FFmpegBinPath);
                    Common.KillSelf();
                }
                BaseUrl = "http://*:" + Common.SystemConfig.HttpPort;
                ErrorMessage.Init();
                SessionManager = new SessionManager();
                SRSApis.Common.Init_SrsServer();
            }
            else
            {
                LogWriter.WriteLog("The system configuration file loads abnormally and the system exits", ConfPath);
                Common.KillSelf();
            }
        }

        /// <summary>
        /// Detect session and allow
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="allowKey"></param>
        /// <param name="sessionCode"></param>
        /// <returns></returns>
        public bool CheckAuth(string ipAddr, string allowKey, string sessionCode)
        {
            if (!CheckSession(sessionCode)) return false;
            if (!CheckAllow(ipAddr, allowKey)) return false;
            return true;
        }

        /// <summary>
        /// check password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckPassword(string password)
        {
            return Common.SystemConfig.Password.Trim().Equals(password.Trim());
        }

        /// <summary>
        /// Check whether the Session is normal
        /// </summary>
        /// <param name="sessionCode"></param>
        /// <returns></returns>
        public bool CheckSession(string sessionCode)
        {
            Session s = this.SessionManager.SessionList.FindLast(x =>
                x.SessionCode.Trim().ToLower().Equals(sessionCode.Trim().ToLower()))!;
            long a = this.GetTimeStampMilliseconds();

            if (s != null && s.Expires > a)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// check appkey
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="allowKey"></param>
        /// <returns></returns>
        public bool CheckAllow(string ipAddr, string allowKey)
        {
            if (Common.SystemConfig.AllowKeys == null ||
                Common.SystemConfig.AllowKeys.Count == 0) return true;
            foreach (var ak in Common.SystemConfig.AllowKeys)
            {
                foreach (var ip in ak.IpArray)
                {
                    string[] ip_tmp;
                    string[] ipAddr_tmp;
                    string ipReal;
                    string ipAddrReal;
                    ipReal = ip;
                    ipAddrReal = ipAddr;
                    if (ip.Trim() == "*" || string.IsNullOrEmpty(ip))
                    {
                        if (allowKey.Trim().ToLower().Equals(ak.Key.Trim().ToLower()))
                        {
                            return true;
                        }

                        return false;
                    }

                    if (ip.Contains('*'))
                    {
                        ip_tmp = ip.Split('.', StringSplitOptions.RemoveEmptyEntries);
                        ipAddr_tmp = ipAddr.Split('.', StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i <= ip_tmp.Length - 1; i++)
                        {
                            if (ip_tmp[i].Trim().Equals("*"))
                            {
                                ipAddr_tmp[i] = "*";
                            }
                        }

                        ipReal = String.Join(".", ip_tmp);
                        ipAddrReal = String.Join(".", ipAddr_tmp);
                    }

                    if (ipReal.Trim().Equals(ipAddrReal.Trim()) &&
                        allowKey.Trim().ToLower().Equals(ak.Key.Trim().ToLower()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Unified processing of apis return results
        /// </summary>
        /// <param name="rt">return value</param>
        /// <param name="rs">ResponseStruct</param>
        /// <returns></returns>
        public JsonResult DelApisResult(object rt, ResponseStruct rs)
        {
            if (rs.Code != (int) ErrorNumber.None)
            {
                return new JsonResult(rs) {StatusCode = (int) HttpStatusCode.BadRequest};
            }

            return new JsonResult(rt) {StatusCode = (int) HttpStatusCode.OK};
        }
    }
}