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
    /// vhosthttphooks interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostHttpHooksController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostHttpHooksController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete HttpHooks configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpHooks/DeleteVhostHttpHooks")]
        public async ValueTask<JsonResult> DeleteVhostHttpHooks(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHttpHooks(deviceId, vhostDomain, "DeleteVhostHttpHooks"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get HttpHooks in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpHooks/GetVhostHttpHooks")]
        public async ValueTask<JsonResult> GetVhostHttpHooks(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHttpHooks(deviceId, vhostDomain, "GetVhostHttpHooks"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set or create HttpHooks
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="httpHooks"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpHooks/SetVhostHttpHooks")]
        public async ValueTask<JsonResult> SetVhostHttpHooks(string deviceId, string vhostDomain, HttpHooks httpHooks)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, httpHooks});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHttpHooks(deviceId, vhostDomain, httpHooks, "SetVhostHttpHooks"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}