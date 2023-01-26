
using Akka.Actor;
using SharpPulsar.Schemas;
using SharpPulsar;
using SRSManager.Messages;
using Akka.Event;
using SharpPulsar.Trino.Message;
using SharpPulsar.Trino;
using System.Text.Json;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SrsApis.SrsManager;
using SharpPulsar.Interfaces;
using SharpPulsar.Builder;
using SRSManageCommon.ManageStructs;

namespace SRSManager.Actors
{
    internal class SrsHooksActor : ReceiveActor
    {
        private PulsarSystem _pulsarSystem;
        private AvroSchema<LiveBroadcastPlan> _broadcast; 
        private AvroSchema<OnlineClient> _client;
        private AvroSchema<ClientLog> _clientLog; 
        private AvroSchema<DvrVideo> _dvr;
        private PulsarSrsConfig _pulsarSrsConfig;
        private readonly ILoggingAdapter _log;
        public SrsHooksActor(PulsarSystem pulsarSystem)
        {
            _broadcast = AvroSchema<LiveBroadcastPlan>.Of(typeof(LiveBroadcastPlan));
            _client = AvroSchema<OnlineClient>.Of(typeof(OnlineClient));
            _clientLog = AvroSchema<ClientLog>.Of(typeof(ClientLog));
            _dvr = AvroSchema<DvrVideo>.Of(typeof(DvrVideo));
            _pulsarSystem = pulsarSystem;
            _log = Context.GetLogger();
            ReceiveAsync<SrsHooks>(vhIf => vhIf.Method == "OnDvr", async vh =>
            {
                await OnDvr(vh.Dvr, vh.Config, Sender);
            });
            Receive<SrsHooks>(vhIf => vhIf.Method == "OnHeartbeat", vh =>
            {
                OnHeartbeat(vh.HeartBeat!, Sender);
            });
            ReceiveAsync<SrsHooks>(vhIf => vhIf.Method == "OnUnPublish", async vh =>
            {
                await OnUnPublish(vh.Client!, vh.Config, Sender);
            });
            ReceiveAsync<SrsHooks>(vhIf => vhIf.Method == "OnPlay", async vh =>
            {
                await OnPlay(vh.Client!, vh.Config, Sender);
            }); 
            ReceiveAsync<SrsHooks>(vhIf => vhIf.Method == "OnStop", async vh =>
            {
                await OnStop(vh.Client!, vh.Config, Sender);
            });
            ReceiveAsync<SrsHooks>(vhIf => vhIf.Method == "OnPublish", async vh =>
            {
                await OnPublish(vh.Client!, vh.Config, Sender);
            });
        }
        private void OnHeartbeat(ReqSrsHeartbeat heartbeat, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var jsonStr = JsonHelper.ToJson(heartbeat);
            jsonStr = JsonHelper.ConvertJsonString(jsonStr);
            sender.Tell(new ApisResult(jsonStr, rs));
        }
        private async ValueTask OnDvr(DvrVideo dvrVideo, PulsarSrsConfig config, IActorRef sender)
        {
            _pulsarSrsConfig = config;
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            try
            {
                var pulsarConfig = new PulsarClientConfigBuilder()
                .ServiceUrl(config.BrokerUrl);
                var client = await _pulsarSystem.NewClient(pulsarConfig);
                dvrVideo.Deleted = false;
                dvrVideo.Undo = false;
                dvrVideo.UpdateTime = DateTime.Now;
                var list = await Context.Parent.Ask<ApisResult>(new GetOnPublishMonitorListById(dvrVideo.Device_Id!, config.Tenant, config.NameSpace, config.TrinoUrl) );
               
                var onPublishList = list.Rt as List<OnlineClient>; 
                var ret = onPublishList?.FindLast(x => x.Client_Id == dvrVideo.Client_Id);
                if (ret != null)
                {
                    dvrVideo.ClientIp = ret.MonitorIp;
                    dvrVideo.MonitorType = ret.MonitorType;
                    dvrVideo.RecordDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                var srs = await Context.Parent.Ask<SrsManager>(new Messages.System(dvrVideo.Device_Id!, "GetSrsManagerInstanceByDeviceId"));
                //var srs = SystemApis.GetSrsManagerInstanceByDeviceId(dvrVideo.Device_Id!);
                dvrVideo.Url = ":" + srs.Srs.Http_server!.Listen +
                               dvrVideo.VideoPath!.Replace(srs.Srs.Http_server.Dir!, "");

                var producerConfig = new ProducerConfigBuilder<DvrVideo>()
                .ProducerName($"live_video_{Guid.NewGuid}")
                .Schema(_dvr)
                .Topic("{DVR}");
                var producer = await client.NewProducerAsync(_dvr, producerConfig);
                var msgId = await producer.NewMessage().Value(dvrVideo).SendAsync();
                sender.Tell(new ApisResult(msgId, rs));
                await producer.CloseAsync();
                await client.ShutdownAsync();  
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SystemDataBaseExcept,
                    Message = ex.ToString()
                };
                _log.Error("save DVR error：" + ex.Message);
                sender.Tell(new ApisResult(null!, rs));
            }

        }
        private async ValueTask OnUnPublish(OnlineClient onlineClient, PulsarSrsConfig config, IActorRef sender)
        {
            _pulsarSrsConfig = config;
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var pulsarConfig = new PulsarClientConfigBuilder()
                .ServiceUrl(config.BrokerUrl);
            var client = await _pulsarSystem.NewClient(pulsarConfig);
            var msgIds = new List<MessageId>();    

            if (onlineClient != null && !string.IsNullOrEmpty(onlineClient.Device_Id) &&
                onlineClient.Client_Id != null)
            {
                try
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
                    
                    var producerConfig = new ProducerConfigBuilder<ClientLog>()
                    .ProducerName($"log_{Guid.NewGuid}")
                    .Schema(_clientLog)
                    .Topic("ClientLog");

                    var producer = await client.NewProducerAsync(_clientLog, producerConfig);
                    var msgId = await producer.NewMessage().Value(tmpClientLog).SendAsync();
                    msgIds.Add(msgId);
                    _log.Info(msgId.ToString());
                    await producer.CloseAsync();
                    producer = null;
                    //LiveBroadcast>(vhIf => vhIf.Method == "CheckIsLiveCh"
                    var plan = await Context.Parent.Ask<ApisResult>(new LiveBroadcast(onlineClient, "CheckIsLiveCh"));

                    var retPlan = (LiveBroadcastPlan)plan.Rt;

                    if (retPlan != null)
                    {
                        var query1 = @$"select * from LiveBroadcastPlan
                        AND Id = '{retPlan.Id}'
                        Order By __publish_time__ ASC";
                        var ret1 = await LiveSql(query1);
                        var producerConfig2 = new ProducerConfigBuilder<LiveBroadcastPlan>()
                        .ProducerName($"live_video_{Guid.NewGuid}")
                        .Schema(_broadcast)
                        .Topic("LiveBroadcastPlan");

                        var producer2 = await client.NewProducerAsync(_broadcast, producerConfig2);
                        foreach(var e in ret1)
                        {
                            e.UpdateTime = DateTime.Now;
                            e.PlanStatus = LiveBroadcastPlanStatus.Finished;

                            var m = await producer2.NewMessage().Value(e).SendAsync();
                            msgIds.Add(m);
                        }
                        
                        _log.Info(msgId.ToString());
                        await producer2.CloseAsync();
                        producer2 = null;
                    }
                    var query = @$"select * from OnlineClient 
                    WHERE DeviceId = '{onlineClient.Device_Id.Trim()}' 
                    AND Client_Id = '{onlineClient.Client_Id}'
                    Order By __publish_time__ ASC";
                    //AND UpdateTime = 'CAST({onlineClient.UpdateTime!.Value.ToString("yyyy-MM-dd HH:mm:ss")} AS timestamp)' 

                    var ret = await OnlineSql(query);
                    var producerConfig3 = new ProducerConfigBuilder<OnlineClient>()
                        .ProducerName($"live_video_{Guid.NewGuid}")
                        .Schema(_client)
                        .Topic("OnlineClient");

                    var producer3 = await client.NewProducerAsync(_client, producerConfig3);
                    if(ret.Count > 0)
                    {
                        foreach(var d in ret)
                        {
                            d.IsOnline = false;
                            d.UpdateTime = onlineClient.UpdateTime;
                            d.Client_Id = onlineClient.Client_Id;
                            d.Device_Id = onlineClient.Device_Id.Trim();
                            var m = await producer3.NewMessage().Value(d).SendAsync();
                            msgIds.Add(m);  
                            _log.Info(m.ToString());
                        }
                    }
                    else
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
                        var m = await producer3.NewMessage().Value(onlineClient).SendAsync();
                        msgIds.Add(m);
                        _log.Info(m.ToString());
                    }
                   
                    await producer3.CloseAsync();
                    producer3 = null;
                    sender.Tell(new ApisResult(msgIds, rs));
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SystemDataBaseExcept,
                        Message = ex.ToString()
                    };
                   _log.Error(ex.Message + "\r\n" + ex.StackTrace);
                    sender.Tell(new ApisResult(msgIds, rs));
                }
            }

        }
        private async ValueTask OnPlay(OnlineClient onlineClient, PulsarSrsConfig config, IActorRef sender)
        {
            _pulsarSrsConfig= config;
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var pulsarConfig = new PulsarClientConfigBuilder()
                .ServiceUrl(config.BrokerUrl);
            var client = await _pulsarSystem.NewClient(pulsarConfig);
            var msgIds = new List<MessageId>();

            if (onlineClient != null && onlineClient.Client_Id != null &&
                !string.IsNullOrEmpty(onlineClient.Device_Id))
            {
                try
                {
                    var tmpClientLog = new ClientLog();
                    var monitorIp = await GetMonitorIpAddressFromStream(onlineClient.Stream!);
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

                    var producerConfig = new ProducerConfigBuilder<ClientLog>()
                     .ProducerName($"log_{Guid.NewGuid}")
                     .Schema(_clientLog)
                     .Topic("ClientLog");

                    var producer = await client.NewProducerAsync(_clientLog, producerConfig);
                    var msgId = await producer.NewMessage().Value(tmpClientLog).SendAsync();
                    msgIds.Add(msgId);
                    _log.Info(msgId.ToString());
                    await producer.CloseAsync();
                    producer = null;

                    var query = @$"select * from OnlineClient 
                    WHERE DeviceId = '{onlineClient.Device_Id.Trim()}' 
                    AND Client_Id = '{onlineClient.Client_Id}'  
                    Order By __publish_time__ ASC";
                    var ret = await OnlineSql(query);
                    var producerConfig3 = new ProducerConfigBuilder<OnlineClient>()
                        .ProducerName($"live_video_{Guid.NewGuid}")
                        .Schema(_client)
                        .Topic("OnlineClient");

                    var producer3 = await client.NewProducerAsync(_client, producerConfig3);
                    if (ret.Count > 0)
                    {
                        foreach (var d in ret)
                        {
                            d.ClientType = ClientType.User;
                            d.IsOnline = true;
                            d.IsPlay = true;
                            d.Param= onlineClient.Param;
                            d.Stream= onlineClient.Stream;
                            d.UpdateTime = onlineClient.UpdateTime;
                            d.PageUrl= onlineClient.PageUrl;
                            d.MonitorIp= onlineClient.MonitorIp;
                            var m = await producer3.NewMessage().Value(d).SendAsync();
                            msgIds.Add(m);
                            _log.Info(m.ToString());
                        }
                    }
                    else
                    {
                        onlineClient.ClientType = ClientType.User;
                        onlineClient.IsOnline = true;
                        onlineClient.IsPlay = true;
                        onlineClient.MonitorIp = await GetMonitorIpAddressFromStream(onlineClient.Stream!);
                        var m = await producer3.NewMessage().Value(onlineClient).SendAsync();
                        msgIds.Add(m);
                        _log.Info(m.ToString());
                    }

                    await producer3.CloseAsync();
                    producer3 = null;
                    sender.Tell(new ApisResult(msgIds, rs));
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SystemDataBaseExcept,
                        Message = ex.ToString()
                    };
                    _log.Error(ex.Message + "\r\n" + ex.StackTrace);
                    sender.Tell(new ApisResult(msgIds, rs));
                }
            }

        }
        private async ValueTask OnStop(OnlineClient onlineClient, PulsarSrsConfig config, IActorRef sender)
        {
            _pulsarSrsConfig = config;
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var pulsarConfig = new PulsarClientConfigBuilder()
                .ServiceUrl(config.BrokerUrl);
            var client = await _pulsarSystem.NewClient(pulsarConfig);
            var msgIds = new List<MessageId>();

            if (onlineClient != null && onlineClient.Client_Id != null &&
                !string.IsNullOrEmpty(onlineClient.Device_Id))
            {
                try
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

                    var producerConfig = new ProducerConfigBuilder<ClientLog>()
                     .ProducerName($"log_{Guid.NewGuid}")
                     .Schema(_clientLog)
                     .Topic("ClientLog");

                    var producer = await client.NewProducerAsync(_clientLog, producerConfig);
                    var msgId = await producer.NewMessage().Value(tmpClientLog).SendAsync();
                    msgIds.Add(msgId);
                    _log.Info(msgId.ToString());
                    await producer.CloseAsync();
                    producer = null;

                    var query = @$"select * from OnlineClient 
                    WHERE DeviceId = '{onlineClient.Device_Id.Trim()}' 
                    AND Client_Id = '{onlineClient.Client_Id}'  
                    Order By __publish_time__ ASC";
                    var ret = await OnlineSql(query);
                    var producerConfig3 = new ProducerConfigBuilder<OnlineClient>()
                        .ProducerName($"live_video_{Guid.NewGuid}")
                        .Schema(_client)
                        .Topic("OnlineClient");

                    var producer3 = await client.NewProducerAsync(_client, producerConfig3);
                    if (ret.Count > 0)
                    {
                        foreach (var d in ret)
                        {
                            d.ClientType = ClientType.User;
                            d.IsOnline = true;
                            d.IsPlay = false;
                            d.UpdateTime = onlineClient.UpdateTime;
                            var m = await producer3.NewMessage().Value(d).SendAsync();
                            msgIds.Add(m);
                            _log.Info(m.ToString());
                        }
                    }
                    else
                    {
                        onlineClient.ClientType = ClientType.User;
                        onlineClient.IsOnline = true;
                        onlineClient.IsPlay = false;
                        var m = await producer3.NewMessage().Value(onlineClient).SendAsync();
                        msgIds.Add(m);
                        _log.Info(m.ToString());
                    }

                    await producer3.CloseAsync();
                    producer3 = null;
                    sender.Tell(new ApisResult(msgIds, rs));
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SystemDataBaseExcept,
                        Message = ex.ToString()
                    };
                    _log.Error(ex.Message + "\r\n" + ex.StackTrace);
                    sender.Tell(new ApisResult(msgIds, rs));
                }
            }

        }
        private async ValueTask OnPublish(OnlineClient onlineClient, PulsarSrsConfig config, IActorRef sender)
        {
            _pulsarSrsConfig = config;
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var pulsarConfig = new PulsarClientConfigBuilder()
                .ServiceUrl(config.BrokerUrl);
            var client = await _pulsarSystem.NewClient(pulsarConfig);
            var msgIds = new List<MessageId>();

            if (onlineClient != null && !string.IsNullOrEmpty(onlineClient.Device_Id) &&
                onlineClient.Client_Id != null)
            {
                try
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

                    var producerConfig = new ProducerConfigBuilder<ClientLog>()
                     .ProducerName($"log_{Guid.NewGuid}")
                     .Schema(_clientLog)
                     .Topic("ClientLog");

                    var producer = await client.NewProducerAsync(_clientLog, producerConfig);
                    var msgId = await producer.NewMessage().Value(tmpClientLog).SendAsync();
                    msgIds.Add(msgId);
                    _log.Info(msgId.ToString());
                    await producer.CloseAsync();
                    producer = null;
                    var plan = await Context.Parent.Ask<ApisResult>(new LiveBroadcast(onlineClient, "CheckIsLiveCh"));

                    var retPlan = (LiveBroadcastPlan)plan.Rt;
                    var boool = await Context.Parent.Ask<ApisResult>(new LiveBroadcast(retPlan, "CheckLivePlan"));

                    var retBool = (bool)boool.Rt; //If it is not in the live broadcast plan, kick the connection
                    if (!retBool && retPlan != null)
                    {
                        sender.Tell(new ApisResult(msgIds, rs));
                        return;
                    }

                    if (retPlan != null)
                    {
                        var query1 = @$"select * from LiveBroadcastPlan
                        AND Id = '{retPlan.Id}'
                        Order By __publish_time__ ASC";
                        var ret1 = await LiveSql(query1);
                        var producerConfig2 = new ProducerConfigBuilder<LiveBroadcastPlan>()
                        .ProducerName($"live_video_{Guid.NewGuid}")
                        .Schema(_broadcast)
                        .Topic("LiveBroadcastPlan");

                        var producer2 = await client.NewProducerAsync(_broadcast, producerConfig2);
                        foreach (var e in ret1)
                        {
                            e.UpdateTime = DateTime.Now;
                            e.PlanStatus = LiveBroadcastPlanStatus.Living;

                            var m = await producer2.NewMessage().Value(e).SendAsync();
                            msgIds.Add(m);
                        }

                        _log.Info(msgId.ToString());
                        await producer2.CloseAsync();
                        producer2 = null;
                    }
                    var query = @$"select * from OnlineClient 
                    WHERE DeviceId = '{onlineClient.Device_Id.Trim()}' 
                    AND Client_Id = '{onlineClient.Client_Id}'
                    Order By __publish_time__ ASC";
                    //AND UpdateTime = 'CAST({onlineClient.UpdateTime!.Value.ToString("yyyy-MM-dd HH:mm:ss")} AS timestamp)' 

                    var ret = await OnlineSql(query);
                    var producerConfig3 = new ProducerConfigBuilder<OnlineClient>()
                        .ProducerName($"live_video_{Guid.NewGuid}")
                        .Schema(_client)
                        .Topic("OnlineClient");

                    var producer3 = await client.NewProducerAsync(_client, producerConfig3);
                    if (ret.Count > 0)
                    {
                        foreach (var d in ret)
                        {
                            d.ClientType = ClientType.Monitor;
                            d.IsOnline = true;
                            d.HttpUrl= onlineClient.HttpUrl;
                            d.Param= onlineClient.Param;
                            d.Stream = onlineClient.Stream; 
                            d.UpdateTime = onlineClient.UpdateTime;
                            var m = await producer3.NewMessage().Value(d).SendAsync();
                            msgIds.Add(m);
                            _log.Info(m.ToString());
                        }
                    }
                    else
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
                        var m = await producer3.NewMessage().Value(onlineClient).SendAsync();
                        msgIds.Add(m);
                        _log.Info(m.ToString());
                    }

                    await producer3.CloseAsync();
                    producer3 = null;
                    sender.Tell(new ApisResult(msgIds, rs));           
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SystemDataBaseExcept,
                        Message = ex.ToString()
                    };
                    _log.Error(ex.Message + "\r\n" + ex.StackTrace);
                    sender.Tell(new ApisResult(msgIds, rs));
                }
            }

        }
        private async ValueTask<string> GetMonitorIpAddressFromStream(string stream)
        {
            var query = @$"select * from OnlineClient 
                WHERE ClientType = '{ClientType.Monitor!}' 
                AND Stream = '{stream!}' 
                Order By __publish_time__ ASC LIMIT 1";
            var sql = await OnlineSql(query);
            if(sql.Count > 0)
                return sql[0]!.MonitorIp!;

            return "unknow";
        }
        private async ValueTask<List<LiveBroadcastPlan>> LiveSql(string query)
        {
            var option = new ClientOptions
            {
                Server = _pulsarSrsConfig.TrinoUrl,
                Execute = query,
                Catalog = "pulsar",
                Schema = $"{_pulsarSrsConfig.Tenant}/{_pulsarSrsConfig.NameSpace}"
            };
            var sql = new SqlInstance(_pulsarSystem.System, option);
            var data = await sql.ExecuteAsync();
            var dvr = new List<LiveBroadcastPlan>();
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
                        dvr.Add(JsonSerializer.Deserialize<LiveBroadcastPlan>(json)!);
                    }
                    _log.Info(JsonSerializer.Serialize(dt.StatementStats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case ErrorResponse er:
                    _log.Info(JsonSerializer.Serialize(er, new JsonSerializerOptions { WriteIndented = true }));
                    break;
            }
            return dvr;
        }
        private async ValueTask<List<OnlineClient>> OnlineSql(string query)
        {
            var option = new ClientOptions
            {
                Server = _pulsarSrsConfig.TrinoUrl,
                Execute = query,
                Catalog = "pulsar",
                Schema = $"{_pulsarSrsConfig.Tenant}/{_pulsarSrsConfig.NameSpace}"
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
        private async ValueTask<List<ClientLog>> LogSql(string query)
        {
            var option = new ClientOptions
            {
                Server = _pulsarSrsConfig.TrinoUrl,
                Execute = query,
                Catalog = "pulsar",
                Schema = $"{_pulsarSrsConfig.Tenant}/{_pulsarSrsConfig.NameSpace}"
            };
            var sql = new SqlInstance(_pulsarSystem.System, option);
            var data = await sql.ExecuteAsync();
            var dvr = new List<ClientLog>();
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
                        dvr.Add(JsonSerializer.Deserialize<ClientLog>(json)!);
                    }
                    _log.Info(JsonSerializer.Serialize(dt.StatementStats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case ErrorResponse er:
                    _log.Info(JsonSerializer.Serialize(er, new JsonSerializerOptions { WriteIndented = true }));
                    break;
            }
            return dvr;
        }
        private async ValueTask<List<DvrVideo>> DvrSql(string query)
        {
            var option = new ClientOptions
            {
                Server = _pulsarSrsConfig.TrinoUrl,
                Execute = query,
                Catalog = "pulsar",
                Schema = $"{_pulsarSrsConfig.Tenant}/{_pulsarSrsConfig.NameSpace}"
            };
            var sql = new SqlInstance(_pulsarSystem.System, option);
            var data = await sql.ExecuteAsync();
            var dvr = new List<DvrVideo>();
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
                        dvr.Add(JsonSerializer.Deserialize<DvrVideo>(json)!);
                    }
                    _log.Info(JsonSerializer.Serialize(dt.StatementStats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case ErrorResponse er:
                    _log.Info(JsonSerializer.Serialize(er, new JsonSerializerOptions { WriteIndented = true }));
                    break;
            }
            return dvr;
        }
        public static Props Prop(PulsarSystem pulsarSystem)
        {
            return Props.Create(() => new SrsHooksActor(pulsarSystem));
        }
    }
}
