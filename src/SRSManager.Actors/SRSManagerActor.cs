using Akka.Actor;
using SRSManageCommon.ManageStructs;
using SrsManageCommon;
using SrsApis.SrsManager;
using SRSManager.Messages;
using SrsConfFile.SRSConfClass;

namespace SRSManager.Actors
{
    internal class SRSManagerActor : ReceiveActor
    {
        private SrsManager _srsManager;

        public SRSManagerActor()
        {
            _srsManager = new SrsManager();
            GlobalSrsApis();
            //Pulsar
            VhostTranscodeApis();
        }

        private void VhostTranscodeApis()
        {
            Receive<VhostTranscode>(vhIf => vhIf.Method == "CreateVhostTranscode", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (_srsManager.Srs.Vhosts == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }

                var retVhost = _srsManager.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vh.VHostDomain.Trim().ToUpper()));

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (retVhost.Vtranscodes == null)
                {
                    retVhost.Vtranscodes = new List<Transcode>();
                }
                var retVhostTranscode = retVhost.Vtranscodes.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(vh.TranscodeInstanceName.Trim().ToUpper()));

                if (retVhostTranscode == null)
                {
                    retVhost.Vtranscodes.Add(vh.Transcode!);
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceAlreadyExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceAlreadyExists],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<VhostTranscode>(vhIf => vhIf.Method == "SetVhostTranscode", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (_srsManager.Srs.Vhosts == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }

                var retVhost = _srsManager.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vh.VHostDomain.Trim().ToUpper()));

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (retVhost.Vtranscodes == null)
                {
                    retVhost.Vtranscodes = new List<Transcode>();
                }
                var retVhostTranscode = retVhost.Vtranscodes.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(vh.TranscodeInstanceName.Trim().ToUpper()));

                if (retVhostTranscode == null)
                {
                    retVhost.Vtranscodes.Add(vh.Transcode!);
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }
                retVhost.Vtranscodes[retVhost.Vtranscodes.IndexOf(retVhostTranscode)] = vh.Transcode!;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostTranscode>(vhIf => vhIf.Method == "DeleteVhostTranscodeByTranscodeInstanceName", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (_srsManager.Srs.Vhosts == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }

                var retVhost = _srsManager.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vh.VHostDomain.Trim().ToUpper()));

                if (retVhost == null || retVhost.Vtranscodes == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }

                var retVhostTranscode = retVhost.Vtranscodes.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(vh.TranscodeInstanceName.Trim().ToUpper()));
                
                if (retVhostTranscode == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }

                var remove = retVhost.Vtranscodes.Remove(retVhostTranscode);
                Sender.Tell(new ApisResult(remove, rs));

            });
            Receive<VhostTranscode>(vhIf => vhIf.Method == "GetVhostTranscodeNameList", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                if (_srsManager.Srs.Vhosts == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                var result = new List<VhostTranscodeNameModule>();
                if (string.IsNullOrEmpty(vh.VHostDomain))
                {
                    foreach (var vhost in _srsManager.Srs.Vhosts)
                    {
                        if (vhost.Vtranscodes != null && vhost.Vtranscodes.Count > 0)
                        {
                            foreach (var code in vhost.Vtranscodes)
                            {
                                var vn = new VhostTranscodeNameModule();
                                vn.VhostDomain = vhost.VhostDomain;
                                vn.TranscodeInstanceName = code.InstanceName;
                                result.Add(vn);
                            }
                        }
                    }

                    Sender.Tell(new ApisResult(result, rs));
                    return;
                }
                var retVhost = _srsManager.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vh.VHostDomain.Trim().ToUpper()));

                if (retVhost!.Vingests == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }

                foreach (var code in retVhost.Vtranscodes!)
                {
                    var vn = new VhostTranscodeNameModule();
                    vn.VhostDomain = retVhost.VhostDomain;
                    vn.TranscodeInstanceName = code.InstanceName;
                    result.Add(vn);
                }

                Sender.Tell(new ApisResult(result, rs));

            });
            Receive<VhostTranscode>(vhIf => vhIf.Method == "GetVhostTranscode", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                if (_srsManager.Srs.Vhosts == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }

                var retVhost = _srsManager.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vh.VHostDomain.Trim().ToUpper()));

                if (retVhost == null || retVhost.Vtranscodes == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }

                var retVhostTranscode = retVhost.Vtranscodes.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(vh.TranscodeInstanceName.Trim().ToUpper()));

                if (retVhostTranscode == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }

                Sender.Tell(new ApisResult(retVhostTranscode, rs));

            });
        }
        private void GlobalSrsApis()
        {
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "Start", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null)
                {
                    if (_srsManager.IsRunning)
                    {
                        Sender.Tell(new ApisResult(true, rs));
                        return;
                    }
                    var rt = _srsManager.Start(out rs);
                    Sender.Tell(new ApisResult(rt, rs));
                    return;
                }
               
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "Stop", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null)
                {
                    var rt = _srsManager.Stop(out rs);
                    Sender.Tell(new ApisResult(rt, rs));
                    return;
                }
                
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "Restart", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null)
                {
                    var rt = _srsManager.Restart(out rs);
                    Sender.Tell(new ApisResult(rt, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "Reload", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null)
                {
                    var rt = _srsManager.Reload(out rs);
                    Sender.Tell(new ApisResult(rt, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "IsRunning", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null)
                {
                    var rt = _srsManager.IsRunning;
                    Sender.Tell(new ApisResult(rt, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "IsInit", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null)
                {
                    var rt = _srsManager.IsInit;
                    Sender.Tell(new ApisResult(rt, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "ChangeChunksize", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs != null)
                {
                    _srsManager.Srs.Chunk_size = deviceId.Short; 
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));
            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "ChangeHttpApiListen", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Http_api != null)
                {
                    _srsManager.Srs.Http_api.Listen = deviceId.Short;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "ChangeHttpApiEnable", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Http_api != null)
                {
                    _srsManager.Srs.Http_api.Enabled = deviceId.Enable;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "ChangeMaxConnections", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs != null)
                {
                    _srsManager.Srs.Max_connections = deviceId.Short;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "ChangeRtmpListen", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs != null)
                {
                    _srsManager.Srs.Listen = deviceId.Short;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "ChangeHttpServerListen", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Http_server != null)
                {
                    _srsManager.Srs.Http_server.Listen = deviceId.Short;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "ChangeHttpServerPath", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Http_server != null)
                {
                    _srsManager.Srs.Http_server.Dir = deviceId.Path;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "ChangeHttpServerEnable", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Http_server != null)
                {
                    _srsManager.Srs.Http_server.Enabled = deviceId.Enable;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "GetGlobalParams", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Http_server != null)
                {
                    var result = new GlobalModule()
                    {
                        Listen = _srsManager.Srs.Listen,
                        HttpApiListen = _srsManager.Srs.Http_api!.Listen,
                        MaxConnections = _srsManager.Srs.Max_connections,
                        HttpApiEnable = _srsManager.Srs.Http_api!.Enabled,
                        HttpServerEnable = _srsManager.Srs.Http_server!.Enabled,
                        HttpServerPath = _srsManager.Srs.Http_server!.Dir,
                        HttpServerListen = _srsManager.Srs.Http_server!.Listen,
                        HeartbeatEnable = _srsManager.Srs.Heartbeat!.Enabled,
                        HeartbeatUrl = _srsManager.Srs.Heartbeat!.Url,
                        HeartbeatSummariesEnable = _srsManager.Srs.Heartbeat!.Summaries,
                    };
                    Sender.Tell(new ApisResult(result, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(null!, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "ChangeGlobalParams", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Http_server != null)
                {
                    var bm = deviceId.Gm;
                    if (bm?.Listen != null) _srsManager.Srs.Listen = bm.Listen;
                    if (bm?.MaxConnections != null) _srsManager.Srs.Max_connections = bm.MaxConnections;
                    if (bm?.HttpApiEnable != null) _srsManager.Srs.Http_api!.Enabled = bm.HttpApiEnable;
                    if (bm?.HttpApiListen != null) _srsManager.Srs.Http_api!.Listen = bm.HttpApiListen;
                    if (bm?.HttpServerPath != null) _srsManager.Srs.Http_server!.Dir = bm.HttpServerPath;
                    if (bm?.HttpServerEnable != null) _srsManager.Srs.Http_server!.Enabled = bm.HttpServerEnable;
                    if (bm?.HttpServerListen != null) _srsManager.Srs.Http_server!.Listen = bm.HttpServerListen;
                    if (bm?.HeartbeatEnable != null) _srsManager.Srs.Heartbeat!.Enabled = bm.HeartbeatEnable;
                    if (bm?.HeartbeatUrl != null) _srsManager.Srs.Heartbeat!.Url = bm.HeartbeatUrl;
                    if (bm?.HeartbeatSummariesEnable != null) _srsManager.Srs.Heartbeat!.Summaries = bm.HeartbeatSummariesEnable;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

            });
        }
        public static Props Prop()
        {
            return Props.Create(() => new SRSManagerActor());
        }
    }
}
