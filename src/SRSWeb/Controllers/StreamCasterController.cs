using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Relational;
using SrsApis.SrsManager.Apis;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManager.Actors;
using SRSManager.Messages;
using SRSManager.Shared;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// SRSStreamCaster interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class StreamCasterController : ControllerBase
    {
        private readonly IActorRef _actor;
        public StreamCasterController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Get all StreamCaster instance names
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/GetStreamCasterInstanceNameList")]
        public async ValueTask<JsonResult> GetStreamCasterInstanceNameList(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new StreamCaster(deviceId, "GetStreamCasterInstanceNameList"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get all instances of StreamCaster
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/GetStreamCasterInstanceList")]
        public async ValueTask<JsonResult> GetStreamCasterInstanceList(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new StreamCaster(deviceId, "GetStreamCasterInstanceList"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Create an instance of StreamCaster
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/CreateStreamCaster")]
        public async ValueTask<JsonResult> CreateStreamCaster(string deviceId, SrsStreamCasterConfClass streamcaster)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, streamcaster});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new StreamCaster(deviceId, streamcaster, "CreateStreamCaster"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get the StreamCaster template
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/GetStreamCasterTemplate")]
        public async ValueTask<JsonResult> GetStreamCasterTemplate(CasterEnum casterType)
        {
            var rss = CommonFunctions.CheckParams(new object[] {casterType});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new GetStreamCasterTemplate(casterType));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Delete a streamcaster with instance name
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/DeleteStreamCasterByInstanceName")]
        public async ValueTask<JsonResult> DeleteStreamCasterByInstanceName(string deviceId, string instanceName)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, instanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new StreamCaster(deviceId, instanceName, "DeleteStreamCasterByInstanceName"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Modify the instance name of streamcaster
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/ChangeStreamCasterInstanceName")]
        public async ValueTask<JsonResult> ChangeStreamCasterInstanceName(string deviceId, string instanceName, string newInstanceName)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, instanceName, newInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new StreamCaster(deviceId, instanceName, newInstanceName, "ChangeStreamCasterInstanceName"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Stop or start a StreamCaster
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/OnOrOff")]
        public async ValueTask<JsonResult> OnOrOff(string deviceId, string instanceName, bool enable)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, instanceName, enable});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new StreamCaster(deviceId, instanceName, enable, "OnOrOffStreamCaster"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Setting up a StreamCaster
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/SetStreamCaster")]
        public async ValueTask<JsonResult> SetStreamCaster(string deviceId, SrsStreamCasterConfClass streamcaster)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, streamcaster});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new StreamCaster(deviceId, streamcaster, "SetStreamCaster"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}