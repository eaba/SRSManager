using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManager.Actors;
using SRSManager.Messages;
using SRSManager.Shared;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// vhostdash interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostDashController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostDashController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete Dash configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostDash/DeleteVhostDash")]
        public async ValueTask<JsonResult> DeleteVhostDash(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostDash(deviceId, vhostDomain, "DeleteVhostDash"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get Dash in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostDash/GetVhostDash")]
        public async ValueTask<JsonResult> GetVhostDash(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostDash(deviceId, vhostDomain, "GetVhostDash"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set up or create a Dash
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="dash"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostDash/SetVhostDash")]
        public async ValueTask<JsonResult> SetVhostDash(string deviceId, string vhostDomain, Dash dash)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, dash});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostDash(deviceId, vhostDomain, dash, "SetVhostDash"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}