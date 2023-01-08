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
    /// vhosthttpremux interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostHttpRemuxController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostHttpRemuxController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete HttpRemux configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpRemux/DeleteVhostHttpRemux")]
        public async ValueTask<JsonResult> DeleteVhostHttpRemux(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHttpRemux(deviceId, vhostDomain, "DeleteVhostHttpRemux"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get HttpRemux in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpRemux/GetVhostHttpRemux")]
        public async ValueTask<JsonResult> GetVhostHttpRemux(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHttpRemux(deviceId, vhostDomain, "GetVhostHttpRemux"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set up or create HttpRemux
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="httpRemux"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpRemux/SetVhostHttpRemux")]
        public async ValueTask<JsonResult> SetVhostHttpRemux(string deviceId, string vhostDomain, HttpRemux httpRemux)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, httpRemux});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHttpRemux(deviceId, vhostDomain, httpRemux, "SetVhostHttpRemux"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}