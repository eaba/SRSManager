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
using Publish = SrsConfFile.SRSConfClass.Publish;

namespace SRSWeb.Controllers
{

    /// <summary>
    /// vhostpublish interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostPublishController
    {
        private readonly IActorRef _actor;
        public VhostPublishController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete the Publish configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostPublish/DeleteVhostPublish")]
        public async ValueTask<JsonResult> DeleteVhostPublish(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostPublish(deviceId, vhostDomain, "DeleteVhostPublish"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get Publish in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostPublish/GetVhostPublish")]
        public async ValueTask<JsonResult> GetVhostPublish(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostPublish(deviceId, vhostDomain, "GetVhostPublish"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set up or create a Publish
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostPublish/SetVhostPublish")]
        public async ValueTask<JsonResult> SetVhostPublish(string deviceId, string vhostDomain, Publish publish)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, publish});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostPublish(deviceId, vhostDomain, publish, "SetVhostPublish"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}