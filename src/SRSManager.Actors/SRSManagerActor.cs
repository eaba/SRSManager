using Akka.Actor;
using SRSManageCommon.ManageStructs;
using SrsManageCommon;
using SrsApis.SrsManager;
using SRSManager.Messages;

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
                        Sender.Tell(new DelApisResult(true, rs));
                        return;
                    }
                    var rt = _srsManager.Start(out rs);
                    Sender.Tell(new DelApisResult(rt, rs));
                    return;
                }
               
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(rt, rs));
                    return;
                }
                
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(rt, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(rt, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(rt, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(rt, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));
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
                    Sender.Tell(new DelApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

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
                    Sender.Tell(new DelApisResult(result, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(null!, rs));

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
                    Sender.Tell(new DelApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new DelApisResult(false, rs));

            });
        }
        public static Props Prop()
        {
            return Props.Create(() => new SRSManagerActor());
        }
    }
}
