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
    /// vhost dvr interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostDvrController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostDvrController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete Dvr configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Route("/VhostDvr/DeleteVhostDvr")]
        public async ValueTask<JsonResult> DeleteVhostDvr(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostDvr(deviceId, vhostDomain, "DeleteVhostDvr"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get Dvr in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostDvr/GetVhostDvr")]
        public async ValueTask<JsonResult> GetVhostDvr(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostDvr(deviceId, vhostDomain, "GetVhostDvr"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set up or create a Dvr
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="dvr"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostDvr/SetVhostDvr")]
        public async ValueTask<JsonResult> SetVhostDvr(string deviceId, string vhostDomain, Dvr dvr)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, dvr});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            
            var rt = await _actor.Ask<ApisResult>(new VhostDvr(deviceId, vhostDomain, dvr, "SetVhostDvr"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}