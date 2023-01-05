using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using SRSManager.Shared;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// vhosttranscode interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostTranscodeController
    {
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
        public JsonResult DeleteVhostTranscodeByTranscodeInstanceName(string deviceId, string vhostDomain,
            string transcodeInstanceName)
        {
            var rss =
                CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, transcodeInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostTranscodeApis.DeleteVhostTranscodeByTranscodeInstanceName(deviceId, vhostDomain,
                transcodeInstanceName, out var rs);
            return Result.DelApisResult(rt, rs);
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
        public JsonResult GetVhostTranscodeNameList(string deviceId, string vhostDomain = "")
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostTranscodeApis.GetVhostTranscodeNameList(deviceId, out var rs, vhostDomain);
            return Result.DelApisResult(rt, rs);
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
        public JsonResult GetVhostTranscode(string deviceId, string vhostDomain, string transcodeInstanceName)
        {
            var rss =
                CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, transcodeInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostTranscodeApis.GetVhostTranscode(deviceId, vhostDomain, transcodeInstanceName,
                out var rs);
            return Result.DelApisResult(rt, rs);
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
        public JsonResult SetVhostTranscode(string deviceId, string vhostDomain, string transcodeInstanceName,
            Transcode transcode)
        {
            var rss = CommonFunctions.CheckParams(new object[]
                {deviceId, vhostDomain, transcodeInstanceName, transcode});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostTranscodeApis.SetVhostTranscode(deviceId, vhostDomain, transcodeInstanceName, transcode,
                out var rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}