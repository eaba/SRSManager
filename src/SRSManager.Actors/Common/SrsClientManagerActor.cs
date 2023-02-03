

using Akka.Actor;
using Akka.Event;
using SrsApis.SrsManager.Apis;
using SRSManageCommon.ManageStructs;
using SrsManageCommon;
using SrsApis.SrsManager;
using SRSManager.Messages;
using SrsManageCommon.SrsManageCommon;
using SharpPulsar.Trino.Message;
using SharpPulsar.Trino;
using SharpPulsar;
using System.Text.Json;

namespace SRSManager.Actors.Actor
{
    internal class SrsClientManagerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log;
        private readonly PulsarSystem _pulsarSystem;
        private readonly string _tenant;
        private readonly string _namespace;
        private readonly string _trinoUrl;
        private readonly string _deviceId;
        public SrsClientManagerActor(string deviceId, PulsarSystem pulsarSystem, string tenant, string namespac, string trinoUrl)
        {
             _pulsarSystem = pulsarSystem; 
            _tenant = tenant;   
            _namespace = namespac;  
            _trinoUrl = trinoUrl; 
            _deviceId = deviceId;   
        }
        private int interval = Common.SystemConfig.SrsClientManagerServiceinterval;

        private async ValueTask RewriteMonitorType()
        {
            try
            {
                var sm = await Context.Parent.Ask<SrsManager>(GetSrsManager.Instance);
                if (sm.IsInit && sm.Srs != null && sm.IsRunning)
                {
                    var query = @$"select * from OnlineClient WHERE IsOnline = 'true' AND ClientType = '{ClientType.Monitor}' AND Device_I = '{_deviceId}' Order By __publish_time__ ASC";

                    var onPublishList = await Sql(query,_tenant, _namespace, _trinoUrl); 
                    if (onPublishList == null || onPublishList.Count == 0) return;
                    var ingestList = await Context.Parent.Ask<SrsManager>(new FastUseful(_deviceId, "GetAllIngestByDeviceId")); 

                    var port = sm.Srs.Http_api!.Listen;
                    List<Channels> ret28181 = null!;
                    if (port != null && sm.Srs != null && sm.Srs.Http_api != null &&
                        sm.Srs.Http_api.Enabled == true)
                    {
                        ret28181 = GetGB28181Channels("http://127.0.0.1:" + port.ToString());
                    }

                    foreach (var client in onPublishList)
                    {
                        if (sm.Srs!.Http_api == null || sm.Srs.Http_api.Enabled == false) continue;

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
        /// <summary>
        /// Get all published cameras
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<OnlineClient> GetOnPublishMonitorListByDeviceId(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (string.IsNullOrEmpty(deviceId))
            {
                rs.Code = ErrorNumber.FunctionInputParamsError;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                return null!;
            }

            lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
            {
                var result = OrmService.Db.Select<OnlineClient>()
                    .Where(x => x.IsOnline == true && x.ClientType == ClientType.Monitor &&
                                x.Device_Id!.Equals(deviceId.Trim())).ToList();
                return result;
            }
        }
        private async ValueTask<List<OnlineClient>> Sql(string query, string tenant, string namespac, string trinoUrl)
        {
            var option = new ClientOptions
            {
                Server = trinoUrl,
                Execute = query,
                Catalog = "pulsar",
                Schema = $"{tenant}/{namespac}"
            };
            var sql = new SqlInstance(_pulsarSystem.System, option);
            var data = await sql.ExecuteAsync();
            var dvr = new List<OnlineClient>();
            switch (data.Response)
            {
                case StatsResponse stats:
                    _log.Info(JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case DataResponse dt:
                    for (var i = 0; i < dt.Data.Count; i++)
                    {
                        var ob = dt.Data.ElementAt(i);
                        var json = JsonSerializer.Serialize(ob, new JsonSerializerOptions { WriteIndented = true });
                        dvr.Add(JsonSerializer.Deserialize<OnlineClient>(json)!);
                    }
                    _log.Info(JsonSerializer.Serialize(dt.StatementStats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case ErrorResponse er:
                    _log.Info(JsonSerializer.Serialize(er, new JsonSerializerOptions { WriteIndented = true }));
                    break;
            }
            return dvr;
        }

    }
}
