using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.DBMoudle;
using SRSManageCommon.ManageStructs;
using SrsManageCommon.SrsManageCommon;
using Common = SRSApis.Common;
using Publish = SrsConfFile.SRSConfClass.Publish;

namespace SrsApis.SrsManager.Apis
{
    public static class FastUsefulApis
    {
        
        /// <summary>
        /// Get a flow information under ingest
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="ingestName"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SrsLiveStream GetStreamInfoByVhostIngestName(string deviceId, string vhostDomain,
            string ingestName, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.SrsManagers == null || Common.SrsManagers.Count == 0)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return null!;
            }

            var rpet = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToLower().Equals(deviceId.Trim().ToLower()));
            if (rpet == null)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return null!;
            }

            var retIngest = VhostIngestApis.GetVhostIngest(deviceId, vhostDomain, ingestName, out rs);
            if (retIngest == null)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return null!;
            }

            try
            {
                var uri = new Uri(retIngest.Engines![0].Output!);
                var uriInput = new Uri(retIngest.Input!.Url!);
                var userInfo = uriInput.UserInfo;
                var username = "";
                var password = "";
                if (userInfo.Contains(":"))
                {
                    var strArr = userInfo.Split(":", StringSplitOptions.RemoveEmptyEntries);
                    if (strArr.Length == 2)
                    {
                        username = strArr[0].Trim();
                        password = strArr[1].Trim();
                    }
                }
                else if (!string.IsNullOrEmpty(userInfo))
                {
                    username = userInfo;
                }

                return new SrsLiveStream()
                {
                    DeviceId = deviceId,
                    IngestName = ingestName,
                    LiveStream = uri.LocalPath,
                    MonitorType = MonitorType.Onvif,
                    VhostDomain = vhostDomain,
                    IpAddress = uriInput.Host,
                    Username = username,
                    Password = password,
                };
            }
            catch
            {
                rs.Code = ErrorNumber.Other;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.Other];
                return null!;
            }
        }

        /// <summary>
        /// Get all ingestBydeviceid
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<Ingest> GetAllIngestByDeviceId(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.SrsManagers == null || Common.SrsManagers.Count == 0)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return null!;
            }

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToLower().Equals(deviceId.Trim().ToLower()));
            if (ret == null)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return null!;
            }

            var ingestList = new List<Ingest>();
            if (ret.Srs.Vhosts == null || ret.Srs.Vhosts.Count == 0) return null!;
            foreach (var vhost in ret.Srs.Vhosts)
            {
                if (vhost != null && vhost.Vingests != null)
                {
                    foreach (var ingest in vhost.Vingests)
                    {
                        if (ingest != null)
                        {
                            ingestList.Add(ingest);
                        }
                    }
                }
            }

            return ingestList;
        }


        /// <summary>
        /// Enable or disable low latency mode for a vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="enable"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool OnOrOffVhostMinDelay(string deviceId, string vhostDomain, bool enable, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.SrsManagers == null || Common.SrsManagers.Count == 0)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return false;
            }

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToLower().Equals(deviceId.Trim().ToLower()));
            if (ret == null)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return false;
            }

            if (ret.Srs.Vhosts == null || ret.Srs.Vhosts.Count == 0)
            {
                rs.Code = ErrorNumber.SrsSubInstanceNotFound;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound];
                return false;
            }

            var retVhost =
                ret.Srs.Vhosts.FindLast(x => x.VhostDomain!.Trim().ToLower().Equals(vhostDomain.Trim().ToLower()));
            if (retVhost == null)
            {
                rs.Code = ErrorNumber.SrsSubInstanceNotFound;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound];
                return false;
            }

            retVhost.Tcp_nodelay = enable;
            retVhost.Min_latency = enable;
            if (enable)
            {
                if (retVhost.Vplay == null)
                {
                    retVhost.Vplay = new Play();
                }

                retVhost.Vplay.Gop_cache = !enable;
                retVhost.Vplay.GopCacheMaxFrames = 2500;
                retVhost.Vplay.Queue_length = 10;
                retVhost.Vplay.Mw_latency = 100;
                if (retVhost.Vpublish == null)
                {
                    retVhost.Vpublish = new Publish();
                }

                retVhost.Vpublish.Mr = !enable;
            }
            else
            {
                if (retVhost.Vplay != null)
                    retVhost.Vplay = null;
                if (retVhost.Vpublish != null)
                    retVhost.Vpublish = null;
            }

            return true;
        }

        /// <summary>
        /// Return monitor information through the value of stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static OnlineClient GetClientInfoByStreamValue(string stream, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
            {
                var ret = OrmService.Db.Select<OnlineClient>()
                    .Where(x => x.ClientType == ClientType.Monitor && x.Stream!.Equals(stream.Trim())).First();
                return ret;
            }
        }

        /// <summary>
        /// Get all running srs information
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<Self_Srs> GetRunningSrsInfoList(out ResponseStruct rs)
        {
            List<Self_Srs> result = null!;
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.SrsManagers == null || Common.SrsManagers.Count == 0)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return null!;
            }

            if (Common.SrsManagers != null && Common.SrsManagers.Count > 0)
            {
                result = new List<Self_Srs>();
                foreach (var sm in Common.SrsManagers)
                {
                    if (sm.IsRunning && sm.Srs.Http_api != null && sm.Srs.Http_api.Enabled == true)
                    {
                        var reqUrl = "http://127.0.0.1:" + sm!.Srs.Http_api!.Listen + "/api/v1/summaries";
                        try
                        {
                            var tmpStr = NetHelperNew.HttpGetRequest(reqUrl, null!);
                            var retReq = JsonHelper.FromJson<SrsSystemInfo>(tmpStr);
                            if (retReq != null && retReq.Data != null && retReq.Data.Self != null)
                            {
                                var filename = Path.GetFileName(retReq.Data.Self.Argv)!;
                                var ext = Path.GetExtension(filename);
                                retReq.Data.Self.Srs_DeviceId = filename.Replace(ext, "");
                                result.Add(retReq.Data.Self);
                            }
                        }
                        catch
                        {
                        }
                    }

                    Thread.Sleep(50);
                }
            }

            return result!;
        }

        /// <summary>
        /// Stop all running srs instances
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<SrsStartStatus> StopAllSrs(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.SrsManagers == null || Common.SrsManagers.Count == 0)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return null!;
            }

            var result = new List<SrsStartStatus>();
            foreach (var sm in Common.SrsManagers)
            {
                if (sm.IsRunning == true)
                {
                    var ret = sm.Stop(out rs);
                    var sts = new SrsStartStatus();
                    sts.DeviceId = sm.SrsDeviceId;
                    sts.IsStarted = !ret;
                    sts.Message = JsonHelper.ToJson(rs);
                    result.Add(sts);
                }

                Thread.Sleep(50);
            }

            return result;
        }

        /// <summary>
        /// Initialize and start all uninitialized or unstarted srs instances
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<SrsStartStatus> InitAndStartAllSrs(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.SrsManagers == null || Common.SrsManagers.Count == 0)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return null!;
            }

            var result = new List<SrsStartStatus>();
            foreach (var sm in Common.SrsManagers)
            {
                if (sm.IsInit == false || sm.IsRunning == false)
                {
                    var ret = sm.SRS_Init(sm.SrsConfigPath, out rs);
                    if (ret)
                    {
                        ret = sm.Start(out rs);
                    }

                    var sts = new SrsStartStatus();
                    sts.DeviceId = sm.SrsDeviceId;
                    sts.IsStarted = ret;
                    sts.Message = JsonHelper.ToJson(rs);
                    result.Add(sts);
                }

                Thread.Sleep(50);
            }

            return result;
        }

        /// <summary>
        /// Kick off a camera or a player by deviceId and clientId
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="clientId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool KickoffClient(string deviceId, string clientId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs != null)
            {
                if (ret.Srs.Http_api != null && ret.Srs.Http_api.Enabled == true)
                {
                    var reqUrl = "http://127.0.0.1:" + ret.Srs.Http_api!.Listen + "/api/v1/clients/" + clientId;
                    try
                    {
                        var tmpStr = NetHelperNew.HttpDeleteRequest(reqUrl, null!);
                        var retReq = JsonHelper.FromJson<SrsSimpleResponseModule>(tmpStr);
                        if (retReq.Code == 0)
                        {
                            return true!;
                        }


                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Obtain Stream status information BySrsDeviceId, and streamId
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="streamId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SrsStreamSingleStatusModule GetStreamStatusByDeviceIdAndStreamId(string deviceId, string streamId,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs != null)
            {
                if (ret.Srs.Http_api != null && ret.Srs.Http_api.Enabled == true)
                {
                    var reqUrl = "http://127.0.0.1:" + ret.Srs.Http_api!.Listen + "/api/v1/streams/" + streamId;
                    try
                    {
                        var tmpStr = NetHelperNew.HttpGetRequest(reqUrl, null!);
                        var retReq = JsonHelper.FromJson<SrsStreamSingleStatusModule>(tmpStr);
                        if (retReq.Code == 0 && retReq.Stream != null)
                        {
                            return retReq!;
                        }

                        return null!;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return null!;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                return null!;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return null!;
        }

        /// <summary>
        /// Get StreamList status information BySrsDeviceId
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SrsStreamsStatusModule GetStreamListStatusByDeviceId(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs != null)
            {
                if (ret.Srs.Http_api != null && ret.Srs.Http_api.Enabled == true)
                {
                    var reqUrl = "http://127.0.0.1:" + ret.Srs.Http_api!.Listen + "/api/v1/streams/";
                    try
                    {
                        var tmpStr = NetHelperNew.HttpGetRequest(reqUrl, null!);
                        var retReq = JsonHelper.FromJson<SrsStreamsStatusModule>(tmpStr);
                        if (retReq.Code == 0 && retReq.Streams != null)
                        {
                            return retReq!;
                        }

                        return null!;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return null!;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                return null!;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return null!;
        }


        /// <summary>
        /// Get Vhost status information BySrsDeviceId, and vhostId
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SrsVhostSingleStatusModule GetVhostStatusByDeviceIdAndVhostId(string deviceId, string vhostId,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs != null)
            {
                if (ret.Srs.Http_api != null && ret.Srs.Http_api.Enabled == true)
                {
                    var reqUrl = "http://127.0.0.1:" + ret.Srs.Http_api!.Listen + "/api/v1/vhosts/" + vhostId;
                    try
                    {
                        var tmpStr = NetHelperNew.HttpGetRequest(reqUrl, null!);
                        var retReq = JsonHelper.FromJson<SrsVhostSingleStatusModule>(tmpStr);
                        if (retReq.Code == 0 && retReq.Vhost != null)
                        {
                            return retReq!;
                        }

                        return null!;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return null!;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                return null!;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return null!;
        }


        /// <summary>
        /// Get VhostList status information BySrsDeviceId
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SrsVhostsStatusModule GetVhostListStatusByDeviceId(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null && ret.Srs != null)
            {
                if (ret.Srs.Http_api != null && ret.Srs.Http_api.Enabled == true)
                {
                    var reqUrl = "http://127.0.0.1:" + ret.Srs.Http_api!.Listen + "/api/v1/vhosts/";
                    try
                    {
                        var tmpStr = NetHelperNew.HttpGetRequest(reqUrl, null!);
                        var retReq = JsonHelper.FromJson<SrsVhostsStatusModule>(tmpStr);
                        if (retReq.Code == 0 && retReq.Vhosts != null)
                        {
                            return retReq!;
                        }

                        return null!;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return null!;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                return null!;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return null!;
        }


        /// <summary>
        /// Get all playing users
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<OnlineClient> GetOnlinePlayerByDeviceId(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
            {
                var result = OrmService.Db.Select<OnlineClient>()
                    .Where(x => x.IsOnline == true && x.ClientType == ClientType.User && x.IsPlay == true &&
                                x.Device_Id!.Equals(deviceId)).ToList();
                return result;
            }
        }

        /// <summary>
        /// Get all playing users
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<OnlineClient> GetOnlinePlayer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
            {
                var result = OrmService.Db.Select<OnlineClient>()
                    .Where(x => x.IsOnline == true && x.ClientType == ClientType.User && x.IsPlay == true).ToList();
                return result;
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

        /// <summary>
        /// Get the ByID of the camera in the release, support multiple ids separated by spaces
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<OnlineClient> GetOnPublishMonitorById(string id, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var strArr = id.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (strArr.Length > 0)
            {
                var lIds = Array.ConvertAll(strArr, long.Parse);
                lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
                {
                    return OrmService.Db.Select<OnlineClient>().Where(x => lIds.Contains(x.Id)).ToList();
                }
            }

            return null!;
        }


        /// <summary>
        /// Get all published cameras
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<OnlineClient> GetOnPublishMonitorList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
            {
                var result = OrmService.Db.Select<OnlineClient>()
                    .Where(x => x.IsOnline == true && x.ClientType == ClientType.Monitor).ToList();
                return result;
            }
        }

        /// <summary>
        /// Obtain the configuration of an ingest through the rtsp address
        /// </summary>
        /// <param name="password"></param>
        /// <param name="rtspUrl"></param>
        /// <param name="rs"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static Ingest GetOnvifMonitorIngestTemplate(string? username, string? password, string rtspUrl,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                if (!rtspUrl.Contains("@"))
                {
                    rtspUrl = rtspUrl.Insert(rtspUrl.IndexOf("://", StringComparison.Ordinal) + 3,
                        username + ":" + password + "@");
                }
            }
            else if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(username))
            {
                if (!rtspUrl.Contains("@"))
                {
                    rtspUrl = rtspUrl.Insert(rtspUrl.IndexOf("://", StringComparison.Ordinal) + 3,
                        username + "@");
                }
            }

            var url = new Uri(rtspUrl);
            var ip = url.Host;
            var port = (ushort) url.Port;
            var protocol = url.Scheme;
            var pathInfo = url.PathAndQuery;
            if (pathInfo.Contains('='))
            {
                var eqflagidx = pathInfo.LastIndexOf('=');
                pathInfo = pathInfo.Substring(eqflagidx + 1);
            }
            else
            {
                var flagidx = pathInfo.LastIndexOf('/');
                pathInfo = pathInfo.Substring(flagidx + 1);
            }

            var result = new Ingest();
            result.IngestName = ip.Trim() + "_" + pathInfo.Trim().ToLower();
            result.Enabled = true;
            result.Input = new IngestInput();
            result.Input.Type = IngestInputType.stream;
            result.Input.Url = rtspUrl;
            result.Ffmpeg = SrsManageCommon.Common.FFmpegBinPath;
            result.Engines = new List<IngestTranscodeEngine>();
            var eng = new IngestTranscodeEngine();
            eng.Enabled = true;
            eng.Perfile = new IngestEnginePerfile();
            eng.Perfile.Re = "re;";
            eng.Perfile.Rtsp_transport = "tcp";
            eng.Vcodec = "copy";
            eng.Acodec = "copy";
            eng.Output = "rtmp://127.0.0.1/live/" + result.IngestName;
            result.Engines.Add(eng);
            return result;
        }
    }
}