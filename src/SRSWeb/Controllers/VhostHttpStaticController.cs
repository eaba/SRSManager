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
    /// vhosthttpstatic interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostHttpStaticController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostHttpStaticController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete the HttpStatic configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpStatic/DeleteVhostHttpStatic")]
        public async ValueTask<JsonResult> DeleteVhostHttpStatic(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHttpStatic(deviceId, vhostDomain, "DeleteVhostHttpStatic"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get HttpStatic in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpStatic/GetVhostHttpStatic")]
        public async ValueTask<JsonResult> GetVhostHttpStatic(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHttpStatic(deviceId, vhostDomain, "GetVhostHttpStatic"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set or create HttpStatic
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="httpStatic"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpStatic/SetVhostHttpStatic")]
        public async ValueTask<JsonResult> SetVhostHttpStatic(string deviceId, string vhostDomain, HttpStatic httpStatic)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, httpStatic});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHttpStatic(deviceId, vhostDomain, httpStatic, "SetVhostHttpStatic"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}