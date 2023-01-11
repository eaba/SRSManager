using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsManageCommon;
using SRSManager.Actors;
using SRSManager.Messages;
using SRSManager.Shared;
using SRSWeb.Attributes;
using Hls = SrsConfFile.SRSConfClass.Hls;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// vhosthls interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostHlsController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostHlsController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete HLS configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHls/DeleteVhostHls")]
        public async ValueTask<JsonResult> DeleteVhostHls(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHls(deviceId, vhostDomain, "DeleteVhostHls"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get Hls in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHls/GetVhostHls")]
        public async ValueTask<JsonResult> GetVhostHls(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHls(deviceId, vhostDomain, "GetVhostHls"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set or create Hls
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="hostHls"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostHls/SetVhostHls")]
        public async ValueTask<JsonResult> SetVhostHls(string deviceId, string vhostDomain, Hls hostHls)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, hostHls});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostHls(deviceId, vhostDomain, hostHls, "SetVhostHls"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}