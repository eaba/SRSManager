using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
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
    /// SRSRtcServer device interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class RtcServerController : ControllerBase
    {
        private readonly IActorRef _actor;
        public RtcServerController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Get rtcserver configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/RtcServer/GetSrsRtcServer")]
        public async ValueTask<JsonResult> GetSrsRtcServer(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new RtcServer(deviceId, "GetSrsRtcServer"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set or create rtcserver
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/RtcServer/SetRtcServer")]
        public async ValueTask<JsonResult> SetSrsRtcServer(string deviceId, SrsRtcServerConfClass rtc)
        {
            var rss = CommonFunctions.CheckParams(new object[] {rtc});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new RtcServer(deviceId, rtc, "SetRtcServer"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// delete rtcserver
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/RtcServer/DelRtcServer")]
        public async ValueTask<JsonResult> DelSrsRtcServer(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new RtcServer(deviceId, "DelRtcServer"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}