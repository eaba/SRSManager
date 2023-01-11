using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
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
    /// vhost control class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Obtain the list of Instance names of the Vhost list
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Log]
        [Route("/Vhost/GetVhostsInstanceName")]
        public async ValueTask<JsonResult> GetVhostsInstanceName(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new Vhost(deviceId, "GetVhostsInstanceName"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get vhost by domain
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Vhost/GetVhostByDomain")]
        public async ValueTask<JsonResult> GetVhostByDomain(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new Vhost(deviceId, vhostDomain, "GetVhostByDomain"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get list of Vhosts
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Vhost/GetVhostList")]
        public async ValueTask<JsonResult> GetVhostList(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new Vhost(deviceId, "GetVhostList"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get various templates of Vhost [0:Stream] [1:File] [2:Device]
        /// </summary>
        /// <param name="vtype"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Vhost/GetVhostTemplate")]
        public async ValueTask<JsonResult> GetVhostTemplate(VhostIngestInputType vtype)
        {
            var rss = CommonFunctions.CheckParams(new object[] {vtype});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new Vhost(vtype, "GetVhostTemplate"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }


        /// <summary>
        /// Set or create Vhost parameters
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhost"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/Vhost/SetVhost")]
        public async ValueTask<JsonResult> SetVhost(string deviceId, SrsvHostConfClass vhost)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhost});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new Vhost(deviceId, vhost, "SetVhost"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Delete a vhost, use the domain name
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Vhost/DeleteVhostByDomain")]
        public async ValueTask<JsonResult> DeleteVhostByDomain(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new Vhost(deviceId, vhostDomain, "DeleteVhostByDomain"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Modify the domain name of the vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="newVhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Vhost/ChangeVhostDomain")]
        public async ValueTask<JsonResult> ChangeVhostDomain(string deviceId, string vhostDomain, string newVhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, newVhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new Vhost(deviceId, vhostDomain, newVhostDomain, "ChangeVhostDomain"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}