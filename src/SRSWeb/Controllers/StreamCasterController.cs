using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// SRSStreamCaster interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class StreamCasterController : ControllerBase
    {
        /// <summary>
        /// Get all StreamCaster instance names
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/GetStreamCasterInstanceNameList")]
        public JsonResult GetStreamCasterInstanceNameList(string deviceId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = StreamCasterApis.GetStreamCastersInstanceName(deviceId, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get all instances of StreamCaster
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/GetStreamCasterInstanceList")]
        public JsonResult GetStreamCasterInstanceList(string deviceId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = StreamCasterApis.GetStreamCasterList(deviceId, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Create an instance of StreamCaster
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/CreateStreamCaster")]
        public JsonResult CreateStreamCaster(string deviceId, SrsStreamCasterConfClass streamcaster)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, streamcaster});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = StreamCasterApis.CreateStreamCaster(deviceId, streamcaster, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get the StreamCaster template
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/GetStreamCasterTemplate")]
        public JsonResult GetStreamCasterTemplate(CasterEnum casterType)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {casterType});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = StreamCasterApis.GetStreamCasterTemplate(casterType, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Delete a streamcaster with instance name
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/DeleteStreamCasterByInstanceName")]
        public JsonResult DeleteStreamCasterByInstanceName(string deviceId, string instanceName)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, instanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = StreamCasterApis.DeleteStreamCasterByInstanceName(deviceId, instanceName, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Modify the instance name of streamcaster
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/ChangeStreamCasterInstanceName")]
        public JsonResult ChangeStreamCasterInstanceName(string deviceId, string instanceName, string newInstanceName)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, instanceName, newInstanceName});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = StreamCasterApis.ChangeStreamCasterInstanceName(deviceId, instanceName, newInstanceName,
                out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Stop or start a StreamCaster
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/OnOrOff")]
        public JsonResult OnOrOff(string deviceId, string instanceName, bool enable)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, instanceName, enable});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = StreamCasterApis.OnOrOffStreamCaster(deviceId, instanceName, enable, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Setting up a StreamCaster
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/StreamCaster/SetStreamCaster")]
        public JsonResult SetStreamCaster(string deviceId, SrsStreamCasterConfClass streamcaster)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, streamcaster});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = StreamCasterApis.SetStreamCaster(deviceId, streamcaster, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }
    }
}