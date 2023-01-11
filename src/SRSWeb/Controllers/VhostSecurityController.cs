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
    /// vhostsecurity interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostSecurityController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostSecurityController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete the Security configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostSecurity/DeleteVhostSecurity")]
        public async ValueTask<JsonResult> DeleteVhostSecurity(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new VhostSecurity(deviceId, vhostDomain, "DeleteVhostSecurity"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get Security in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostSecurity/GetVhostSecurity")]
        public async ValueTask<JsonResult> GetVhostSecurity(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new VhostSecurity(deviceId, vhostDomain, "GetVhostSecurity"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Set or create Security
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="security"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostSecurity/SetVhostSecurity")]
        public async ValueTask<JsonResult> SetVhostSecurity(string deviceId, string vhostDomain, Security security)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, security});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new VhostSecurity(deviceId, vhostDomain, security, "SetVhostSecurity"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
    }
}