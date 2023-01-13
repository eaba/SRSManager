using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.RequestModules;
using SRSManager.Actors;
using SRSManager.Messages;
using SRSManager.Shared;
using SRSWeb.Attributes;
using SharpPulsar.Builder;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// Global SRS interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class GlobalSrsController : ControllerBase
    {
        private readonly IActorRef _actor;
        public GlobalSrsController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Is srs running
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/IsRunning")]
        public async ValueTask<JsonResult> IsRunning(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
           // var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "IsRunning"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Whether srs is initialized
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/IsInit")]
        public async ValueTask<JsonResult> IsInit(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "IsInit"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// start srs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/StartSrs")]
        public async ValueTask<JsonResult> StartSrs(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "Start"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// stop srs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/StopSrs")]
        public async ValueTask<JsonResult> StopSrs(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "Stop"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// restart srs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/RestartSrs")]
        public async ValueTask<JsonResult> RestartSrs(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "Restart"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Reload the srs configuration (srs.reload)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/ReloadSrs")]
        public async ValueTask<JsonResult> ReloadtSrs(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "Reload"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter Chunksize
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeChunksize")]
        public async ValueTask<JsonResult> GlobalChangeChunksize(string deviceId, ushort chunkSize)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, chunkSize });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, chunkSize, "ChangeChunksize"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter HttpApiListen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpApiListen")]
        public async ValueTask<JsonResult> GlobalChangeHttpApiListen(string deviceId, ushort port)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, port });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, port, "ChangeHttpApiListen"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter HttpApiEnable
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpApiEnable")]
        public async ValueTask<JsonResult> GlobalChangeHttpApiEnable(string deviceId, bool enable)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, enable });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, enable, "ChangeHttpApiEnable"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter Maxconnections
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeMaxConnections")]
        public async ValueTask<JsonResult> GlobalChangeMaxConnections(string deviceId, ushort max)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, max });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, max, "ChangeMaxConnections"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter rtmp listen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeRtmpListen")]
        public async ValueTask<JsonResult> GlobalChangeRtmpListen(string deviceId, ushort port)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, port });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, port, "ChangeRtmpListen"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter Httpserver listen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpServerListen")]
        public async ValueTask<JsonResult> GlobalChangeHttpServerListen(string deviceId, ushort port)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId , port });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, port, "ChangeHttpServerListen"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter HttpserverPath
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpServerPath")]
        public async ValueTask<JsonResult> GlobalChangeHttpServerPath(string deviceId, string path)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, path });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, path, "ChangeHttpServerPath"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter Httpserver enable
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpServerEnable")]
        public async ValueTask<JsonResult> GlobalChangeHttpServerEnable(string deviceId, bool enable)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, enable });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, enable, "ChangeHttpServerEnable"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get the global parameters of the srs instance
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GetGlobalParams")]
        public async ValueTask<JsonResult> GetGlobalParams(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "GetGlobalParams"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameters of the srs instance
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/ChangeGlobalParams")]
        public async ValueTask<JsonResult> ChangeGlobalParams(ReqChangeSrsGlobalParams req)
        {
            var rss = CommonFunctions.CheckParams(new object[] { req });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(req.DeviceId, req.Gm, "ChangeGlobalParams"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Set Pulsar Client
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/SetPulsarClient")]
        public async ValueTask<JsonResult> SetPulsarClient(string deviceId, PulsarClientConfigBuilder client)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, client });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, client, "SetPulsarClient"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Get Pulsar Client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GetPulsarClient")]
        public async ValueTask<JsonResult> GetPulsarClient(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "GetPulsarClient"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Start Pulsar Client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/StartPulsarClient")]
        public async ValueTask<JsonResult> StartPulsarClient(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "StartPulsarClient"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Start Pulsar Client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/StopPulsarClient")]
        public async ValueTask<JsonResult> StopPulsarClient(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "StopPulsarClient"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Set Pulsar Producer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/SetPulsarProducer")]
        public async ValueTask<JsonResult> SetPulsarProducer(string deviceId, ProducerConfigBuilder<byte[]> producer)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, producer });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, producer, "SetPulsarProducer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Set Pulsar Producer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/SetPulsarProducer")]
        public async ValueTask<JsonResult> SetPulsarProducer<T>(string deviceId, ProducerConfigBuilder<T> producer)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, producer });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, producer, "SetPulsarProducer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Get Pulsar Producer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GetPulsarProducer")]
        public async ValueTask<JsonResult> GetPulsarProducer(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "GetPulsarProducer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Get Pulsar Producer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/PulsarProducer")]
        public async ValueTask<JsonResult> PulsarProducer(string deviceId, byte[] data)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, data });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, data, "PulsarProducer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Set Pulsar Consumer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/SetPulsarConsumer")]
        public async ValueTask<JsonResult> SetPulsarConsumer(string deviceId, ConsumerConfigBuilder<byte[]> consumer)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, consumer });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, consumer, "SetPulsarConsumer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Get Pulsar Consumer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GetPulsarConsumer")]
        public async ValueTask<JsonResult> GetPulsarConsumer(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "GetPulsarConsumer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Pulsar Consumer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/PulsarConsumer")]
        public async ValueTask<JsonResult> PulsarConsumer(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "PulsarConsumer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Set Pulsar Reader
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/SetPulsarReader")]
        public async ValueTask<JsonResult> SetPulsarReader(string deviceId, ReaderConfigBuilder<byte[]> reader)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, reader });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, reader, "SetPulsarReader"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Get Pulsar Reader
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GetPulsarReader")]
        public async ValueTask<JsonResult> GetPulsarReader(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "GetPulsarReader"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Pulsar Reader
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/PulsarReader")]
        public async ValueTask<JsonResult> PulsarReader(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "PulsarReader"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
    }
   
}
