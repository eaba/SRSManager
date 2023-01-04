using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using SRSWeb.Attributes;
using Hls = SrsConfFile.SRSConfClass.Hls;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// vhosthls interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostHlsController
    {
        /// <summary>
        /// Delete HLS configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHls/DeleteVhostHls")]
        public JsonResult DeleteVhostHls(string deviceId, string vhostDomain)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostHlsApis.DeleteVhostHls(deviceId, vhostDomain, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get Hls in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHls/GetVhostHls")]
        public JsonResult GetVhostHls(string deviceId, string vhostDomain)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostHlsApis.GetVhostHls(deviceId, vhostDomain, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Set or create Hls
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="hostHls"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostHls/SetVhostHls")]
        public JsonResult SetVhostHls(string deviceId, string vhostDomain, Hls hostHls)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, hostHls});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostHlsApis.SetVhostHls(deviceId, vhostDomain, hostHls, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}