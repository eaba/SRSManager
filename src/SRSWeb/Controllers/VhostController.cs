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
    /// vhost control class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostController
    {
        /// <summary>
        /// Obtain the list of Instance names of the Vhost list
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Log]
        [Route("/Vhost/GetVhostsInstanceName")]
        public JsonResult GetVhostsInstanceName(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostApis.GetVhostsInstanceName(deviceId, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get vhost by domain
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Vhost/GetVhostByDomain")]
        public JsonResult GetVhostByDomain(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostApis.GetVhostByDomain(deviceId, vhostDomain, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get list of Vhosts
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Vhost/GetVhostList")]
        public JsonResult GetVhostList(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostApis.GetVhostList(deviceId, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get various templates of Vhost [0:Stream] [1:File] [2:Device]
        /// </summary>
        /// <param name="vtype"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Vhost/GetVhostTemplate")]
        public JsonResult GetVhostTemplate(VhostIngestInputType vtype)
        {
            var rss = CommonFunctions.CheckParams(new object[] {vtype});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostApis.GetVhostTemplate(vtype, out var rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Set or create Vhost parameters
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhost"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/Vhost/SetVhost")]
        public JsonResult SetVhost(string deviceId, SrsvHostConfClass vhost)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhost});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostApis.SetVhost(deviceId, vhost, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Delete a vhost, use the domain name
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Vhost/DeleteVhostByDomain")]
        public JsonResult DeleteVhostByDomain(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostApis.DeleteVhostByDomain(deviceId, vhostDomain, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Modify the domain name of the vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="newVhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Vhost/ChangeVhostDomain")]
        public JsonResult ChangeVhostDomain(string deviceId, string vhostDomain, string newVhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, newVhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostApis.ChangeVhostDomain(deviceId, vhostDomain, newVhostDomain, out var rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}