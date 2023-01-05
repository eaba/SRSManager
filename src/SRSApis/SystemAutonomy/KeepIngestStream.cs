using System;
using System.Threading;
using SrsApis.SrsManager.Apis;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.DBMoudle;
using SRSManageCommon.ManageStructs;

namespace SRSApis.SystemAutonomy
{
    /**
     * This type of obsolete, replaced by IngestMonitor
     */
    public class KeepIngestStream
    {
        private int interval = 1000;


        private void DoThing(string deviceId, string vhostDomain, Ingest ingest)
        {
            LogWriter.WriteLog("Reboot Device ID" + deviceId + "under" + vhostDomain + "under" + ingest.IngestName + " Ingest");

            lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
            {
                OrmService.Db.Delete<OnlineClient>().Where(x => x.RtspUrl == ingest.Input!.Url).ExecuteAffrows();
            }

            var retInt = FoundProcess(ingest);
            if (retInt > -1)
            {
                try
                {
                    var cmd = "kill -9 " + retInt.ToString();
                    LinuxShell.Run(cmd, 1000);
                }
                catch (Exception ex)
                {
                    LogWriter.WriteLog(
                        "Reboot Device ID" + deviceId + "under" + vhostDomain + "under" + ingest.IngestName + " fail Ingest",
                        ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
                }
            }

            ResponseStruct rs = null!;
            VhostIngestApis.OnOrOffIngest(deviceId, vhostDomain, ingest.IngestName!, false, out rs);
            SystemApis.RefreshSrsObject(deviceId, out rs);
            Thread.Sleep(1000);
            VhostIngestApis.OnOrOffIngest(deviceId, vhostDomain, ingest.IngestName!, true, out rs);
            SystemApis.RefreshSrsObject(deviceId, out rs);
        }

        private int FoundProcess(Ingest ingest)
        {
            var url = ingest.Input!.Url!.Replace("&", @"\&");
            var cmd = "ps  -aux |grep " + url + "|grep -v grep |awk '{print $2}'";
            LinuxShell.Run(cmd, 1000, out var sdt, out var err);
            if (string.IsNullOrEmpty(sdt) && string.IsNullOrEmpty(err))
            {
                return -1;
            }

            if (int.TryParse(sdt, out var i))
            {
                return i;
            }

            if (int.TryParse(err, out var j))
            {
                return j;
            }

            return -1;
        }

        private bool IngestIsDead(string deviceId, Ingest ingest)
        {
            var onPublishList = FastUsefulApis.GetOnPublishMonitorListByDeviceId(deviceId, out var rs);
            if (onPublishList == null || onPublishList.Count == 0)
            {
                return true;
            }

            var client =
                onPublishList.FindLast(x => !string.IsNullOrEmpty(x.RtspUrl) && x.RtspUrl! == ingest.Input!.Url!);
            if (client != null)
            {
                if (client.IsOnline == false)
                {
                    return true;
                }

                return false;
            }


            return true;
        }

        private void Run()
        {
            while (true)
            {
                try
                {
                    var retDeviceList = SystemApis.GetAllSrsManagerDeviceId();
                    if (retDeviceList != null && retDeviceList.Count > 0)
                    {
                        foreach (var dev in retDeviceList)
                        {
                            if (string.IsNullOrEmpty(dev)) continue;
                            var retSrsManager = SystemApis.GetSrsManagerInstanceByDeviceId(dev);
                            if (retSrsManager == null || retSrsManager.Srs == null || !retSrsManager.IsRunning)
                                continue;
                            var retSrsVhostList =
                                VhostApis.GetVhostList(retSrsManager.SrsDeviceId, out var rs);
                            if (retSrsVhostList == null || retSrsVhostList.Count == 0) continue;
                            foreach (var vhost in retSrsVhostList)
                            {
                                if (vhost == null || vhost.Vingests == null || vhost.Vingests.Count == 0) continue;
                                foreach (var ingest in vhost.Vingests)
                                {
                                    if (ingest.Enabled == false) continue;
                                    if (IngestIsDead(dev, ingest))
                                    {
                                        LogWriter.WriteLog(
                                            "Monitored to the device ID" + dev + "under" + vhost.VhostDomain + "under" + ingest.IngestName +
                                            " In an abnormal state, restart the ingest immediately", "", ConsoleColor.Red);
                                        DoThing(dev, vhost.VhostDomain!, ingest);
                                    }

                                    Thread.Sleep(30);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogWriter.WriteLog("Ingest guard exception...", ex.Message + "\r\n" + ex.StackTrace);
                }

                Thread.Sleep(interval);
            }
        }

        public KeepIngestStream()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    LogWriter.WriteLog("Start the Ingest daemon service... (cycle interval：" + interval + "ms)");
                    Run();
                }
                catch (Exception ex)
                {
                    LogWriter.WriteLog("Failed to start the Ingest daemon service...", ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
                    Console.WriteLine(ex.Message);
                }
            })).Start();
        }
    }
}