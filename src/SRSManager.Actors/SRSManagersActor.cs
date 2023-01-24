using System.Runtime.InteropServices;
using Akka.Actor;
using SharpPulsar;
using SrsManageCommon.SrsManageCommon;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using SRSManager.Messages;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using SrsApis.SrsManager;
using SrsConfFile.SRSConfClass;
using Akka.Event;
using SharpPulsar.Builder;

namespace SRSManager.Actors
{
    //t
    //dynamic_highlatency.m3u8?type=live
    //             time         number                pulsar chunk
    //chunk_1673984110307885148_1508_a.ts?type=live
    // fast
    // prod-fastly-us-east-1.video.....



    public class SRSManagersActor : ReceiveActor
    {
        private IActorRef _cutMergeService;
        private PulsarSystem _pulsarSystem = PulsarSystem.GetInstance(Context.System, actorSystemName: "Pulsar");
        private Dictionary<string, IActorRef> _srs = new Dictionary<string, IActorRef>();
        private IActorRef _dvrPlan;
        private readonly ILoggingAdapter _log;
        private PulsarClientConfigBuilder _client;
        public SRSManagersActor()
        {
            _cutMergeService = Context.ActorOf(CutMergeServiceActor.Prop());
            _dvrPlan = Context.ActorOf(DvrPlanActor.Prop(_pulsarSystem, _cutMergeService));
            _log = Context.GetLogger();

            ReceiveAsync<GetRunningSrsInfoList>(async g =>
            {

                var result = new List<Self_Srs>();
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                foreach(var srs in _srs.Values)
                {
                    var s = await srs.Ask<Self_Srs>(GetRunningSrsInfo.Instance);
                    if(s != null)
                    { result.Add(s); }
                }
                if(result.Count == 0)
                {
                    rs.Code = ErrorNumber.SrsObjectNotInit;
                    rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                    Sender.Tell(new ApisResult(null!, rs));
                }
                else
                    Sender.Tell(new ApisResult(result, rs));
            });
            ReceiveAsync<StopAllSrs>(async _ =>
            {
                var result = new List<SrsStartStatus>();
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                foreach (var srs in _srs.Values)
                {
                    var s = await srs.Ask<SrsStartStatus>(StopSrs.Instance);
                    if (s != null)
                    { result.Add(s); }
                }
                if (result.Count == 0)
                {
                    rs.Code = ErrorNumber.SrsObjectNotInit;
                    rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                    Sender.Tell(new ApisResult(null!, rs));
                }
                else
                    Sender.Tell(new ApisResult(result, rs));
            });
            ReceiveAsync<InitAndStartAllSrs>(async _ =>
            {
                var result = new List<SrsStartStatus>();
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                foreach (var srs in _srs.Values)
                {
                    var s = await srs.Ask<SrsStartStatus>(InitAndStart.Instance);
                    if (s != null)
                    { result.Add(s); }
                }
                if (result.Count == 0)
                {
                    rs.Code = ErrorNumber.SrsObjectNotInit;
                    rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                    Sender.Tell(new ApisResult(null!, rs));
                }
                else
                    Sender.Tell(new ApisResult(result, rs));

            });

            Receive<GetManagerSrs>(_ => Sender.Tell(new ManagerSrs(_srs)));
            Receive<DvrPlan>(d => _dvrPlan.Forward(d));
            Receive<GlobalSrs>(g =>
            {
                var srs = SRSManager(g.DeviceId);
                srs.Forward(g);
            });
            Receive<VhostTranscode>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostSecurity>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostRtc>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostRefer>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostPublish>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostPlay>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostIngest>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostHttpStatic>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostHttpRemux>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostHttpHooks>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostHls>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostHds>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostForward>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostExec>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostDvr>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostDash>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<Vhost>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostCluster>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostBandcheck>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<VhostBandcheck>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<StreamCaster>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<Stats>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<SrtServer>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
            Receive<DvrPlan>(v =>
            {                
               _dvrPlan.Forward(v);
            });
            
            ReceiveAsync<Messages.System > (async v =>
            {
                if(v.Sm != null)
                {
                    await CreateNewSrsInstance(v.Sm);
                }
                else
                {
                    var srs = SRSManager(v.DeviceId);
                    srs.Forward(v);
                }
                
            });
            Receive<GetAllSrsManagerDeviceId>(_ =>
            {
                List<string> list = null!;
                if (_srs.Count > 0)
                {
                    list = new List<string>();
                }
                else
                {
                    Sender.Tell(null!);
                    return;
                }

                foreach (var srs in _srs.Keys)
                {
                    if (srs != null)
                    {
                        list.Add(srs);
                    }
                }
                Sender.Tell(list);
            });

            Receive<GetDriveDisksInfo>(_ =>
            {                
                Sender.Tell(GetDriveDisksInfo());
            });

            ReceiveAsync<GetSystemInfo>(async _ =>
            {
                var sim = new SystemInfoModule();
                sim.NetworkInterfaceList = GetNetworkAdapterList();
                sim.Architecture = RuntimeInformation.OSArchitecture.ToString();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    sim.Platform = "linux";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    sim.Platform = "windows";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    sim.Platform = "Mac/OSX";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                    sim.Platform = "freebsd";
                sim.DisksInfo = GetDriveDisksInfo();
                sim.Version = RuntimeInformation.OSDescription + " " + Environment.OSVersion;
                sim.X64 = Environment.Is64BitOperatingSystem;
                sim.HostName = Environment.MachineName;
                sim.CpuCoreSize = (ushort)Environment.ProcessorCount;

                if (_srs.Count > 0)
                {
                    foreach (var srs in _srs.Values)
                    {
                        var sm = await srs.Ask<SrsManager>(GetSrsManager.Instance);
                        if (sm.IsRunning && sm.Srs.Http_api != null && sm.Srs.Http_api.Enabled == true)
                        {
                            var reqUrl = "http://127.0.0.1:" + sm!.Srs.Http_api!.Listen + "/api/v1/summaries";
                            try
                            {
                                var tmpStr = NetHelperNew.HttpGetRequest(reqUrl, null!);
                                var retReq = JsonHelper.FromJson<SrsSystemInfo>(tmpStr);
                                if (retReq != null && retReq.Data != null && retReq.Data.Self != null)
                                {
                                    if (sim.SrsList == null) sim.SrsList = new List<Self_Srs>();
                                    var filename = Path.GetFileName(retReq.Data.Self.Argv)!;
                                    var ext = Path.GetExtension(filename);
                                    retReq.Data.Self.Srs_DeviceId = filename.Replace(ext, "");
                                    sim.SrsList.Add(retReq.Data.Self);
                                    if (sim.System == null)
                                    {
                                        sim.System = retReq.Data.System;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }

                        Thread.Sleep(50);
                    }
                }

                Sender.Tell(sim);
            });
            Receive<GetNetworkAdapterList>(_ =>
            {
                Sender.Tell(GetNetworkAdapterList());
            });
            Receive<GetSrsInstanceTemplate>(_ =>
            {
                Sender.Tell(new ApisResult(GetSrsInstanceTemplate(out var rs), rs));
            });
            Receive<GetSrtServerTemplate>(_ =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                var srs = new SrsSrtServerConfClass()
                {
                    SectionsName = "srt_server",
                    Enabled = true,
                    Listen = 10080,
                    Maxbw = 1000000000,
                    Connect_timeout = 4000,
                    Peerlatency = 300,
                    Recvlatency = 300,
                    Sendbuf = 2000000,
                    Recvbuf = 2000000,
                    Latency = 100,
                    Tsbpdmode = false,
                    Tlpktdrop = false,
                    Passphrase = "",
                    Pbkeylen = 16
                };
                Sender.Tell(new ApisResult(srs, rs));
            });
            Receive<GetStreamCasterTemplate>(g =>
            {
                var casterType = g.CasterType;
                var result = new SrsStreamCasterConfClass();
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None] + "\r\n" + JsonHelper.ToJson(casterType),
                };
                switch (casterType)
                {
                    case CasterEnum.flv:
                        result.InstanceName = "streamcaster-flv-template";
                        result.SectionsName = "stream_caster";
                        result.sip = null;
                        result.Enabled = true;
                        result.Caster = CasterEnum.flv;
                        result.Listen = 8936;
                        result.Output = "rtmp://127.0.0.1/[vhost]/[app]/[stream]";
                        Sender.Tell(new ApisResult(result, rs));
                        break;
                    case CasterEnum.gb28181:
                        result.InstanceName = "streamcaster-gb28181-template";
                        result.SectionsName = "stream_caster";
                        result.sip = new Sip();
                        result.sip.SectionsName = "sip";
                        result.sip.Enabled = true;
                        result.sip.Listen = 5060;
                        result.sip.Serial = "34020000002000000001"; //server-id
                        result.sip.Realm = "3402000000"; //server domain
                        result.sip.Ack_timeout = 30;
                        result.sip.Keepalive_timeout = 120;
                        result.sip.Auto_play = true;
                        result.sip.Invite_port_fixed = true;
                        result.sip.Query_catalog_interval = 60;
                        result.Enabled = true;
                        result.Caster = CasterEnum.gb28181;
                        result.Output = "rtmp://127.0.0.1/[vhost]/[app]/[stream]";
                        result.Listen = 9000;
                        result.Rtp_port_max = 58300;
                        result.Rtp_port_min = 58200;
                        result.Wait_keyframe = false;
                        result.Rtp_idle_timeout = 30;
                        result.Audio_enable = true; //Only supports acc format audio stream
                        result.Host = "*";
                        result.Auto_create_channel = false;
                        Sender.Tell(new ApisResult(result, rs));
                        break;
                    case CasterEnum.mpegts_over_udp:
                        result.InstanceName = "streamcaster-mpegts_over_udp-template";
                        result.SectionsName = "stream_caster";
                        result.sip = null;
                        result.Enabled = true;
                        result.Caster = CasterEnum.mpegts_over_udp;
                        result.Listen = 1935;
                        result.Output = "rtmp://127.0.0.1/[vhost]/[app]/[stream]";
                        Sender.Tell(new ApisResult(result, rs));
                        break;
                    case CasterEnum.rtsp:
                        result.InstanceName = "streamcaster-rtsp-template";
                        result.SectionsName = "stream_caster";
                        result.sip = null;
                        result.Enabled = true;
                        result.Caster = CasterEnum.rtsp;
                        result.Listen = 554;
                        result.Output = "rtmp://127.0.0.1/[vhost]/[app]/[stream]";
                        result.Rtp_port_min = 57200;
                        result.Rtp_port_max = 57300;
                        Sender.Tell(new ApisResult(result, rs));
                        break;
                    default:
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.FunctionInputParamsError,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError] + "\r\n" +
                                      JsonHelper.ToJson(casterType),
                        };
                        Sender.Tell(new ApisResult(null!, rs));
                        break;
                }
               
            });
            Receive<GetRtcServerTemplate>(_ =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                var rtc = new SrsRtcServerConfClass()
                {
                    Enabled = true,
                    Listen = 8000,
                    Candidate = "*",
                    SectionsName = "rtc_server",
                    Protocol = "udp",
                    Tcp = new Tcp
                    {
                        Enabled = false,
                        Listen = 8000
                    },
                    UseAutoDetectNetworkIp = true,
                    IpFamily = "ipv4",
                    ApiAsCandidates = true,
                    ResolveApiDomain = true,
                    KeepApiDomain = false,
                    Black_hole = new BlackHole()
                    {
                        Enabled = false,
                        Addr = "127.0.0.1:10000",
                        SectionsName = "black_hole",
                    }
                };
                Sender.Tell(new ApisResult(rtc, rs));
            });
        }
        private List<NetworkInterfaceModule> GetNetworkAdapterList()
        {
            var listofnetwork = new List<NetworkInterfaceModule>();
            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            var ipaddr = "";
            ushort index = 0;
            if (adapters.Length > 0)
            {
                foreach (var adapter in adapters)
                {
                    if (adapter.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                    ipaddr = "";
                    var adapterProperties = adapter.GetIPProperties();
                    var ipCollection = adapterProperties.UnicastAddresses;
                    foreach (var ipadd in ipCollection)
                    {
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipaddr = ipadd.Address.ToString(); //get ip
                        }
                        else if (ipadd.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            //Local IPV6 address, do not take IPV6 address
                        }
                    }

                    var tmp_adapter = new NetworkInterfaceModule()
                    {
                        Index = index,
                        Name = adapter.Name,
                        //  Speed = (adapter.Speed / 1000 / 1000).ToString() + "MB", //linux not Supported
                        Mac = adapter.GetPhysicalAddress().ToString(),
                        Type = adapter.NetworkInterfaceType.ToString(),
                        Ipaddr = ipaddr,
                    };
                    index++;
                    if (!string.IsNullOrEmpty(tmp_adapter.Ipaddr))
                    {
                        var loop = 1;
                        if (!tmp_adapter.Mac.Contains('-'))
                        {
                            for (var i = 1; i < 10; i += 2)
                            {
                                tmp_adapter.Mac = tmp_adapter.Mac.Insert(i + loop, "-");
                                loop += 1;
                            }
                        }

                        listofnetwork.Add(tmp_adapter);
                    }
                }

                return listofnetwork;
            }

            return null!;
        }
        private SrsManager GetSrsInstanceTemplate(out ResponseStruct rs)
        {
            var srsManager = new SrsManager();
            srsManager.Srs = new SrsSystemConfClass();

            srsManager.Srs = new SrsSystemConfClass();
            srsManager.Srs.Listen = 1935;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                srsManager.Srs.Max_connections = 1000;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                srsManager.Srs.Max_connections = 128;
            }
            else
            {
                srsManager.Srs.Max_connections = 512;
            }

            srsManager.SrsDeviceId = SrsManageCommon.Common.CreateUuid()?.Trim()!;
            srsManager.SrsWorkPath = Common.WorkPath;
            srsManager.Srs.Srs_log_file = srsManager.SrsWorkPath + srsManager.SrsDeviceId + "/srs.log";
            srsManager.Srs.Srs_log_level = "verbose"; //Observer initially
            srsManager.Srs.Pid = srsManager.SrsWorkPath + srsManager.SrsDeviceId + "/srs.pid";
            srsManager.Srs.Chunk_size = 6000;
            srsManager.Srs.Ff_log_dir = srsManager.SrsWorkPath + srsManager.SrsDeviceId + "/ffmpegLog/";
            srsManager.Srs.Ff_log_level = "warning";
            srsManager.Srs.Daemon = true;
            srsManager.Srs.Utc_time = false;
            srsManager.Srs.Work_dir = srsManager.SrsWorkPath;
            srsManager.Srs.Asprocess = false; //If the parent process is closed, if false, srs will not be closed
            srsManager.Srs.Inotify_auto_reload = false; //Configuration file modification does not automatically reload
            srsManager.Srs.Srs_log_tank = "file";
            srsManager.Srs.Grace_start_wait = 2300;
            srsManager.Srs.Grace_final_wait = 3200;
            srsManager.Srs.Force_grace_quit = false;
            srsManager.Srs.Http_api = new SrsHttpApiConfClass();
            srsManager.Srs.Http_api.Crossdomain = true;
            srsManager.Srs.Http_api.Enabled = true;
            srsManager.Srs.Http_api.Listen = 8000;
            srsManager.Srs.Http_api.InstanceName = "";
            srsManager.Srs.Http_api.SectionsName = "http_api";
            /*srsManager.Srs.Http_api.Raw_Api = new RawApi();
            srsManager.Srs.Http_api.Raw_Api.Allow_query = true;
            srsManager.Srs.Http_api.Raw_Api.Allow_reload = true;
            srsManager.Srs.Http_api.Raw_Api.Allow_update = true;
            srsManager.Srs.Http_api.Raw_Api.SectionsName = "raw_api";
            srsManager.Srs.Http_api.Raw_Api.Enabled = true;*/
            srsManager.Srs.Heartbeat = new SrsHeartbeatConfClass();
            srsManager.Srs.Heartbeat.Device_id = SrsManageCommon.Common.AddDoubleQuotation(srsManager.SrsDeviceId!);
            srsManager.Srs.Heartbeat.Enabled = true;
            srsManager.Srs.Heartbeat.SectionsName = "heartbeat";
            srsManager.Srs.Heartbeat.Interval = 5; //in seconds
            srsManager.Srs.Heartbeat.Summaries = true;
            srsManager.Srs.Heartbeat.Url = "http://127.0.0.1:5000/api/v1/heartbeat";
            srsManager.Srs.Http_server = new SrsHttpServerConfClass();
            srsManager.Srs.Http_server.Enabled = true;
            srsManager.Srs.Http_server.Dir = srsManager.SrsWorkPath + srsManager.SrsDeviceId + "/wwwroot";
            srsManager.Srs.Http_server.Listen = 8001;
            srsManager.Srs.Http_server.SectionsName = "http_server";
            srsManager.Srs.Http_server.Crossdomain = true;
            srsManager.Srs.Vhosts = new List<SrsvHostConfClass>();
            var vhost = new SrsvHostConfClass();
            vhost.SectionsName = "vhost";
            vhost.VhostDomain = "__defaultVhost__";
            srsManager.Srs.Vhosts.Add(vhost);
            rs = new ResponseStruct();
            rs.Code = ErrorNumber.None;
            rs.Message = ErrorMessage.ErrorDic![ErrorNumber.None];
            return srsManager;
        }

        private async ValueTask<SrsManager> CreateNewSrsInstance(SrsManager sm)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var check = false;
            ApisResult api;
            foreach (var srs in _srs.Values)
            {
                api = await srs.Ask<ApisResult>(new CheckNewSrsInstancePathRight(sm));

                if (api.Rt is bool value)
                {
                    if(value)
                        check = true;
                }
            }
            if(!check)   
                return null!;

            foreach (var srs in _srs.Values)
            {
                api = await srs.Ask<ApisResult>(new CheckNewSrsInstanceListenRight(sm));

                if (api.Rt is bool value)
                {
                    if (value)
                        check = true;
                }
            }
            if (!check)
                return null!;

            if (!sm.CreateSrsManagerSelf(out rs))
            {
                return null!;
            }

            return sm;
        }
        private List<DriveDiskInfo> GetDriveDisksInfo()
        {
            var disks = new List<DriveDiskInfo>();
            var drives = DriveInfo.GetDrives();
            foreach (var d in drives)
            {
                var ddi = new DriveDiskInfo()
                {
                    Format = d.DriveFormat,
                    VolumeLabel = d.VolumeLabel,
                    Free = (ulong)d.AvailableFreeSpace / 1000 / 1000,
                    Size = (ulong)d.TotalSize / 1000 / 1000,
                    Path = d.Name,
                    RootDirectory = d.RootDirectory.FullName,
                };
                if (ddi.Size > 0 && ddi.Free > 0)
                {
                    disks.Add(ddi);
                }
            }

            return disks;
        }
        private IActorRef SRSManager(string deviceId)
        {
            var d = $"deviceId_{deviceId}";
            if (_srs.ContainsKey(d))
                return _srs[d];
            else
            {
                var s = Context.ActorOf(SRSManagerActor.Prop(_pulsarSystem), d);
                _srs[d] = s;
                return s;
            }

        }
        
        public static Props Prop()
        {
            return Props.Create(() => new SRSManagersActor());
        }
    }
}
