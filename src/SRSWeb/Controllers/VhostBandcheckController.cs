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
    /// vhostbandcheck interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostBandcheckController
    {
        /// <summary>
        /// Delete Bandcheck configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostBandcheck/DeleteVhostBandcheck")]
        public JsonResult DeleteVhostBandcheck(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostBandcheckApis.DeleteVhostBandcheck(deviceId, vhostDomain, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get Bandcheck in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostBandcheck/GetVhostBandcheck")]
        public JsonResult GetVhostBandcheck(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostBandcheckApis.GetVhostBandcheck(deviceId, vhostDomain, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Set up or create a Bandcheck
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="bandcheck"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostBandcheck/SetVhostBandcheck")]
        public JsonResult SetVhostBandcheck(string deviceId, string vhostDomain, Bandcheck bandcheck)
        {
            var rss = CommonFunctions.CheckParams(new object[] {vhostDomain, deviceId, bandcheck});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostBandcheckApis.SetVhostBandcheck(deviceId, vhostDomain, bandcheck, out var rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}