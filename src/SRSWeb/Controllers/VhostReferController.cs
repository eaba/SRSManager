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
    /// vhostrefer interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostReferController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostReferController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete Refer configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostRefer/DeleteVhostRefer")]
        public async ValueTask<JsonResult> DeleteVhostRefer(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostRefer(deviceId, vhostDomain, "DeleteVhostRefer"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get Refer from Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostRefer/GetVhostRefer")]
        public async ValueTask<JsonResult> GetVhostRefer(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostRefer(deviceId, vhostDomain, "GetVhostRefer"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set up or create a Refer
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostRefer/SetVhostRefer")]
        public async ValueTask<JsonResult> SetVhostRefer(string deviceId, string vhostDomain, Refer refer)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, refer});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            
            var rt = await _actor.Ask<ApisResult>(new VhostRefer(deviceId, vhostDomain, refer, "SetVhostRefer"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}