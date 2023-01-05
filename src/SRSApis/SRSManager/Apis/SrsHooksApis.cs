using System;
using SrsManageCommon;
using SRSManageCommon.DBMoudle;
using SRSManageCommon.ManageStructs;

namespace SrsApis.SrsManager.Apis
{
    /// <summary>
    /// SRSHookApis
    /// </summary>
    public static class SrsHooksApis
    {
        /// <summary>
        /// Handle Dvr information
        /// </summary>
        /// <param name="dvrVideo"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool OnDvr(DvrVideo dvrVideo)
        {
            try
            {
                dvrVideo.Deleted = false;
                dvrVideo.Undo = false;
                dvrVideo.UpdateTime = DateTime.Now;
                var onPublishList =
                    FastUsefulApis.GetOnPublishMonitorListByDeviceId(dvrVideo.Device_Id!, out var rs);
                var ret = onPublishList.FindLast(x => x.Client_Id == dvrVideo.Client_Id);
                if (ret != null)
                {
                    dvrVideo.ClientIp = ret.MonitorIp;
                    dvrVideo.MonitorType = ret.MonitorType;
                    dvrVideo.RecordDate = DateTime.Now.ToString("yyyy-MM-dd");
                }

                var srs = SystemApis.GetSrsManagerInstanceByDeviceId(dvrVideo.Device_Id!);
                dvrVideo.Url = ":" + srs.Srs.Http_server!.Listen +
                               dvrVideo.VideoPath!.Replace(srs.Srs.Http_server.Dir!, "");
                lock (Common.LockDbObjForDvrVideo)
                {
                    OrmService.Db.Insert(dvrVideo).ExecuteAffrows();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("save DVR error：" + ex.Message);
            }

            return true;
        }


        /// <summary>
        /// Process srs heartbeat information
        /// </summary>
        /// <param name="heartbeat"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool OnHeartbeat(ReqSrsHeartbeat heartbeat, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var jsonStr = JsonHelper.ToJson(heartbeat);
            jsonStr = JsonHelper.ConvertJsonString(jsonStr);
            lock (Common.LockDbObjForHeartbeat)
            {
                return true;
            }
        }

