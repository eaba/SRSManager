using Akka.Actor;
using SRSManageCommon.ManageStructs;
using SrsManageCommon;
using SrsApis.SrsManager;
using SRSManager.Messages;
using SrsConfFile.SRSConfClass;
using SharpPulsar.User;
using SharpPulsar;
using SrsConfFile;
using Akka.Event;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Crypto.Agreement.Srp;

namespace SRSManager.Actors
{
    internal class SRSManagerActor : ReceiveActor
    {
        private SrsManager _srsManager;
        private PulsarSystem _pulsarSystem;
        private PulsarClient _client;
        private Producer<byte[]> _producer;
        private Consumer<byte[]> _consumer;
        private Reader<byte[]> _reader;
        private readonly ILoggingAdapter _log;
        public SRSManagerActor(PulsarSystem pulsarSystem)
        {
            _log = Context.GetLogger();
            _pulsarSystem = pulsarSystem;
            _srsManager = new SrsManager();
            Receive<CheckNewSrsInstanceListenRight>(c =>
            {
                var list = CheckNewSrsInstanceListenRight(c.Sm, out ResponseStruct rs);
                Sender.Tell(new ApisResult(list, rs));
            });
            Receive<CheckNewSrsInstancePathRight>(c =>
            {
                var path = CheckNewSrsInstancePathRight(c.Sm, out ResponseStruct rs);
                Sender.Tell(new ApisResult(path, rs));
            });
            Receive<GetSrsManager>(_ =>
            {
                Sender.Tell(_srsManager);
            });
            GlobalSrsApis();
            //Pulsar
            SrtServerApis();
            StatsApis();
            StreamCasterApis();
            SystemApis();
            VhostBandcheckApis();
            VhostClusterApis();
            VhostApis();
            VhostDashApis();
            VhostDvrApis();
            VhostExecApis();
            VhostForwardApis();
            VhostHdsApis();
            VhostHlsApis();
            VhostHttpHooksApis();
            VhostHttpRemuxApis();
            VhostHttpStaticApis();
            VhostIngestApis();
            VhostPlayApis();
            VhostPublishApis();
            VhostReferApis();
            VhostRtcApis();        
            VhostSecurityApis();    
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
                if (_srsManager.Srs == null)
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
                if (_srsManager.Srs == null)
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
                if (_srsManager.Srs == null)
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
            Receive<VhostTranscode>(vhIf => vhIf.Method == "GetVhostTranscode", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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
            Receive<VhostTranscode>(vhIf => vhIf.Method == "GetVhostTranscodeNameList", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost!.Vtranscodes == null)
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
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "SetPulsarClient", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Pulsar!.Client == null)
                {
                    _srsManager.Srs.Pulsar.Client = deviceId.Client; 
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
            ReceiveAsync<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "StartPulsarClient", async deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs.Pulsar!.Client == null)
                {
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (_client == null)
                {
                    try
                    {
                        _client = await _pulsarSystem.NewClient(_srsManager.Srs.Pulsar.Client);
                        Sender.Tell(new ApisResult(true, rs));
                    }
                    catch(Exception ex)  
                    {
                        _client = null!;
                        Sender.Tell(new ApisResult(ex, rs)); 
                    }
                    return;
                }
                Sender.Tell(new ApisResult(true, rs));
                return;

            });
            ReceiveAsync<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "StopPulsarClient", async deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs.Pulsar!.Client == null)
                {
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (_client != null)
                {
                    try
                    {
                        await _client.ShutdownAsync();
                        _client = null!;
                        Sender.Tell(new ApisResult(true, rs));
                    }
                    catch(Exception ex)
                    {
                        _client = null!;
                        Sender.Tell(new ApisResult(ex, rs));
                    }
                    return;
                }
                Sender.Tell(new ApisResult(true, rs));
                return;

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "GetPulsarClient", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Pulsar!.Client != null)
                {
                    Sender.Tell(new ApisResult(_srsManager.Srs.Pulsar.Client, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(null!, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "SetPulsarProducer", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Pulsar!.Producer == null)
                {
                    _srsManager.Srs.Pulsar.Producer = deviceId.Producer;
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
            ReceiveAsync<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "PulsarProducer", async deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_client == null)
                {
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (_srsManager.Srs.Pulsar!.Producer != null)
                {
                    try
                    {
                        if (_producer == null)
                            _producer = await _client.NewProducerAsync(_srsManager.Srs.Pulsar.Producer);

                        var receipt = await _producer.SendAsync(deviceId.Data);
                        Sender.Tell(new ApisResult(receipt, rs));
                    }
                    catch (Exception ex)
                    {
                        Sender.Tell(new ApisResult(ex, rs));
                    }                    
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(null!, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "GetPulsarProducer", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Pulsar!.Producer != null)
                {
                    Sender.Tell(new ApisResult(_srsManager.Srs.Pulsar.Producer, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(null!, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "SetPulsarConsumer", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Pulsar!.Consumer == null)
                {
                    _srsManager.Srs.Pulsar.Consumer = deviceId.Consumer;
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
            ReceiveAsync<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "PulsarConsumer", async deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_client == null)
                {
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (_srsManager.Srs.Pulsar!.Consumer != null)
                {
                    try
                    {
                        if (_consumer == null)
                            _consumer = await _client.NewConsumerAsync(_srsManager.Srs.Pulsar.Consumer);

                        var message = await _consumer.ReceiveAsync(TimeSpan.FromMilliseconds(1000));
                        if (message == null) 
                            Sender.Tell(new ApisResult(null!, rs));
                        else
                        {
                            Sender.Tell(new ApisResult(message.Data, rs));
                            await _consumer.AcknowledgeAsync(message);    
                        }
                    }
                    catch (Exception ex)
                    {
                        Sender.Tell(new ApisResult(ex, rs));
                    }
                    return;
                }
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(null!, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "GetPulsarConsumer", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Pulsar!.Consumer != null)
                {
                    Sender.Tell(new ApisResult(_srsManager.Srs.Pulsar.Consumer, rs));
                    return;
                }
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(null!, rs));

            });
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "SetPulsarReader", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Pulsar!.Reader == null)
                {
                    _srsManager.Srs.Pulsar.Reader = deviceId.Reader;
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
            Receive<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "GetPulsarReader", deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager != null && _srsManager.Srs.Pulsar!.Reader != null)
                {
                    Sender.Tell(new ApisResult(_srsManager.Srs.Pulsar.Reader, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(null!, rs));

            });
            ReceiveAsync<GlobalSrs>(deviceIdIf => deviceIdIf.Method == "PulsarReader", async deviceId =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_client == null)
                {
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (_srsManager.Srs.Pulsar!.Reader != null)
                {
                    try
                    {
                        if (_reader == null)
                            _reader = await _client.NewReaderAsync(_srsManager.Srs.Pulsar.Reader);

                        var message = await _reader.ReadNextAsync(TimeSpan.FromMilliseconds(1000));
                        if (message == null)
                            Sender.Tell(new ApisResult(null!, rs));
                        else
                        {
                            Sender.Tell(new ApisResult(message.Data, rs));
                        }
                    }
                    catch (Exception ex)
                    {
                        Sender.Tell(new ApisResult(ex, rs));
                    }
                    return;
                }
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(null!, rs));

            });
        }
        private void VhostSecurityApis()
        {
            Receive<VhostSecurity>(vhIf => vhIf.Method == "SetVhostSecurity", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vsecurity = vh.Security; 
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostSecurity>(vhIf => vhIf.Method == "DeleteVhostSecurity", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vsecurity = null;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostSecurity>(vhIf => vhIf.Method == "GetVhostSecurity", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vsecurity!, rs));
            });
        }

        private void VhostRtcApis()
        {
            Receive<VhostRtc>(vhIf => vhIf.Method == "SetVhostRtc", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Rtc = vh.Rtc;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostRtc>(vhIf => vhIf.Method == "DeleteVhostRtc", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Rtc = null;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostRtc>(vhIf => vhIf.Method == "GetVhostRtc", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Rtc!, rs));
            });
        }
        private void VhostReferApis()
        {
            Receive<VhostRefer>(vhIf => vhIf.Method == "SetVhostRefer", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vrefer = vh.Refer;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostRefer>(vhIf => vhIf.Method == "DeleteVhostRefer", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vrefer = null;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostRefer>(vhIf => vhIf.Method == "GetVhostRefer", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vrefer!, rs));
            });
        }
        private void VhostPublishApis()
        {
            Receive<VhostPublish>(vhIf => vhIf.Method == "SetVhostPublish", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vpublish = vh.Publish;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostPublish>(vhIf => vhIf.Method == "DeleteVhostPublish", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vpublish = null;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostPublish>(vhIf => vhIf.Method == "GetVhostPublish", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vpublish!, rs));
            });
        }
        private void VhostPlayApis()
        {
            Receive<VhostPlay>(vhIf => vhIf.Method == "SetVhostPlay", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vplay = vh.Play;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostPlay>(vhIf => vhIf.Method == "DeleteVhostPlay", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vplay = null;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostPlay>(vhIf => vhIf.Method == "GetVhostPlay", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vplay!, rs));
            });
        }
        private void VhostIngestApis()
        {
            Receive<VhostIngest>(vhIf => vhIf.Method == "OnOrOffIngest", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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
                if (retVhost.Vingests == null)
                {
                    retVhost.Vingests = new List<Ingest>();
                }
                var retVhostIngest = retVhost.Vingests.FindLast(x =>
                    x.IngestName!.Trim().ToUpper().Equals(vh.IngestName!.Trim().ToUpper()));

                if (retVhostIngest != null)
                {
                    retVhostIngest.Enabled = vh.Enable;
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
            Receive<VhostIngest>(vhIf => vhIf.Method == "CreateVhostIngest", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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
                if (retVhost.Vingests == null)
                {
                    retVhost.Vingests = new List<Ingest>();
                }
                var retVhostIngest = retVhost.Vingests.FindLast(x =>
                    x.IngestName!.Trim().ToUpper().Equals(vh.IngestName!.Trim().ToUpper()));

                if (retVhostIngest == null)
                {
                    retVhost.Vingests.Add(vh.Ingest!);
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
            Receive<VhostIngest>(vhIf => vhIf.Method == "SetVhostIngest", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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
                if (retVhost.Vingests == null)
                {
                    retVhost.Vingests = new List<Ingest>();
                }
                var retVhostIngest = retVhost.Vingests.FindLast(x =>
                    x.IngestName!.Trim().ToUpper().Equals(vh.IngestName.Trim().ToUpper()));

                if (retVhostIngest == null)
                {
                    retVhost.Vingests.Add(vh.Ingest!);
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }
                retVhost.Vingests[retVhost.Vingests.IndexOf(retVhostIngest)] = vh.Ingest!;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostIngest>(vhIf => vhIf.Method == "DeleteVhostIngestByIngestInstanceName", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null || retVhost.Vingests == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }

                var retVhostIngest = retVhost.Vingests.FindLast(x =>
                    x.IngestName!.Trim().ToUpper().Equals(vh.IngestName.Trim().ToUpper()));
                if (retVhostIngest == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                 
                var remove = retVhost.Vingests.Remove(retVhostIngest);
                Sender.Tell(new ApisResult(remove, rs));

            });
            Receive<VhostIngest>(vhIf => vhIf.Method == "GetVhostIngest", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null || retVhost.Vingests == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }

                var retVhostIngest = retVhost.Vingests.FindLast(x =>
                    x.IngestName!.Trim().ToUpper().Equals(vh.IngestName.Trim().ToUpper()));

                if (retVhostIngest == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                
                Sender.Tell(new ApisResult(retVhostIngest, rs));

            });
            Receive<VhostIngest>(vhIf => vhIf.Method == "GetVhostIngestList", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vingests!, rs));
            });
            Receive<VhostIngest>(vhIf => vhIf.Method == "GetVhostIngestNameList", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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
                var result = new List<VhostIngestNameModule>();
                if (string.IsNullOrEmpty(vh.VHostDomain))
                {
                    foreach (var vhost in _srsManager.Srs.Vhosts)
                    {
                        if (vhost.Vingests != null && vhost.Vingests.Count > 0)
                        {
                            foreach (var ingest in vhost.Vingests)
                            {
                                var vn = new VhostIngestNameModule();
                                vn.VhostDomain = vhost.VhostDomain;
                                vn.IngestInstanceName = ingest.IngestName;
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

                foreach (var ingest in retVhost.Vingests)
                {
                    var vn = new VhostIngestNameModule();
                    vn.VhostDomain = retVhost.VhostDomain;
                    vn.IngestInstanceName = ingest.IngestName;
                    result.Add(vn);
                }

                Sender.Tell(new ApisResult(result, rs));
            });
        }

        private void VhostHttpStaticApis() 
        {
            Receive<VhostHttpStatic>(vhIf => vhIf.Method == "SetVhostHttpStatic", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vhttp_static = vh.HttpStatic;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostHttpStatic>(vhIf => vhIf.Method == "DeleteVhostHttpStatic", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vhttp_static = null;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostHttpStatic>(vhIf => vhIf.Method == "GetVhostHttpStatic", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vhttp_static!, rs));
            });
        }
        private void VhostHttpRemuxApis()
        {
            Receive<VhostHttpRemux>(vhIf => vhIf.Method == "SetVhostHttpRemux", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vhttp_remux = vh.HttpRemux;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostHttpRemux>(vhIf => vhIf.Method == "DeleteVhostHttpRemux", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vhttp_remux = null;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostHttpRemux>(vhIf => vhIf.Method == "GetVhostHttpRemux", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vhttp_remux!, rs));
            });
        }
        private void VhostHttpHooksApis()
        {
            Receive<VhostHttpHooks>(vhIf => vhIf.Method == "SetVhostHttpHooks", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vhttp_hooks = vh.HttpHooks;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostHttpHooks>(vhIf => vhIf.Method == "DeleteVhostHttpHooks", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vhttp_hooks = null;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostHttpHooks>(vhIf => vhIf.Method == "GetVhostHttpHooks", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vhttp_hooks!, rs));
            });
        }
        private void VhostHlsApis()
        {
            Receive<VhostHls>(vhIf => vhIf.Method == "SetVhostHls", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vhls = vh.HostHls;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostHls>(vhIf => vhIf.Method == "DeleteVhostHls", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vhls = null;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostHls>(vhIf => vhIf.Method == "GetVhostHls", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vhls!, rs));
            });
        }
        private void VhostHdsApis()
        {
            Receive<VhostHds>(vhIf => vhIf.Method == "SetVhostHds", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vhds = vh.Hds;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostHds>(vhIf => vhIf.Method == "DeleteVhostHds", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vhds = null;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostHds>(vhIf => vhIf.Method == "GetVhostHds", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vhds!, rs));
            });
        }
        private void VhostForwardApis()
        {
            Receive<VhostForward>(vhIf => vhIf.Method == "SetVhostForward", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vforward = vh.Forward;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostForward>(vhIf => vhIf.Method == "DeleteVhostForward", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vforward = null;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostForward>(vhIf => vhIf.Method == "GetVhostForward", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vforward!, rs));
            });
        }
        private void VhostExecApis()
        {
            Receive<VhostExec>(vhIf => vhIf.Method == "SetVhostExec", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vexec = vh.Exec;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostExec>(vhIf => vhIf.Method == "GetVhostExec", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vexec!, rs));
            });
            Receive<VhostExec>(vhIf => vhIf.Method == "DeleteVhostExec", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vexec = null;
                Sender.Tell(new ApisResult(true, rs));

            });
        }
        private void VhostDvrApis()
        {
            Receive<VhostDvr>(vhIf => vhIf.Method == "SetVhostDvr", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vdvr = vh.Dvr;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostDvr>(vhIf => vhIf.Method == "GetVhostDvr", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vdvr!, rs));
            });
            Receive<VhostDvr>(vhIf => vhIf.Method == "DeleteVhostDvr", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vdvr = null;
                Sender.Tell(new ApisResult(true, rs));

            });
        }
        private void VhostDashApis()
        {
            Receive<VhostDash>(vhIf => vhIf.Method == "SetVhostDash", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vdash = vh.Dash;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostDash>(vhIf => vhIf.Method == "GetVhostDash", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vdash!, rs));
            });
            Receive<VhostDash>(vhIf => vhIf.Method == "DeleteVhostDash", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vdash = null;
                Sender.Tell(new ApisResult(true, rs));

            });
        }
        private void VhostApis()
        {
            Receive<Vhost>(vhIf => vhIf.Method == "GetVhostsInstanceName", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                Sender.Tell(new ApisResult(_srsManager.Srs.Vhosts.Select(x => x.InstanceName).ToList()!, rs));
                return;

            });
            Receive<Vhost>(vhIf => vhIf.Method == "GetVhostByDomain", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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
                    x.VhostDomain!.Trim().ToUpper().Equals(vh.VHostDomain.Trim().ToUpper()))!;
                if (retVhost != null)
                {
                    Sender.Tell(new ApisResult(retVhost, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                Sender.Tell(new ApisResult(null!, rs));
                return;

            });
            Receive<Vhost>(vhIf => vhIf.Method == "GetVhostList", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                Sender.Tell(new ApisResult(_srsManager.Srs.Vhosts!, rs));
                return;

            });
            Receive<Vhost>(vhIf => vhIf.Method == "GetVhostTemplate", vh =>
            {
                var vhost = new SrsvHostConfClass();
                vhost.Vingests = new List<Ingest>();
                vhost.InstanceName = "your.domain.com"; //change it 
                vhost.VhostDomain = vhost.InstanceName;
                vhost.SectionsName = "vhost";
                vhost.Enabled = false;
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None] + "\r\n" + JsonHelper.ToJson(vh.VType),
                };
                switch (vh.VType)
                {
                    case VhostIngestInputType.WebCast://add for live
                        var dvr = new Dvr();
                        dvr.Dvr_apply = "all";
                        dvr.Enabled = true;
                        dvr.Dvr_plan = "segment";
                        dvr.Dvr_path = "please replace";
                        dvr.Dvr_duration = 120;
                        dvr.Dvr_wait_keyframe = true;
                        dvr.Time_Jitter = PlayTimeJitter.full;
                        var httpHooks = new HttpHooks();
                        httpHooks.Enabled = true;
                        //httpHooks.On_connect = "http://127.0.0.1:5800/SrsHooks/OnConnect";
                        httpHooks.On_publish = "http://127.0.0.1:5800/SrsHooks/OnPublish";
                        //httpHooks.On_close = "http://127.0.0.1:5800/SrsHooks/OnClose";
                        httpHooks.On_play = "http://127.0.0.1:5800/SrsHooks/OnPlay";
                        httpHooks.On_unpublish = "http://127.0.0.1:5800/SrsHooks/OnUnPublish";
                        httpHooks.On_stop = "http://127.0.0.1:5800/SrsHooks/OnStop";
                        httpHooks.On_dvr = "http://127.0.0.1:5800/SrsHooks/OnDvr";
                        httpHooks.On_hls = "http://127.0.0.1:5800/SrsHooks/Test";
                        httpHooks.On_hls_notify = "http://127.0.0.1:5800/SrsHooks/Test";
                        var httpRemux = new HttpRemux();
                        httpRemux.Enabled = true;
                        httpRemux.Fast_cache = 30;
                        httpRemux.Mount = "[vhost]/[app]/[stream].flv";
                        vhost.Vdvr = dvr;
                        vhost.Vhttp_hooks = httpHooks;
                        vhost.Vhttp_remux = httpRemux;
                        Sender.Tell(new ApisResult(vhost!, rs));
                        break;
                    case VhostIngestInputType.Stream: //RTSP or other Stream
                        var ing = new Ingest();
                        ing.Enabled = true;
                        ing.InstanceName = "ingest_template_rtsp";
                        ing.IngestName = ing.InstanceName;
                        ing.SectionsName = "ingest";
                        var inginput = new IngestInput();
                        var ingeng = new IngestTranscodeEngine();
                        ing.Engines = new List<IngestTranscodeEngine>();
                        ingeng.Perfile = new IngestEnginePerfile();
                        ingeng.Perfile.SectionsName = "perfile";
                        ingeng.Enabled = true;
                        inginput.SectionsName = "input";
                        inginput.Type = IngestInputType.stream;
                        inginput.Url =
                            "rtsp://admin:12345678@192.168.2.21:554/Streaming/Channels/501?transportmode=unicast";
                        ing.Input = inginput;
                        ing.Ffmpeg = SrsManageCommon.Common.FFmpegBinPath;
                        ingeng.SectionsName = "engine";
                        ingeng.Enabled = true;
                        ingeng.Vcodec = "copy";
                        ingeng.Acodec = "copy";
                        ingeng.Perfile.Rtsp_transport = "tcp";
                        ingeng.Perfile.SectionsName = "perfile";
                        ingeng.Output = "rtmp://127.0.0.1/[vhost]/[live]/[livestream]";
                        ing.Engines.Add(ingeng);
                        ing.Input = inginput;
                        vhost.Vingests.Add(ing);
                        Sender.Tell(new ApisResult(vhost!, rs));
                        break;
                    case VhostIngestInputType.File: //From File
                        var inginput1 = new IngestInput();
                        var ingeng1 = new IngestTranscodeEngine();
                        var ing1 = new Ingest();
                        ing1.Enabled = true;
                        ing1.Engines = new List<IngestTranscodeEngine>();
                        ing1.InstanceName = "ingest_template_file";
                        ing1.IngestName = ing1.InstanceName;
                        ing1.SectionsName = "ingest";
                        ingeng1.Enabled = true;
                        inginput1.SectionsName = "input";
                        inginput1.Type = IngestInputType.file;
                        inginput1.Url =
                            "./demo.mp4";
                        ing1.Input = inginput1;
                        ing1.Ffmpeg = SrsManageCommon.Common.FFmpegBinPath;
                        ingeng1.SectionsName = "engine";
                        ingeng1.Enabled = true;
                        ingeng1.Output = "rtmp://127.0.0.1/[vhost]/[live]/[livestream]";
                        ing1.Engines.Add(ingeng1);
                        ing1.Input = inginput1;
                        vhost.Vingests.Add(ing1);
                        Sender.Tell(new ApisResult(vhost!, rs));
                        break;
                    case VhostIngestInputType.Device: //From Device
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.SrsConfigFunctionUnsupported,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.SrsConfigFunctionUnsupported] + "\r\n" +
                                      JsonHelper.ToJson(vh.VType),
                        };
                        Sender.Tell(new ApisResult(null!, rs));
                        break;
                    default:
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.FunctionInputParamsError,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError] + "\r\n" +
                                      JsonHelper.ToJson(vh.VType),
                        };
                        Sender.Tell(new ApisResult(null!, rs));
                        break;
                }

            });
            Receive<Vhost>(vhIf => vhIf.Method == "SetVhost", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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
                    _srsManager.Srs.Vhosts = new List<SrsvHostConfClass>
                    {
                        vh.VHost!
                    };
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                var retVhost = _srsManager.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vh.VHostDomain.Trim().ToUpper()))!;
                if (retVhost != null)
                {
                    _srsManager.Srs.Vhosts.Add(vh.VHost!);   
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                _srsManager.Srs.Vhosts[_srsManager.Srs.Vhosts.IndexOf(retVhost!)] = vh.VHost;
                Sender.Tell(new ApisResult(true, rs));
                return;

            });
            Receive<Vhost>(vhIf => vhIf.Method == "DeleteVhostByDomain", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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
                    x.VhostDomain!.Trim().ToUpper().Equals(vh.VHostDomain.Trim().ToUpper()))!;
                if (retVhost != null)
                {
                    Sender.Tell(new ApisResult(_srsManager.Srs.Vhosts.Remove(retVhost), rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                Sender.Tell(new ApisResult(false, rs));
                return;

            });
            Receive<Vhost>(vhIf => vhIf.Method == "ChangeVhostDomain", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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
                    x.VhostDomain!.Trim().ToUpper().Equals(vh.VHostDomain.Trim().ToUpper()))!;

                var retVhostNew = _srsManager.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vh.NewVhostDomain.Trim().ToUpper()))!;

                if (retVhost != null)
                {
                    if (retVhostNew != null)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.SrsSubInstanceAlreadyExists,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceAlreadyExists],
                        };
                        Sender.Tell(new ApisResult(false, rs)); 
                        return;
                    }

                    retVhost.VhostDomain = vh.NewVhostDomain;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }
                
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                Sender.Tell(new ApisResult(false, rs));
                return;

            });
            
        }
        private void VhostClusterApis()
        {
            Receive<VhostCluster>(vhIf => vhIf.Method == "SetVhostCluster", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vcluster = vh.Cluster;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostCluster>(vhIf => vhIf.Method == "GetVhostCluster", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vcluster!, rs));
            });
            Receive<VhostCluster>(vhIf => vhIf.Method == "DeleteVhostCluster", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vcluster = null;
                Sender.Tell(new ApisResult(true, rs));

            });
        }
        private void VhostBandcheckApis()
        {
            Receive<VhostBandcheck>(vhIf => vhIf.Method == "SetVhostBandcheck", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vbandcheck = vh.Bandcheck;
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<VhostBandcheck>(vhIf => vhIf.Method == "GetVhostBandcheck", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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

                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                Sender.Tell(new ApisResult(retVhost.Vbandcheck!, rs));
            });
            Receive<VhostBandcheck>(vhIf => vhIf.Method == "DeleteVhostBandcheck", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
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


                retVhost.Vbandcheck = null;
                Sender.Tell(new ApisResult(true, rs));

            });
        }
        private void SystemApis()
        {
            Receive<Messages.System>(vhIf => vhIf.Method == "RefreshSrsObject", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                SrsConfigBuild.Build(_srsManager.Srs, _srsManager.SrsConfigPath);
                _log.Info($"Rewrite the Srs configuration file to refresh the Srs instance...{_srsManager.Srs.ConfFilePath!}");
                var bl = _srsManager.Reload(out rs);
                Sender.Tell(new ApisResult(bl, rs));

            });
            Receive<Messages.System>(vhIf => vhIf.Method == "DelSrsInstanceByDeviceId", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };

                var ret = _srsManager;
                if (ret != null)
                {
                    if (ret.Srs != null && (ret.IsRunning || ret.IsInit))
                    {
                        //Stop the srs process
                        while (ret.IsRunning)
                        {
                            ret.Stop(out rs);
                            Thread.Sleep(100);
                        }
                    }

                    File.Delete(ret.SrsConfigPath);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.None,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                    };
                    Sender.Tell(new ApisResult(true, rs));
                    Self.GracefulStop(TimeSpan.FromMilliseconds(1000));
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                }
            });
            Receive<Messages.System>(vhIf => vhIf.Method == "GetSrsInstanceByDeviceId", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
               
                Sender.Tell(new ApisResult(_srsManager, rs));
            });
            Receive<Messages.System>(vhIf => vhIf.Method == "GetSrsManagerInstanceByDeviceId", vh =>
            {
               
                Sender.Tell(_srsManager);

            });
        }

        private void StreamCasterApis()
        {
            Receive<StreamCaster>(vhIf => vhIf.Method == "SetStreamCaster", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs != null && _srsManager.Srs.Stream_casters != null)
                {
                    var retStreamCaster = _srsManager.Srs.Stream_casters.FindLast(x =>
                        x.InstanceName!.Trim().ToUpper().Equals(vh.Streamcaster!.InstanceName!.Trim().ToUpper()));
                    if (retStreamCaster != null)
                    {
                        if (retStreamCaster != null) //Revise
                        {
                            _srsManager.Srs.Stream_casters[_srsManager.Srs.Stream_casters.IndexOf(retStreamCaster)] = vh.Streamcaster!;
                            Sender.Tell(new ApisResult(true, rs));
                            return;
                        }

                        _srsManager.Srs.Stream_casters.Add(vh.Streamcaster!);
                        Sender.Tell(new ApisResult(true, rs));
                        return;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                Sender.Tell(new ApisResult(false, rs));
                
            });
            Receive<StreamCaster>(vhIf => vhIf.Method == "DeleteStreamCasterByInstanceName", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (_srsManager.Srs.Stream_casters == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }

                var retStreamCaster = _srsManager.Srs.Stream_casters.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(vh.InstanceName.Trim().ToUpper()));

                if (retStreamCaster == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }


                _srsManager.Srs.Stream_casters.Remove(retStreamCaster);
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<StreamCaster>(vhIf => vhIf.Method == "GetStreamCasterInstanceNameList", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                if (_srsManager.Srs.Stream_casters == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }

                var slist = _srsManager.Srs.Stream_casters.Select(i => i.InstanceName).ToList()!;
                Sender.Tell(new ApisResult(slist, rs));
            });
            Receive<StreamCaster>(vhIf => vhIf.Method == "GetStreamCasterInstanceList", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }
                if (_srsManager.Srs.Stream_casters == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(null!, rs));
                    return;
                }

                Sender.Tell(new ApisResult(_srsManager.Srs.Stream_casters!, rs));
            });
            Receive<StreamCaster>(vhIf => vhIf.Method == "CreateStreamCaster", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }

                if (_srsManager.Srs.Stream_casters == null)
                {
                    _srsManager.Srs.Stream_casters = new List<SrsStreamCasterConfClass>
                    {
                        vh.Streamcaster!
                    };
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                var retStreamCaster = _srsManager.Srs.Stream_casters.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(vh.Streamcaster!.InstanceName!.Trim().ToUpper()));

                if (retStreamCaster == null)
                {
                    _srsManager.Srs.Stream_casters.Add(vh.Streamcaster!);
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
            Receive<StreamCaster>(vhIf => vhIf.Method == "ChangeStreamCasterInstanceName", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (_srsManager.Srs.Stream_casters == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }

                var retStreamCaster = _srsManager.Srs.Stream_casters.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(vh.InstanceName!.Trim().ToUpper()));
                if (retStreamCaster == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }
                var retStreamCasterNew = _srsManager.Srs.Stream_casters.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(vh.NewInstanceName!.Trim().ToUpper()));
                if (retStreamCasterNew != null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceAlreadyExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceAlreadyExists],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }

                retStreamCaster.InstanceName = vh.NewInstanceName;
                
                Sender.Tell(new ApisResult(true, rs));

            });
            Receive<StreamCaster>(vhIf => vhIf.Method == "OnOrOffStreamCaster", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs != null && _srsManager.Srs.Stream_casters != null)
                {
                    var retStreamCaster = _srsManager.Srs.Stream_casters.FindLast(x =>
                        x.InstanceName!.Trim().ToUpper().Equals(vh.InstanceName!.Trim().ToUpper()));
                    if (retStreamCaster != null)
                    {
                        retStreamCaster.Enabled = vh.Enable;
                        if (retStreamCaster.sip != null)
                        {
                            retStreamCaster.sip.Enabled = vh.Enable;
                        }

                        Sender.Tell(new ApisResult(true, rs));
                        return;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    Sender.Tell(new ApisResult(false, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                Sender.Tell(new ApisResult(false, rs));
            });
        }
        private void StatsApis()
        {
            Receive<Stats>(vhIf => vhIf.Method == "SetSrsStats", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs != null)
                {
                    _srsManager.Srs.Stats = vh.Stat;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

                return;
                
            });
            Receive<Stats>(vhIf => vhIf.Method == "GetSrsStats", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs != null)
                {
                    Sender.Tell(new ApisResult(_srsManager.Srs.Stats!, rs));
                    return;
                }
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(null!, rs));
            });
            Receive<Stats>(vhIf => vhIf.Method == "DelStats", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs != null)
                {
                    _srsManager.Srs.Stats = null;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

                return;

            });
        }
        private void SrtServerApis()
        {
            Receive<SrtServer>(vhIf => vhIf.Method == "SetSrtServer", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs != null)
                {
                    _srsManager.Srs.Srt_server = vh.Srt;
                    Sender.Tell(new ApisResult(true, rs));
                    return;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(false, rs));

                return;

            });
            Receive<SrtServer>(vhIf => vhIf.Method == "GetSrtServer", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs != null)
                {
                    Sender.Tell(new ApisResult(_srsManager.Srs.Srt_server!, rs));
                    return;
                }
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                Sender.Tell(new ApisResult(null!, rs));
            });
            Receive<SrtServer>(vhIf => vhIf.Method == "DelSrtServer", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs != null)
                {
                    _srsManager.Srs.Srt_server = null;
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
            Receive<SrtServer>(vhIf => vhIf.Method == "OnOrOffSrtServer", vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                if (_srsManager.Srs != null /*&& ret.Srs.Rtc_server != null*/)
                {
                    _srsManager.Srs.Srt_server!.Enabled = vh.Enable;
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
        private bool CheckNewSrsInstancePathRight(SrsManager sm, out ResponseStruct rs)
        {
            if (sm == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return false;
            }

            var devId = sm.SrsDeviceId;
            var confPath = sm.SrsConfigPath;
            if (string.IsNullOrEmpty(devId) || string.IsNullOrEmpty(confPath))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return false;
            }

            var ret = _srsManager.SrsDeviceId.Trim().ToUpper().Equals(devId.Trim().ToUpper());
            if (ret)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsInstanceExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceExists],
                };
                return false;
            }

            ret = _srsManager.SrsConfigPath.Trim().ToUpper().Equals(confPath.Trim().ToUpper());
            if (ret)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsInstanceConfigPathExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceConfigPathExists],
                };
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            return true;
        }
        private bool CheckNewSrsInstanceListenRight(SrsManager sm, out ResponseStruct rs)
        {
            if (sm == null || sm.Srs == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return false;
            }

            var port = sm.Srs.Listen;
            if (port == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return false;
            }

            if (_srsManager.Srs.Listen != port)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsInstanceListenExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceListenExists],
                };
                return false;
            }

            if (sm.Srs.Http_api != null && sm.Srs.Http_api.Listen != null)
            {
                port = sm.Srs.Http_api.Listen;
                if (_srsManager.Srs.Http_api!.Listen != port)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsInstanceHttpApiListenExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceHttpApiListenExists],
                    };
                    return false;
                }
            }

            if (sm.Srs.Http_server != null && sm.Srs.Http_server.Listen != null)
            {
                port = sm.Srs.Http_server.Listen;
                if (_srsManager.Srs.Http_server!.Listen != port)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsInstanceHttpServerListenExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceHttpServerListenExists],
                    };
                    return false;
                }
            }

            if (sm.Srs.Rtc_server != null && sm.Srs.Rtc_server.Listen != null)
            {
                port = sm.Srs.Rtc_server.Listen;
                if (_srsManager.Srs.Rtc_server!.Listen != port)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsInstanceRtcServerListenExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceRtcServerListenExists],
                    };
                    return false;
                }
            }

            if (sm.Srs.Srt_server != null && sm.Srs.Srt_server.Listen != null)
            {
                port = sm.Srs.Srt_server.Listen;
                if (_srsManager.Srs.Srt_server!.Listen != port)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsInstanceSrtServerListenExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceSrtServerListenExists],
                    };
                    return false;
                }
            }

            if (sm.Srs.Stream_casters != null && sm.Srs.Stream_casters.Count > 0)
            {
                foreach (var caster in sm.Srs.Stream_casters)
                {
                    if (caster != null && caster.Listen != null)
                    {
                        foreach (var sc in _srsManager.Srs.Stream_casters!)
                        {
                            if (sc != null)
                            {
                                if (sc.Listen == caster.Listen)
                                {
                                    rs = new ResponseStruct()
                                    {
                                        Code = ErrorNumber.SrsInstanceStreamCasterListenExists,
                                        Message = ErrorMessage.ErrorDic![
                                            ErrorNumber.SrsInstanceStreamCasterListenExists],
                                    };
                                    return false;
                                }

                                if (caster.sip != null && sc.sip != null && caster.sip.Listen != null &&
                                    sc.sip.Listen != null)
                                {
                                    if (caster.sip.Listen == sc.sip.Listen)
                                    {
                                        rs = new ResponseStruct()
                                        {
                                            Code = ErrorNumber.SrsInstanceStreamCasterSipListenExists,
                                            Message = ErrorMessage.ErrorDic![
                                                ErrorNumber.SrsInstanceStreamCasterSipListenExists],
                                        };
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            return true;
        }
        public static Props Prop(PulsarSystem pulsarSystem)
        {
            return Props.Create(() => new SRSManagerActor(pulsarSystem));
        }
    }
}
