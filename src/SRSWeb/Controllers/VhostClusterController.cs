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
    /// vhostcluster interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostClusterController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostClusterController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete the Cluster configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostCluster/DeleteVhostCluster")]
        public async ValueTask<JsonResult> DeleteVhostCluster(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostCluster(deviceId, vhostDomain, "DeleteVhostCluster"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get the Cluster in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostCluster/GetVhostCluster")]
        public async ValueTask<JsonResult> GetVhostCluster(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostCluster(deviceId, vhostDomain, "GetVhostCluster"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set up or create a Cluster
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="cluster"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostCluster/SetVhostCluster")]
        public async ValueTask<JsonResult> SetVhostCluster(string deviceId, string vhostDomain, Cluster cluster)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, cluster});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostCluster(deviceId, vhostDomain, cluster, "SetVhostCluster"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
    
}