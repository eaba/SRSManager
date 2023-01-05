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
    /// vhosthttpstatic interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostHttpStaticController
    {
        /// <summary>
        /// Delete the HttpStatic configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpStatic/DeleteVhostHttpStatic")]
        public JsonResult DeleteVhostHttpStatic(string deviceId, string vhostDomain)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostHttpStaticApis.DeleteVhostHttpStatic(deviceId, vhostDomain, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get HttpStatic in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpStatic/GetVhostHttpStatic")]
        public JsonResult GetVhostHttpStatic(string deviceId, string vhostDomain)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostHttpStaticApis.GetVhostHttpStatic(deviceId, vhostDomain, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Set or create HttpStatic
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="httpStatic"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpStatic/SetVhostHttpStatic")]
        public JsonResult SetVhostHttpStatic(string deviceId, string vhostDomain, HttpStatic httpStatic)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, httpStatic});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostHttpStaticApis.SetVhostHttpStatic(deviceId, vhostDomain, httpStatic, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}