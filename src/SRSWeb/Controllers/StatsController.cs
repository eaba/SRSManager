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
    public class StatsController : ControllerBase
    {
        /// <summary>
        /// Get Stats configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Stats/GetSrsStats")]
        public JsonResult GetSrsStats(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = StatsApis.GetStats(deviceId, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// set or create stats
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/Stats/SetSrsStats")]
        public JsonResult SetSrsStats(string deviceId, SrsStatsConfClass stats)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, stats});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = StatsApis.SetStatsServer(deviceId, stats, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Delete Stats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Stats/DelStats")]
        public JsonResult DelStats(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = StatsApis.DeleteStats(deviceId, out var rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}