        /// <summary>
        /// when the connection is closed
        /// </summary>
        /// <param name="onlineClient"></param>
        /// <returns></returns>
        public static bool OnClose(OnlineClient onlineClient)
        {
            if (onlineClient != null && !string.IsNullOrEmpty(onlineClient.ClientIp) && onlineClient.Client_Id != null)
            {
                try
                {
                    lock (Common.LockDbObjForOnlineClient)
                    {
                        var tmpClientLog = new ClientLog();
                        tmpClientLog.EventMethod = EventMethod.Close;
                        tmpClientLog.App = onlineClient.App;
                        tmpClientLog.Param = onlineClient.Param;
                        tmpClientLog.Stream = onlineClient.Stream;
                        tmpClientLog.Vhost = onlineClient.Vhost;
                        tmpClientLog.Client_Id = onlineClient.Client_Id;
                        tmpClientLog.ClientIp = onlineClient.ClientIp;
                        tmpClientLog.ClientType = onlineClient.ClientType;
                        tmpClientLog.Device_Id = onlineClient.Device_Id;
                        tmpClientLog.HttpUrl = onlineClient.HttpUrl;
                        tmpClientLog.IsOnline = false;
                        tmpClientLog.IsPlay = false;
                        tmpClientLog.MonitorIp = onlineClient.MonitorIp;
                        tmpClientLog.MonitorType = onlineClient.MonitorType;
                        tmpClientLog.PageUrl = onlineClient.PageUrl;
                        tmpClientLog.RtmpUrl = onlineClient.RtmpUrl;
                        tmpClientLog.RtspUrl = onlineClient.RtspUrl;
                        tmpClientLog.UpdateTime = DateTime.Now;
                        OrmService.Db.Insert<ClientLog>(tmpClientLog).ExecuteAffrows();
                        var retPlan = LiveBroadcastApis.CheckIsLiveCh(onlineClient, out var rs);

                        if (retPlan != null)
                        {
                            lock (Common.LockDbObjForLivePlan)
                            {
                                OrmService.Db.Update<LiveBroadcastPlan>().Set(x => x.UpdateTime, DateTime.Now)
                                    .Set(x => x.PlanStatus, LiveBroadcastPlanStatus.Finished)
                                    .Where(x => x.Id == retPlan!.Id).ExecuteAffrows();
                            }
                        }
                        
                        OrmService.Db.Delete<OnlineClient>()
                            .Where(x => x.Client_Id == onlineClient.Client_Id && x.ClientIp == onlineClient.ClientIp)
                            .ExecuteAffrows();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+"\r\n"+ex.StackTrace);
                    return false;
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// Obtain monitorip address through stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static string GetMonitorIpAddressFromStream(string stream)
        {
            lock (Common.LockDbObjForOnlineClient)
            {
                var ret = OrmService.Db.Select<OnlineClient>()
                    .Where(x => x.Stream!.Trim().Equals(stream.Trim()) && x.ClientType == ClientType.Monitor).First();

                if (ret != null)
                {
                    return ret.MonitorIp!;
                }
            }


            return "unknow";
        }

        /// <summary>
        /// When the client plays
        /// </summary>
        /// <param name="onlineClient"></param>
        /// <returns></returns>
        public static bool OnPlay(OnlineClient onlineClient)
        {
            if (onlineClient != null && onlineClient.Client_Id != null &&
                !string.IsNullOrEmpty(onlineClient.Device_Id))
            {
                try
                {
                    lock (Common.LockDbObjForOnlineClient)
                    {
                        var tmpClientLog = new ClientLog();
                        var monitorIp = GetMonitorIpAddressFromStream(onlineClient.Stream!);
                        tmpClientLog.EventMethod = EventMethod.Play;
                        tmpClientLog.App = onlineClient.App;
                        tmpClientLog.Param = onlineClient.Param;
                        tmpClientLog.Stream = onlineClient.Stream;
                        tmpClientLog.Vhost = onlineClient.Vhost;
                        tmpClientLog.Client_Id = onlineClient.Client_Id;
                        tmpClientLog.ClientIp = onlineClient.ClientIp;
                        tmpClientLog.ClientType = ClientType.User;
                        tmpClientLog.Device_Id = onlineClient.Device_Id;
                        tmpClientLog.HttpUrl = onlineClient.HttpUrl;
                        tmpClientLog.IsOnline = true;
                        tmpClientLog.IsPlay = true;
                        tmpClientLog.MonitorIp = monitorIp;
                        tmpClientLog.MonitorType = onlineClient.MonitorType;
                        tmpClientLog.PageUrl = onlineClient.PageUrl;
                        tmpClientLog.RtmpUrl = onlineClient.RtmpUrl;
                        tmpClientLog.RtspUrl = onlineClient.RtspUrl;
                        tmpClientLog.UpdateTime = DateTime.Now;

                        OrmService.Db.Insert<ClientLog>(tmpClientLog).ExecuteAffrows();
                        var ret = OrmService.Db.Update<OnlineClient>().Set(x => x.ClientType, ClientType.User)
                            .Set(x => x.IsOnline, true).Set(x => x.IsPlay, true)
                            .Set(x => x.Param, onlineClient.Param)
                            .Set(x => x.Stream, onlineClient.Stream).Set(x => x.UpdateTime, onlineClient.UpdateTime)
                            .Set(x => x.PageUrl, onlineClient.PageUrl).Set(x => x.MonitorIp,
                                monitorIp)
                            .Where(x => x.Client_Id == onlineClient.Client_Id &&
                                        x.Device_Id!.Trim() == onlineClient.Device_Id.Trim())
                            .ExecuteAffrows();
                        if (ret <= 0)
                        {
                            onlineClient.ClientType = ClientType.User;
                            onlineClient.IsOnline = true;
                            onlineClient.IsPlay = true;
                            onlineClient.MonitorIp = GetMonitorIpAddressFromStream(onlineClient.Stream!);
                            var retInsert = OrmService.Db.Insert(onlineClient).ExecuteAffrows();
                            if (retInsert > 0)
                            {
                                return true;
                            }

                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+"\r\n"+ex.StackTrace);
                    return false;
                }
            }

            return false;
        }


        /// <summary>
        /// When the client stops playing
        /// </summary>
        /// <param name="onlineClient"></param>
        /// <returns></returns>
        public static bool OnStop(OnlineClient onlineClient)
        {
            if (onlineClient != null && !string.IsNullOrEmpty(onlineClient.Device_Id) &&
                onlineClient.Client_Id != null)
            {
                try
                {
                    lock (Common.LockDbObjForOnlineClient)
                    {
                        var tmpClientLog = new ClientLog();
                        tmpClientLog.EventMethod = EventMethod.Stop;
                        tmpClientLog.App = onlineClient.App;
                        tmpClientLog.Param = onlineClient.Param;
                        tmpClientLog.Stream = onlineClient.Stream;
                        tmpClientLog.Vhost = onlineClient.Vhost;
                        tmpClientLog.Client_Id = onlineClient.Client_Id;
                        tmpClientLog.ClientIp = onlineClient.ClientIp;
                        tmpClientLog.ClientType = ClientType.User;
                        tmpClientLog.Device_Id = onlineClient.Device_Id;
                        tmpClientLog.HttpUrl = onlineClient.HttpUrl;
                        tmpClientLog.IsOnline = true;
                        tmpClientLog.IsPlay = false;
                        tmpClientLog.MonitorIp = onlineClient.MonitorIp;
                        tmpClientLog.MonitorType = onlineClient.MonitorType;
                        tmpClientLog.PageUrl = onlineClient.PageUrl;
                        tmpClientLog.RtmpUrl = onlineClient.RtmpUrl;
                        tmpClientLog.RtspUrl = onlineClient.RtspUrl;
                        tmpClientLog.UpdateTime = DateTime.Now;
                        OrmService.Db.Insert<ClientLog>(tmpClientLog).ExecuteAffrows();
                        var ret = OrmService.Db.Update<OnlineClient>().Set(x => x.ClientType, ClientType.User)
                            .Set(x => x.IsOnline, true).Set(x => x.IsPlay, false)
                            .Set(x => x.UpdateTime, onlineClient.UpdateTime)
                            .Where(x => x.Client_Id == onlineClient.Client_Id &&
                                        x.Device_Id!.Trim() == onlineClient.Device_Id.Trim())
                            .ExecuteAffrows();
                        if (ret <= 0)
                        {
                            onlineClient.ClientType = ClientType.User;
                            onlineClient.IsOnline = true;
                            onlineClient.IsPlay = false;
                            var retInsert = OrmService.Db.Insert(onlineClient).ExecuteAffrows();
                            if (retInsert > 0)
                            {
                                return true;
                            }

                            return false;
                        }
                    }


                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+"\r\n"+ex.StackTrace);
                    return false;
                }
            }

            return false;
        }


        /// <summary>
        /// When streaming from the camera
        /// </summary>
        /// <param name="onlineClient"></param>
        /// <returns></returns>
        public static bool OnPublish(OnlineClient onlineClient)
        {
            if (onlineClient != null && !string.IsNullOrEmpty(onlineClient.Device_Id) &&
                onlineClient.Client_Id != null)
            {
                try
                {
                    lock (Common.LockDbObjForOnlineClient)
                    {
                        var tmpClientLog = new ClientLog();
                        tmpClientLog.EventMethod = EventMethod.Publish;
                        tmpClientLog.App = onlineClient.App;
                        tmpClientLog.Param = onlineClient.Param;
                        tmpClientLog.Stream = onlineClient.Stream;
                        tmpClientLog.Vhost = onlineClient.Vhost;
                        tmpClientLog.Client_Id = onlineClient.Client_Id;
                        tmpClientLog.ClientIp = onlineClient.ClientIp;
                        tmpClientLog.ClientType = ClientType.Monitor;
                        tmpClientLog.Device_Id = onlineClient.Device_Id;
                        tmpClientLog.HttpUrl = onlineClient.HttpUrl;
                        tmpClientLog.IsOnline = true;
                        tmpClientLog.IsPlay = false;
                        tmpClientLog.MonitorIp = onlineClient.MonitorIp;
                        tmpClientLog.MonitorType = MonitorType.Unknow;
                        tmpClientLog.PageUrl = onlineClient.PageUrl;
                        tmpClientLog.RtmpUrl = onlineClient.RtmpUrl;
                        tmpClientLog.RtspUrl = onlineClient.RtspUrl;
                        tmpClientLog.UpdateTime = DateTime.Now;
                        OrmService.Db.Insert<ClientLog>(tmpClientLog).ExecuteAffrows();
                        var retPlan = LiveBroadcastApis.CheckIsLiveCh(onlineClient, out var rs);
                        var retBool = LiveBroadcastApis.CheckLivePlan(retPlan, out rs); //If it is not in the live broadcast plan, kick the connection
                        if (!retBool && retPlan != null)
                        {
                            return false;
                        }

                        if (retPlan != null)
                        {
                            lock (Common.LockDbObjForLivePlan)
                            {

                                OrmService.Db.Update<LiveBroadcastPlan>().Set(x => x.UpdateTime, DateTime.Now)
                                    .Set(x => x.PlanStatus, LiveBroadcastPlanStatus.Living)
                                    .Where(x => x.Id == retPlan!.Id).ExecuteAffrows();
                            }
                        }

                        var ret = OrmService.Db.Update<OnlineClient>().Set(x => x.ClientType, ClientType.Monitor)
                            .Set(x => x.IsOnline, true).Set(x => x.HttpUrl, onlineClient.HttpUrl)
                            .Set(x => x.Param, onlineClient.Param)
                            .Set(x => x.Stream, onlineClient.Stream).Set(x => x.UpdateTime, onlineClient.UpdateTime)
                            .Set(x => x.ClientType, ClientType.Monitor)
                            .Where(x => x.Client_Id == onlineClient.Client_Id &&
                                        x.Device_Id!.Trim() == onlineClient.Device_Id.Trim())
                            .ExecuteAffrows();
                        if (ret <= 0)
                        {
                            if (retPlan != null && retBool)
                            {
                                onlineClient.ClientType = ClientType.User;
                                onlineClient.MonitorType = MonitorType.Webcast;
                            }
                            else
                            {
                                onlineClient.ClientType = ClientType.Monitor;
                                onlineClient.MonitorType = MonitorType.Unknow;
                            }
                            
                            onlineClient.IsOnline = true;
                            var retInsert = OrmService.Db.Insert(onlineClient).ExecuteAffrows();
                            if (retInsert > 0)
                            {
                                return true;
                            }

                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+"\r\n"+ex.StackTrace);
                    return false;
                }
            }

            return false;
        }


        /// <summary>
        /// When the camera stops streaming
        /// </summary>
        /// <param name="onlineClient"></param>
        /// <returns></returns>
        public static bool OnUnPublish(OnlineClient onlineClient)
        {
            if (onlineClient != null && !string.IsNullOrEmpty(onlineClient.Device_Id) &&
                onlineClient.Client_Id != null)
            {
                try
                {
                    lock (Common.LockDbObjForOnlineClient)
                    {
                        var tmpClientLog = new ClientLog();
                        tmpClientLog.EventMethod = EventMethod.UnPublish;
                        tmpClientLog.App = onlineClient.App;
                        tmpClientLog.Param = onlineClient.Param;
                        tmpClientLog.Stream = onlineClient.Stream;
                        tmpClientLog.Vhost = onlineClient.Vhost;
                        tmpClientLog.Client_Id = onlineClient.Client_Id;
                        tmpClientLog.ClientIp = onlineClient.ClientIp;
                        tmpClientLog.ClientType = ClientType.Monitor;
                        tmpClientLog.Device_Id = onlineClient.Device_Id;
                        tmpClientLog.HttpUrl = onlineClient.HttpUrl;
                        tmpClientLog.IsOnline = false;
                        tmpClientLog.IsPlay = false;
                        tmpClientLog.MonitorIp = onlineClient.MonitorIp;
                        tmpClientLog.MonitorType = onlineClient.MonitorType;
                        tmpClientLog.PageUrl = onlineClient.PageUrl;
                        tmpClientLog.RtmpUrl = onlineClient.RtmpUrl;
                        tmpClientLog.RtspUrl = onlineClient.RtspUrl;
                        tmpClientLog.UpdateTime = DateTime.Now;
                        OrmService.Db.Insert<ClientLog>(tmpClientLog).ExecuteAffrows();
                        var retPlan = LiveBroadcastApis.CheckIsLiveCh(onlineClient, out var rs);

                        if (retPlan != null)
                        {
                            lock (Common.LockDbObjForLivePlan)
                            {
                                OrmService.Db.Update<LiveBroadcastPlan>().Set(x => x.UpdateTime, DateTime.Now)
                                    .Set(x => x.PlanStatus, LiveBroadcastPlanStatus.Finished)
                                    .Where(x => x.Id == retPlan!.Id).ExecuteAffrows();
                            }
                        }

                        var ret = OrmService.Db.Update<OnlineClient>()
                            .Set(x => x.IsOnline, false).Set(x => x.UpdateTime, onlineClient.UpdateTime)
                            .Where(x => x.Client_Id == onlineClient.Client_Id &&
                                        x.Device_Id!.Trim() == onlineClient.Device_Id.Trim())
                            .ExecuteAffrows();
                        if (ret <= 0)
                        {
                            if (retPlan == null)
                            {
                                onlineClient.ClientType = ClientType.Monitor; 
                            }
                            else
                            {
                                onlineClient.ClientType = ClientType.User;
                                onlineClient.MonitorType = MonitorType.Webcast;
                            }
                            onlineClient.IsOnline = false;
                            var retInsert = OrmService.Db.Insert(onlineClient).ExecuteAffrows();
                            if (retInsert > 0)
                            {
                                return true;
                            }

                            return false;
                        }
                    }


                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+"\r\n"+ex.StackTrace);
                    return false;
                }
            }

            return false;
        }


        /// <summary>
        /// when a device is connected
        /// </summary>
        /// <param name="onlineClient"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool OnConnect(OnlineClient onlineClient)
        {
            if (onlineClient != null && !string.IsNullOrEmpty(onlineClient.Device_Id) &&
                onlineClient.Client_Id != null)
            {
                try
                {
                    lock (Common.LockDbObjForOnlineClient)
                    {
                        var tmpClientLog = new ClientLog();
                        tmpClientLog.EventMethod = EventMethod.Connect;
                        tmpClientLog.App = onlineClient.App;
                        tmpClientLog.Param = onlineClient.Param;
                        tmpClientLog.Stream = onlineClient.Stream;
                        tmpClientLog.Vhost = onlineClient.Vhost;
                        tmpClientLog.Client_Id = onlineClient.Client_Id;
                        tmpClientLog.ClientIp = onlineClient.ClientIp;
                        tmpClientLog.ClientType = ClientType.Monitor;
                        tmpClientLog.Device_Id = onlineClient.Device_Id;
                        tmpClientLog.HttpUrl = onlineClient.HttpUrl;
                        tmpClientLog.IsOnline = true;
                        tmpClientLog.IsPlay = false;
                        tmpClientLog.MonitorIp = onlineClient.MonitorIp;
                        tmpClientLog.MonitorType = MonitorType.Unknow;
                        tmpClientLog.PageUrl = onlineClient.PageUrl;
                        tmpClientLog.RtmpUrl = onlineClient.RtmpUrl;
                        tmpClientLog.RtspUrl = onlineClient.RtspUrl;
                        tmpClientLog.UpdateTime = DateTime.Now;
                        OrmService.Db.Insert<ClientLog>(tmpClientLog).ExecuteAffrows();

                        var retPlan = LiveBroadcastApis.CheckIsLiveCh(onlineClient, out var rs);
                        var retBool = LiveBroadcastApis.CheckLivePlan(retPlan, out rs); //如果不是直播计划内的就踢掉连接
                        if (!retBool && retPlan != null)
                        {
                            return false;
                        }


                        var ret = OrmService.Db.Update<OnlineClient>().Set(x => x.ClientType, ClientType.Monitor)
                            .Set(x => x.IsOnline, true).Set(x => x.HttpUrl, onlineClient.HttpUrl)
                            .Set(x => x.Param, onlineClient.Param)
                            .Set(x => x.Stream, onlineClient.Stream).Set(x => x.UpdateTime, onlineClient.UpdateTime)
                            .Set(x => x.MonitorIp, onlineClient.ClientIp)
                            .Where(x => x.Client_Id == onlineClient.Client_Id &&
                                        x.Device_Id!.Trim() == onlineClient.Device_Id.Trim())
                            .ExecuteAffrows();
                        if (ret <= 0)
                        {
                            onlineClient.ClientType = ClientType.Monitor;
                            onlineClient.IsOnline = true;
                            onlineClient.MonitorIp = onlineClient.ClientIp;
                            onlineClient.MonitorType = MonitorType.Unknow;
                            var retInsert = OrmService.Db.Insert(onlineClient).ExecuteAffrows();
                            if (retInsert > 0)
                            {
                                return true;
                            }

                            return false;
                        }
                    }


                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+"\r\n"+ex.StackTrace);
                    return false;
                }
            }

            return false;
        }
    }
}