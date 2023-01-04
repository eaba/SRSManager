using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// SRSRtcServer device interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class SrtServerController : ControllerBase
    {
        /// <summary>
        /// Get srtserver configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/SrtServer/GetSrtServer")]
        public JsonResult GetSrtServer(string deviceId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = SrtServerApis.GetSrtServer(deviceId, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Set up or create srtserver
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/SrtServer/SetSrtServer")]
        public JsonResult SetSrsSrtServer(string deviceId, SrsSrtServerConfClass srt)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {srt, deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = SrtServerApis.SetSrtServer(deviceId, srt, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// delete srtserver
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/SrtServer/DelSrtServer")]
        public JsonResult DelSrsSrtServer(string deviceId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = SrtServerApis.DeleteSrtServer(deviceId, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}