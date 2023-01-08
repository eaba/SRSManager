using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using SRSManager.Actors;
using SRSManager.Messages;
using SRSManager.Shared;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// vhostplay interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostPlayController
    {
        private readonly IActorRef _actor;
        public VhostPlayController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete Play configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostPlay/DeleteVhostPlay")]
        public async ValueTask<JsonResult> DeleteVhostPlay(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostPlay(deviceId, vhostDomain, "DeleteVhostPlay"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get Play in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostPlay/GetVhostPlay")]
        public async ValueTask<JsonResult> GetVhostPlay(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostPlay(deviceId, vhostDomain, "GetVhostPlay"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set up or create a Play
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="play"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostPlay/SetVhostPlay")]
        public async ValueTask<JsonResult> SetVhostPlay(string deviceId, string vhostDomain, Play play)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, play});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostPlay(deviceId, vhostDomain, play, "SetVhostPlay"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}