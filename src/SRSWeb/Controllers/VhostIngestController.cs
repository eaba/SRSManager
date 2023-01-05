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
    /// vhostingest interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostIngestController
    {
        /// <summary>
        /// Get IngestList by deviceId, vhostDomain
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/GetVhostIngestList")]
        public JsonResult GetVhostIngestList(string deviceId, string vhostDomain)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostIngestApis.GetVhostIngestList(deviceId, vhostDomain,
                out var rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Delete an Ingest by VhostDomain and IngestInstanceName
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="ingestInstanceName"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/DeleteVhostIngestByIngestInstanceName")]
        public JsonResult DeleteVhostIngestByIngestInstanceName(string deviceId, string vhostDomain,
            string ingestInstanceName)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, ingestInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostIngestApis.DeleteVhostIngestByIngestInstanceName(deviceId, vhostDomain, ingestInstanceName,
                out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get all or specified ingest instance names in vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/GetVhostIngestNameList")]
        public JsonResult GetVhostIngestNameList(string deviceId, string vhostDomain = "")
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostIngestApis.GetVhostIngestNameList(deviceId, out var rs, vhostDomain);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get an Ingest configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="ingestInstanceName"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/GetVhostIngest")]
        public JsonResult GetVhostIngest(string deviceId, string vhostDomain, string ingestInstanceName)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, ingestInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostIngestApis.GetVhostIngest(deviceId, vhostDomain, ingestInstanceName, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Set up or create an Ingest
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="ingest"></param>
        /// <param name="ingestInstanceName"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/SetVhostIngest")]
        public JsonResult SetVhostIngest(string deviceId, string vhostDomain, string ingestInstanceName, Ingest ingest)
        {
            var rss = CommonFunctions.CheckParams(new object[]
                {deviceId, vhostDomain, ingest, ingestInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostIngestApis.SetVhostIngest(deviceId, vhostDomain, ingestInstanceName, ingest,
                out var rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// enable or disable an ingest
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="ingestInstanceName"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostIngest/OnOrOffIngest")]
        public JsonResult OnOrOffIngest(string deviceId, string vhostDomain, string ingestInstanceName, bool enable)
        {
            var rss = CommonFunctions.CheckParams(new object[]
                {deviceId, vhostDomain, enable, ingestInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = VhostIngestApis.OnOrOffIngest(deviceId, vhostDomain, ingestInstanceName, enable,
                out var rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}