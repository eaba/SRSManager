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
    /// vhostrtc interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostRtcController
    {
        /// <summary>
        /// Delete Rtc configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostRtc/DeleteVhostRtc")]
        public JsonResult DeleteVhostRtc(string deviceId, string vhostDomain)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostRtcApis.DeleteVhostRtc(deviceId, vhostDomain, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get Rtc in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostRtc/GetVhostRtc")]
        public JsonResult GetVhostRtc(string deviceId, string vhostDomain)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostRtcApis.GetVhostRtc(deviceId, vhostDomain, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Set or create Rtc
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="rtc"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostRtc/SetVhostRtc")]
        public JsonResult SetVhostRtc(string deviceId, string vhostDomain, Rtc rtc)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, rtc});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostRtcApis.SetVhostRtc(deviceId, vhostDomain, rtc, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}