using System;
using System.Collections.Generic;
using System.Threading;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SRSManageCommon.DBMoudle;
using SRSManageCommon.ManageStructs;
using SrsManageCommon.SrsManageCommon;

namespace SRSApis.SystemAutonomy
{
    public class SrsClientManager
    {
        private int interval = SrsManageCommon.Common.SystemConfig.SrsClientManagerServiceinterval;

        private void RewriteMonitorType()
        {
            try
            {
                if (Common.SrsManagers != null)
                {
                    foreach (var srs in Common.SrsManagers)
                    {
                        if (srs == null || srs.Srs == null) continue;

                        if (srs.IsInit && srs.Srs != null && srs.IsRunning)
                        {
                            var onPublishList =
                                FastUsefulApis.GetOnPublishMonitorListByDeviceId(srs.SrsDeviceId,
                                    out var rs);
                            if (onPublishList == null || onPublishList.Count == 0) continue;
                            var ingestList = FastUsefulApis.GetAllIngestByDeviceId(srs.SrsDeviceId, out rs);

                            var port = srs.Srs.Http_api!.Listen;
                            List<Channels> ret28181 = null!;
                            if (port != null && srs.Srs != null && srs.Srs.Http_api != null &&
                                srs.Srs.Http_api.Enabled == true)
                            {
                                ret28181 = GetGB28181Channels("http://127.0.0.1:" + port.ToString());
                            }

                            foreach (var client in onPublishList)
                            {
                                if (srs.Srs!.Http_api == null || srs.Srs.Http_api.Enabled == false) continue;

                                #region Handle 28181 devices

                                if (ret28181 != null)
                                {
                                    foreach (var r in ret28181)
                                    {
                                        if (!string.IsNullOrEmpty(r.Stream) && r.Stream.Equals(client.Stream))
                                        {
                                            lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
                                            {
                                                var reti = OrmService.Db.Update<OnlineClient>()
                                                    .Set(x => x.MonitorType, MonitorType.GBT28181)
                                                    .Where(x => x.Client_Id == client.Client_Id)
                                                    .ExecuteAffrows();
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #region Handle onvif devices

                                if (ingestList != null && ingestList.Count > 0)
                                {
                                    foreach (var ingest in ingestList)
                                    {
                                        if (ingest != null && ingest.Input != null
                                                           && client.RtspUrl != null &&
                                                           ingest.Input!.Url!.Equals(client.RtspUrl))
                                        {
                                            lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
                                            {
                                                var reti = OrmService.Db.Update<OnlineClient>()
                                                    .Set(x => x.MonitorType, MonitorType.Onvif)
                                                    .Where(x => x.Client_Id == client.Client_Id)
                                                    .ExecuteAffrows();
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #region Handle live streams

                                var retj = OrmService.Db.Update<OnlineClient>()
                                    .Set(x => x.MonitorType, MonitorType.Webcast)
                                    .Where(x => x.MonitorType == MonitorType.Unknow &&
                                                x.ClientType == ClientType.Monitor)
                                    .ExecuteAffrows();

                                #endregion
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteLog("rewriteMonitorType exception", ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
            }
        }

        private List<Channels> GetGB28181Channels(string httpUri)
        {
            var act = "/api/v1/gb28181?action=query_channel";
            var url = httpUri + act;
            try
            {
                var tmpStr = NetHelperNew.HttpGetRequest(url, null!);
                var ret = JsonHelper.FromJson<SrsT28181QueryChannelModule>(tmpStr);
                if (ret.Code == 0 && ret.Data != null)
                {
                    return ret.Data.Channels!;
                }

                return null!;
            }
            catch (Exception ex)
            {
                LogWriter.WriteLog("Obtaining SRS-GB28181 channel data is abnormal...", ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
                return null!;
            }
        }

        private void CompletionOnvifIpAddress()
        {
            try
            {
                if (Common.SrsManagers != null)
                {
                    foreach (var srs in Common.SrsManagers)
                    {
                        if (srs == null || srs.Srs == null) continue;
                        if (srs.IsInit && srs.Srs != null && srs.IsRunning)
                        {
                            var ret = VhostIngestApis.GetVhostIngestNameList(srs.SrsDeviceId, out var rs);
                            if (ret != null)
                            {
                                foreach (var r in ret)
                                {
                                    var ingest = VhostIngestApis.GetVhostIngest(srs.SrsDeviceId, r.VhostDomain!,
                                        r.IngestInstanceName!,
                                        out rs);

                                    if (ingest != null)
                                    {
                                        var inputIp =
                                            SrsManageCommon.Common
                                                .GetIngestRtspMonitorUrlIpAddress(ingest.Input!.Url!)!;
                                        if (SrsManageCommon.Common.IsIpAddr(inputIp!))
                                        {
                                            lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
                                            {
                                                var reti = OrmService.Db.Update<OnlineClient>()
                                                    .Set(x => x.MonitorIp, inputIp)
                                                    .Set(x => x.RtspUrl, ingest.Input!.Url!)
                                                    .Where(x => x.Stream!.Equals(ingest.IngestName) &&
                                                                x.Device_Id!.Equals(srs.SrsDeviceId) &&
                                                                (x.MonitorIp == null || x.MonitorIp == "" ||
                                                                 x.MonitorIp == "127.0.0.1"))
                                                    .ExecuteAffrows();
                                                if (reti > 0)
                                                {
                                                    LogWriter.WriteLog("Complete the IP address of the camera in the Ingest streamer...",
                                                        srs.SrsDeviceId + "/" + r.VhostDomain + "/" +
                                                        ingest.IngestName +
                                                        " Get the IP:" + inputIp + " Get the Rtsp address:" + ingest.Input!.Url);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteLog("completionOnvifIpAddress exception", ex.Message + "\r\n" + ex.StackTrace,
                    ConsoleColor.Yellow);
            }
        }

        private void CompletionT28181IpAddress()
        {
            try
            {
                if (Common.SrsManagers != null)
                {
                    foreach (var srs in Common.SrsManagers)
                    {
                        if (srs == null || srs.Srs == null) continue;
                        if (srs.IsInit && srs.Srs != null && srs.IsRunning)
                        {
                            var port = srs.Srs.Http_api!.Listen;
                            if (port == null || srs.Srs.Http_api == null || srs.Srs.Http_api.Enabled == false)
                                continue;
                            var ret = GetGB28181Channels("http://127.0.0.1:" + port.ToString());
                            if (ret != null)
                            {
                                foreach (var r in ret)
                                {
                                    if (!string.IsNullOrEmpty(r.Rtp_Peer_Ip) && !string.IsNullOrEmpty(r.Stream))
                                    {
                                        lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
                                        {
                                            var reti = OrmService.Db.Update<OnlineClient>()
                                                .Set(x => x.MonitorIp, r.Rtp_Peer_Ip)
                                                .Where(x => x.Stream!.Equals(r.Stream) &&
                                                            x.Device_Id!.Equals(srs.SrsDeviceId) &&
                                                            (x.MonitorIp == null || x.MonitorIp == "" ||
                                                             x.MonitorIp == "127.0.0.1"))
                                                .ExecuteAffrows();
                                            if (reti > 0)
                                            {
                                                LogWriter.WriteLog("Complete the IP address of the camera in the StreamCaster receiver...",
                                                    srs.SrsDeviceId + "/" + r.Stream + " Get the IP:" + r.Rtp_Peer_Ip);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteLog("completionT28181IpAddress exception", ex.Message + "\r\n" + ex.StackTrace,
                    ConsoleColor.Yellow);
            }
        }

        private void ClearOfflinePlayerUser()
        {
            try
            {
                if (Common.HaveAnySrsInstanceRunning())
                {
                    lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
                    {
                        var re = OrmService.Db.Delete<OnlineClient>().Where(x => x.ClientType == ClientType.User &&
                                                                                 x.IsPlay == false &&
                                                                                 x.UpdateTime <=
                                                                                 DateTime.Now.AddMinutes(-3))
                            .ExecuteAffrows();
                        if (re > 0)
                        {
                            LogWriter.WriteLog("Clean up dead client playback connections... clean up the number：" + re);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteLog("clearOfflinePlayerUser", ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
            }
        }

        private void Run()
        {
            while (true)
            {
                #region Complete the monitorip address from ingest

                CompletionOnvifIpAddress();

                Thread.Sleep(500);

                #endregion

                #region Supplement 28181 monitorip address

                CompletionT28181IpAddress();

                Thread.Sleep(500);

                #endregion

                #region Delete non-playing clients of the user type that have not been updated for a long time

                ClearOfflinePlayerUser();

                Thread.Sleep(500);

                #endregion

                #region Override camera type

                RewriteMonitorType();

                Thread.Sleep(500);

                #endregion


                Thread.Sleep(interval);
            }
        }

        public SrsClientManager()
        {
            new Thread(new ThreadStart(delegate

            {
                try
                {
                    LogWriter.WriteLog("Start the client monitoring service... (cycle interval：" + interval + "ms)");
                    Run();
                }
                catch (Exception ex)
                {
                    LogWriter.WriteLog("Failed to start client monitoring service...", ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
                }
            })).Start();
        }
    }
}