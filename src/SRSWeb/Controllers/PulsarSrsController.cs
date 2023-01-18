using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsManageCommon;
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
    public class PulsarSrsController : ControllerBase
    {
        private readonly IActorRef _actor;
        public PulsarSrsController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        
        /// <summary>
        /// Set Pulsar Client
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/SetPulsarClient")]
        public async ValueTask<JsonResult> SetPulsarClient<T>(string deviceId, PulsarClientConfigBuilder client)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, client });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, client, "SetPulsarClient"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Get Pulsar Client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/GetPulsarClient")]
        public async ValueTask<JsonResult> GetPulsarClient<T>(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, "GetPulsarClient"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Start Pulsar Client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/StartPulsarClient")]
        public async ValueTask<JsonResult> StartPulsarClient<T>(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, "StartPulsarClient"));
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
        public async ValueTask<JsonResult> StopPulsarClient<T>(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, "StopPulsarClient"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Set Pulsar Producer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/SetPulsarProducer")]
        public async ValueTask<JsonResult> SetPulsarProducer<T>(string deviceId, ProducerConfigBuilder<T> producer)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, producer });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, producer, "SetPulsarProducer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
     
        /// <summary>
        /// Get Pulsar Producer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/GetPulsarProducer")]
        public async ValueTask<JsonResult> GetPulsarProducer<T>(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, "GetPulsarProducer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Get Pulsar Producer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/PulsarProducer")]
        public async ValueTask<JsonResult> PulsarProducer<T>(string deviceId, T data)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, data });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, data, "PulsarProducer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Set Pulsar Consumer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/SetPulsarConsumer")]
        public async ValueTask<JsonResult> SetPulsarConsumer<T>(string deviceId, ConsumerConfigBuilder<T> consumer)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, consumer });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, consumer, "SetPulsarConsumer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Get Pulsar Consumer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/GetPulsarConsumer")]
        public async ValueTask<JsonResult> GetPulsarConsumer<T>(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, "GetPulsarConsumer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Pulsar Consumer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/PulsarConsumer")]
        public async ValueTask<JsonResult> PulsarConsumer<T>(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, "PulsarConsumer"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Set Pulsar Reader
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/SetPulsarReader")]
        public async ValueTask<JsonResult> SetPulsarReader<T>(string deviceId, ReaderConfigBuilder<T> reader)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, reader });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, reader, "SetPulsarReader"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Get Pulsar Reader
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/GetPulsarReader")]
        public async ValueTask<JsonResult> GetPulsarReader<T>(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, "GetPulsarReader"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Pulsar Reader
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/PulsarSrs/PulsarReader")]
        public async ValueTask<JsonResult> PulsarReader<T>(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new PulsarSrs<T>(deviceId, "PulsarReader"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
    }
   
}
