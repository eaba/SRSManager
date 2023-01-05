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
    /// vhosthttpremux interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostHttpRemux
    {
        /// <summary>
        /// Delete HttpRemux configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpRemux/DeleteVhostHttpRemux")]
        public JsonResult DeleteVhostHttpRemux(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostHttpRemuxApis.DeleteVhostHttpRemux(deviceId, vhostDomain, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get HttpRemux in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpRemux/GetVhostHttpRemux")]
        public JsonResult GetVhostHttpRemux(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostHttpRemuxApis.GetVhostHttpRemux(deviceId, vhostDomain, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Set up or create HttpRemux
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="httpRemux"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpRemux/SetVhostHttpRemux")]
        public JsonResult SetVhostHttpRemux(string deviceId, string vhostDomain, HttpRemux httpRemux)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, httpRemux});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostHttpRemuxApis.SetVhostHttpRemux(deviceId, vhostDomain, httpRemux, out var rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}