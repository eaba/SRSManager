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
    /// vhostplay interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostPlayController
    {
        /// <summary>
        /// Delete Play configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostPlay/DeleteVhostPlay")]
        public JsonResult DeleteVhostPlay(string deviceId, string vhostDomain)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostPlayApis.DeleteVhostPlay(deviceId, vhostDomain, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get Play in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostPlay/GetVhostPlay")]
        public JsonResult GetVhostPlay(string deviceId, string vhostDomain)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostPlayApis.GetVhostPlay(deviceId, vhostDomain, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Set up or create a Play
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="play"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostPlay/SetVhostPlay")]
        public JsonResult SetVhostPlay(string deviceId, string vhostDomain, Play play)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, play});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostPlayApis.SetVhostPlay(deviceId, vhostDomain, play, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}