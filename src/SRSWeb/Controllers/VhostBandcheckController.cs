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
    /// vhostbandcheck interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostBandcheckController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostBandcheckController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete Bandcheck configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostBandcheck/DeleteVhostBandcheck")]
        public async ValueTask<JsonResult> DeleteVhostBandcheck(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostBandcheck(deviceId, vhostDomain, "DeleteVhostBandcheck"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get Bandcheck in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostBandcheck/GetVhostBandcheck")]
        public async ValueTask<JsonResult> GetVhostBandcheck(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostBandcheck(deviceId, vhostDomain, "GetVhostBandcheck"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set up or create a Bandcheck
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="bandcheck"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostBandcheck/SetVhostBandcheck")]
        public async ValueTask<JsonResult> SetVhostBandcheck(string deviceId, string vhostDomain, Bandcheck bandcheck)
        {
            var rss = CommonFunctions.CheckParams(new object[] {vhostDomain, deviceId, bandcheck});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostBandcheck(deviceId, vhostDomain, bandcheck, "SetVhostBandcheck"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}