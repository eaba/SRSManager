﻿using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// vhosthttphooks interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class VhostHttpHooksController
    {
        /// <summary>
        /// Delete HttpHooks configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpHooks/DeleteVhostHttpHooks")]
        public JsonResult DeleteVhostHttpHooks(string deviceId, string vhostDomain)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = VhostHttpHooksApis.DeleteVhostHttpHooks(deviceId, vhostDomain, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get HttpHooks in Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpHooks/GetVhostHttpHooks")]
        public JsonResult GetVhostHttpHooks(string deviceId, string vhostDomain)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = VhostHttpHooksApis.GetVhostHttpHooks(deviceId, vhostDomain, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Set or create HttpHooks
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="httpHooks"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/VhostHttpHooks/SetVhostHttpHooks")]
        public JsonResult SetVhostHttpHooks(string deviceId, string vhostDomain, HttpHooks httpHooks)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, httpHooks});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = VhostHttpHooksApis.SetVhostHttpHooks(deviceId, vhostDomain, httpHooks, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }
    }
}