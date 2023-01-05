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
    /// vhostexec interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostExecController
    {
        /// <summary>
        /// Delete Exec configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostExec/DeleteVhostExec")]
        public JsonResult DeleteVhostExec(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostExecApis.DeleteVhostExec(deviceId, vhostDomain, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get Exec in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostExec/GetVhostExec")]
        public JsonResult GetVhostExec(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostExecApis.GetVhostExec(deviceId, vhostDomain, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Set up or create Exec
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="exec"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostExec/SetVhostExec")]
        public JsonResult SetVhostExec(string deviceId, string vhostDomain, Exec exec)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, exec});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostExecApis.SetVhostExec(deviceId, vhostDomain, exec, out var rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}