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
    /// SRSRtcServer device interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class RtcServerController : ControllerBase
    {
        /// <summary>
        /// Get rtcserver configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/RtcServer/GetSrsRtcServer")]
        public JsonResult GetSrsRtcServer(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = RtcServerApis.GetRtcServer(deviceId, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Set or create rtcserver
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/RtcServer/SetRtcServer")]
        public JsonResult SetSrsRtcServer(string deviceId, SrsRtcServerConfClass rtc)
        {
            var rss = CommonFunctions.CheckParams(new object[] {rtc});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = RtcServerApis.SetRtcServer(deviceId, rtc, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// delete rtcserver
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/RtcServer/DelRtcServer")]
        public JsonResult DelSrsRtcServer(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = RtcServerApis.DeleteRtcServer(deviceId, out var rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}