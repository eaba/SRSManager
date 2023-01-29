using System;
using System.Collections.Generic;
using System.IO;
using SrsApis.SrsManager;
using SRSApis.SystemAutonomy;
using SrsConfFile;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;

namespace SRSApis
{
    public static class Common
    {
        public static readonly string WorkPath = Environment.CurrentDirectory + "/";

        public static List<SrsManager> SrsManagers = new List<SrsManager>();

        /// <summary>
        /// SrsOnlineClient管理
        /// </summary>
        public static SrsClientManager SrsOnlineClient;
        public static SrsAndFFmpegLogMonitor SrsAndFFmpegLogMonitor;
        public static DvrPlanExec? DvrPlanExec = null!;

        // public static BlockingCollection<string>()

        static Common()
        {
            ErrorMessage.Init();
            SrsOnlineClient = new SrsClientManager();
            SrsAndFFmpegLogMonitor = new SrsAndFFmpegLogMonitor();
            DvrPlanExec = new DvrPlanExec();
        }

        /// <summary>
        /// 有没有srs正在运行
        /// </summary>
        /// <returns></returns>
        public static bool HaveAnySrsInstanceRunning()
        {
            if (SrsManagers != null && SrsManagers.Count > 0)
            {
                foreach (var srs in SrsManagers)
                {
                    if (srs.Srs != null)
                        if (srs.IsRunning)
                        {
                            return true;
                        }
                }
            }

            return false;
        }


        /// <summary>
        /// Refresh and write the configuration file of the SRS instance to disk
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool RefreshSrsObject(SrsManager sm, out ResponseStruct rs)
        {
            SrsConfigBuild.Build(sm.Srs, sm.SrsConfigPath);
            LogWriter.WriteLog("Rewrite the Srs configuration file to refresh the Srs instance...", sm.Srs.ConfFilePath!);
            return sm.Reload(out rs);
        }

        /// <summary>
        /// Start the SRS instance
        /// </summary>
        public static void StartServers()
        {
            ResponseStruct rs;
            bool ret;
            foreach (var sm in SrsManagers)
            {
                ret = sm.Start(out rs);
                var rsStr = JsonHelper.ToJson(rs);
                if (ret)
                {
                    LogWriter.WriteLog("SRS started successfully...DeviceID:" + sm.SrsDeviceId, rsStr);
                }
                else
                {
                    LogWriter.WriteLog("SRS startup failed...DeviceID:" + sm.SrsDeviceId, rsStr, ConsoleColor.Yellow);
                }
            }
        }

        /// <summary>
        /// Initialize the SRS instance
        /// </summary>
        public static void Init_SrsServer()
        {
            LogWriter.WriteLog("Initialize the Srs server instance...");
            var dir = new DirectoryInfo(WorkPath);
            ResponseStruct rs;
            var ret = false;
            foreach (var file in dir.GetFiles())
            {
                if (file.Extension.Trim().ToLower().Equals(".conf")) //find the configuration file
                {
                    var sm = new SrsManager();
                    
                    ret = sm.SRS_Init(file.FullName, out rs);
                    var rsStr = JsonHelper.ToJson(rs);
                    if (!ret)
                    {
                        LogWriter.WriteLog("Failed to initialize SRS configuration...ConfigPath:" + file.FullName, rsStr, ConsoleColor.Yellow);
                    }
                    else
                    {
                        LogWriter.WriteLog("Initialize SRS successfully...ConfigPath:" + file.FullName, rsStr);
                        SrsManagers.Add(sm);
                    }
                }
            }

            //SysemController
            if (SrsManagers.Count == 0)
            {
                LogWriter.WriteLog("If there is no Srs instance configuration file, the system will automatically create a Srs instance configuration file");
                var sm = new SrsManager();
                ret = sm.CreateSrsManager(out rs);
                var rsStr = JsonHelper.ToJson(rs);
                if (!ret)
                {
                    LogWriter.WriteLog("Failed to create an SRS instance...:", rsStr, ConsoleColor.Yellow);
                }
                else
                {
                    LogWriter.WriteLog("Initialize SRS successfully...ConfigPath:" + sm.SrsConfigPath, rsStr);
                    SrsManagers.Add(sm);
                }
            }
        }
    }
}