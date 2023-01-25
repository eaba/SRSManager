
using System.Text.Json;
using Akka.Actor;
using Akka.Event;
using SharpPulsar;
using SharpPulsar.Builder;
using SharpPulsar.Schemas;
using SharpPulsar.Trino.Message;
using SharpPulsar.Trino;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using SRSManager.Messages;
using SrsApis.SrsManager.Apis;
using SrsConfFile.SRSConfClass;
using Org.BouncyCastle.Ocsp;
using MySqlX.XDevAPI.Relational;

namespace SRSManager.Actors
{
    internal class LiveBroadCastActor : ReceiveActor
    {
        private PulsarSystem _pulsarSystem;
        private AvroSchema<LiveBroadcastPlan> _broadcast;
        private PulsarSrsConfig _pulsarSrsConfig;
        private readonly ILoggingAdapter _log;
        private Producer<LiveBroadcastPlan> _producer;
        private ProducerConfigBuilder<LiveBroadcastPlan> _producerConfig;
        private PulsarClient _client;
        private string _workPath = Environment.CurrentDirectory + "/";
        private PulsarClientConfigBuilder _pulsarConfig;
        private SystemConfig _systemConfig = null!;
        public LiveBroadCastActor(PulsarSystem pulsarSystem)
        {
            _broadcast = AvroSchema<LiveBroadcastPlan>.Of(typeof(LiveBroadcastPlan));
            _pulsarSystem = pulsarSystem;
            _log = Context.GetLogger();
            ReceiveAsync<LiveBroadcast>(vhIf => vhIf.Method == "PulsarSrsConfig", async vh =>
            {
                if (_pulsarConfig != null)
                {
                    return;
                }
                var f = vh.Config!.Value;
                _pulsarSrsConfig = f;
                _pulsarConfig = new PulsarClientConfigBuilder()
                .ServiceUrl(f.BrokerUrl);
                _client = await pulsarSystem.NewClient(_pulsarConfig);

                _producerConfig = new ProducerConfigBuilder<LiveBroadcastPlan>()
                .ProducerName("live_video")
                .Schema(_broadcast)
                .Topic($"{f.Topic}");
            });
            Receive<LiveBroadcast>(vhIf => vhIf.Method == "CheckIsLivePlan", vh =>
            {
                CheckLivePlan(vh.Plan!, Sender);
            });
            ReceiveAsync<LiveBroadcast>(vhIf => vhIf.Method == "DeleteLivePlanById", async vh =>
            {
                await DeleteLivePlanById(vh.Plan!, Sender);
            });
            ReceiveAsync<LiveBroadcast>(vhIf => vhIf.Method == "SetLivePlan", async vh =>
            {
                await SetLivePlan(vh.Rlbp!, Sender);
            });
            ReceiveAsync<LiveBroadcast>(vhIf => vhIf.Method == "CheckIsLiveCh", async vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                var query = @$"select * from LiveBroadcastPlan 
                WHERE DeviceId = '{vh.Client!.Device_Id!}' 
                AND App = '{vh.Client!.App!}'  
                AND Vhost = '{vh.Client!.Vhost!}' 
                AND Stream = '{vh.Client!.Stream!}' 
                Order By __publish_time__ ASC LIMIT 1";
                var sql = await Sql(query);
                Sender.Tell(new ApisResult(sql.FirstOrDefault()!, rs));
            });
            ReceiveAsync<LiveBroadcast>(vhIf => vhIf.Method == "GetLivePlanList", async vh =>
            {
                var rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                var query = @$"select * from LiveBroadcastPlan 
                WHERE DeviceId = '{vh.Plan!.DeviceId}' 
                AND App = '{vh.Plan!.App!}'  
                AND Vhost = '{vh.Plan!.Vhost!}' 
                AND Stream = '{vh.Plan!.Stream!}'                  
                AND PublishIpAddr = '{vh.Plan!.PublishIpAddr!}'  
                AND Enable = '{vh.Plan!.Enable!}'  
                AND PlanStatus = '{vh.Plan!.PlanStatus!}' 
                AND StartTime = 'CAST({vh.Plan!.StartTime!.Value.ToString("yyyy-MM-dd HH:mm:ss")} AS timestamp) '
                AND EndTime = 'CAST({vh.Plan!.EndTime!.Value.ToString("yyyy-MM-dd HH:mm:ss")} AS timestamp) ' 
                Order By __publish_time__ ASC";
                var sql = await Sql(query);
                Sender.Tell(new ApisResult(sql.ToList()!, rs));
            });
        }
        /// <summary>
        /// Delete a recording plan ById
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask DeleteLivePlanById(LiveBroadcastPlan id, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            try
            {
                if (_producer == null)
                {
                    _producer = await _client.NewProducerAsync(_broadcast, _producerConfig);
                }
                id.Delete = true;
                await _producer.NewMessage().Value(id).SendAsync();
                sender.Tell(new ApisResult(true, rs));
            }
            catch
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SystemDataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept],
                };
                sender.Tell(new ApisResult(false, rs));
            }
        }

        private void CheckLivePlan(LiveBroadcastPlan plan, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (plan != null)
            {
                if (plan.StartTime <= DateTime.Now && plan.EndTime >= DateTime.Now && plan.Enable == true) //If the time is not within the range, it will also be kicked out
                {
                    sender.Tell(new ApisResult(true, rs));
                }
            }

            sender.Tell(new ApisResult(false, rs));
        }
        private async ValueTask SetLivePlan(ReqLiveBroadcastPlan rlbp, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (rlbp.StartTime >= rlbp.EndTime || rlbp.StartTime >= DateTime.Now)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                sender.Tell(new ApisResult(false, rs));
            }

            try
            {
                if (_producer == null)
                {
                    _producer = await _client.NewProducerAsync(_broadcast, _producerConfig);
                }
                var query = @$"select * from LiveBroadcastPlan 
                WHERE Id = '{rlbp.Id}' AND delete = 'false'
                Order By __publish_time__ ASC LIMIT 1";
                var sql = await Sql(query);
                if(sql.Count > 0)
                {
                    foreach(var row in sql) 
                    {
                        var update = row;
                        row.App = rlbp.App;
                        row.Enable= rlbp.Enable;
                        row.Stream = rlbp.Stream;   
                        row.Vhost= rlbp.Vhost;
                        row.DeviceId= rlbp.DeviceId;
                        row.EndTime = rlbp.EndTime; 
                        row.StartTime = rlbp.StartTime;
                        row.UpdateTime = DateTime.Now;
                        row.Id = rlbp.Id;
                        await _producer.NewMessage().Value(update).SendAsync();
                    }
                    
                    var api = await Context.Parent.Ask<ApisResult>(new Vhost(rlbp.DeviceId!, rlbp.Vhost!, "GetVhostByDomain"));
                    var retVhost = api.Rt is SrsvHostConfClass;
                    var retAddVhost = false;
                    if (!retVhost)
                    {
                       
                        api = await Context.Parent.Ask<ApisResult>(new Vhost(VhostIngestInputType.WebCast, "GetVhostTemplate"));
                        if(api.Rt is SrsvHostConfClass retVhostTemp)
                        {
                            retVhostTemp.Vdvr!.Dvr_path = _workPath + rlbp.DeviceId +
                                                      "/wwwroot/dvr/[2006][01][02]/[vhost]/[app]/[stream]/[15]/[2006][01][02][15][04][05].mp4";
                            retVhostTemp.VhostDomain = "webcast";
                            retVhostTemp.Enabled = true;
                            api = await Context.Parent.Ask<ApisResult>(new Vhost(rlbp.DeviceId!, retVhostTemp, "SetVhost"));
                            retAddVhost = api.Rt is bool;
                        }
                        
                    }
                    else
                    {
                        retAddVhost = true;
                    }

                    if (retAddVhost)
                    {
                        sender.Tell(new ApisResult(true, rs));
                        return;
                    }
                    foreach (var row in sql)
                    {
                        var delete = row;
                        row.DeviceId = rlbp.DeviceId;
                        row.EndTime = rlbp.EndTime;
                        row.StartTime = rlbp.StartTime;
                        row.UpdateTime = DateTime.Now;
                        row.Id = rlbp.Id;
                        row.Delete = true;
                        await _producer.NewMessage().Value(delete).SendAsync();
                    }
                    sender.Tell(new ApisResult(false, rs));
                    return;
                }

                rlbp.UpdateTime = DateTime.Now;
                rlbp.PlanStatus = LiveBroadcastPlanStatus.WaitForExec;
                await _producer.NewMessage().Value(rlbp).SendAsync();
                var api2 = await Context.Parent.Ask<ApisResult>(new Vhost(rlbp.DeviceId!, rlbp.Vhost!, "GetVhostByDomain"));
                var retVhost2 = api2.Rt is SrsvHostConfClass;
                var retAddVhost2 = false;
                if (!retVhost2)
                {

                    api2 = await Context.Parent.Ask<ApisResult>(new Vhost(VhostIngestInputType.WebCast, "GetVhostTemplate"));
                    if (api2.Rt is SrsvHostConfClass retVhostTemp)
                    {
                        retVhostTemp.Vdvr!.Dvr_path = _workPath + rlbp.DeviceId +
                                                  "/wwwroot/dvr/[2006][01][02]/[vhost]/[app]/[stream]/[15]/[2006][01][02][15][04][05].mp4";
                        retVhostTemp.VhostDomain = "webcast";
                        retVhostTemp.Enabled = true;
                        api2 = await Context.Parent.Ask<ApisResult>(new Vhost(rlbp.DeviceId!, retVhostTemp, "SetVhost"));
                        retAddVhost2 = api2.Rt is bool;
                    }

                }
                else
                {
                    retAddVhost2 = true;
                }

                if (retAddVhost2)
                {
                    sender.Tell(new ApisResult(true, rs));
                    return;
                }
                
                rlbp.UpdateTime = DateTime.Now;
                rlbp.Id = rlbp.Id;
                rlbp.Delete = true;
                await _producer.NewMessage().Value(rlbp).SendAsync();
                sender.Tell(new ApisResult(false, rs));
            }
            catch
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SystemDataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept],
                };
                sender.Tell(new ApisResult(false, rs));
                return;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            sender.Tell(new ApisResult(false, rs));
        }
        private async ValueTask<List<LiveBroadcastPlan>> Sql(string query)
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
        public static Props Prop(PulsarSystem pulsarSystem)
        {
            return Props.Create(() => new LiveBroadCastActor(pulsarSystem));
        }
    }
}
