using System.Reactive.Joins;
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
    /// vhostingest interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostIngestController
    {
        private readonly IActorRef _actor;
        public VhostIngestController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Get IngestList by deviceId, vhostDomain
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/GetVhostIngestList")]
        public async ValueTask<JsonResult> GetVhostIngestList(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostIngest(deviceId, vhostDomain, "GetVhostIngestList"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }


        /// <summary>
        /// Delete an Ingest by VhostDomain and IngestInstanceName
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="ingestInstanceName"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/DeleteVhostIngestByIngestInstanceName")]
        public async ValueTask<JsonResult> DeleteVhostIngestByIngestInstanceName(string deviceId, string vhostDomain,
            string ingestInstanceName)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, ingestInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostIngest(deviceId, vhostDomain, ingestInstanceName, "DeleteVhostIngestByIngestInstanceName"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get all or specified ingest instance names in vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/GetVhostIngestNameList")]
        public async ValueTask<JsonResult> GetVhostIngestNameList(string deviceId, string vhostDomain = "")
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostIngest(deviceId, vhostDomain, "GetVhostIngestNameList"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get an Ingest configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="ingestInstanceName"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/GetVhostIngest")]
        public async ValueTask<JsonResult> GetVhostIngest(string deviceId, string vhostDomain, string ingestInstanceName)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, ingestInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostIngest(deviceId, vhostDomain, ingestInstanceName, "GetVhostIngest"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Set up or create an Ingest
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="ingest"></param>
        /// <param name="ingestInstanceName"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/SetVhostIngest")]
        public async ValueTask<JsonResult> SetVhostIngest(string deviceId, string vhostDomain, string ingestInstanceName, Ingest ingest)
        {
            var rss = CommonFunctions.CheckParams(new object[]
                {deviceId, vhostDomain, ingest, ingestInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostIngest(deviceId, vhostDomain, ingestInstanceName,
                ingest, "SetVhostIngest"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }


        /// <summary>
        /// enable or disable an ingest
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="ingestInstanceName"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/OnOrOffIngest")]
        public async ValueTask<JsonResult> OnOrOffIngest(string deviceId, string vhostDomain, string ingestInstanceName, bool enable)
        {
            var rss = CommonFunctions.CheckParams(new object[]
                {deviceId, vhostDomain, enable, ingestInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new VhostIngest(deviceId, vhostDomain, ingestInstanceName,
                enable, "OnOrOffIngest"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }
    }
}