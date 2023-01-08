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
    /// vhosttranscode interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostTranscodeController : ControllerBase
    {
        private readonly IActorRef _actor;
        public VhostTranscodeController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Delete a Transcode by VhostDomain and TranscodeInstanceName
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="transcodeInstanceName"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostTranscode/DeleteVhostTranscodeByTranscodeInstanceName")]
        public async ValueTask<JsonResult> DeleteVhostTranscodeByTranscodeInstanceName(string deviceId, string vhostDomain,
            string transcodeInstanceName)
        {
            var rss =
                CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, transcodeInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
 
            var a = await _actor.Ask<ApisResult>(new VhostTranscode(deviceId, vhostDomain, transcodeInstanceName, "DeleteVhostTranscodeByTranscodeInstanceName"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get all or specified transcode instance names in vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostTranscode/GetVhostTranscodeNameList")]
        public async ValueTask<JsonResult> GetVhostTranscodeNameList(string deviceId, string vhostDomain = "")
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new VhostTranscode(deviceId, vhostDomain, "GetVhostTranscodeNameList"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get a Transcode configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="transcodeInstanceName"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostTranscode/GetVhostTranscode")]
        public async ValueTask<JsonResult> GetVhostTranscode(string deviceId, string vhostDomain, string transcodeInstanceName)
        {
            var rss =
                CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, transcodeInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new VhostTranscode(deviceId, vhostDomain, transcodeInstanceName, "GetVhostTranscode"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Set up or create Transcode
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="transcodeInstanceName"></param>
        /// <param name="transcode"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostTranscode/SetVhostTranscode")]
        public async ValueTask<JsonResult> SetVhostTranscode(string deviceId, string vhostDomain, string transcodeInstanceName,
            Transcode transcode)
        {
            var rss = CommonFunctions.CheckParams(new object[]
                {deviceId, vhostDomain, transcodeInstanceName, transcode});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new VhostTranscode(deviceId, vhostDomain, transcodeInstanceName, "SetVhostTranscode"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
    }
}