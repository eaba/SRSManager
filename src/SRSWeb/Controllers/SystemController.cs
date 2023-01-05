using System.Net;
using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.ResponseModules;
using SRSManageCommon.ManageStructs;
using SRSManager.Shared;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// System information interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class SystemController : ControllerBase
    {
        /// <summary>
        /// Write the configuration file of the SRS instance to disk
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/RefreshSrsObject")]
        public JsonResult RefreshSrsObject(string deviceId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = SystemApis.RefreshSrsObject(deviceId, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Get deviceid in all Srs managers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/GetAllSrsManagerDeviceId")]
        public JsonResult GetAllSrsManagerDeviceId()
        {
            ResponseStruct rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var rt = SystemApis.GetAllSrsManagerDeviceId();
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Create a SrsInstance
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/System/CreateNewSrsInstance")]
        public JsonResult CreateNewSrsInstance(SrsManager sm)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {sm});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = SystemApis.CreateNewSrsInstance(sm, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Get the SRS instance template
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/GetSrsInstanceTemplate")]
        public JsonResult GetSrsInstanceTemplate()
        {
            var rt = SystemApis.GetSrsInstanceTemplate(out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Delete an SRS instance
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/DelSrsByDevId")]
        public JsonResult DelSrsInstanceByDeviceId(string deviceId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = SystemApis.DelSrsInstanceByDeviceId(deviceId, out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get the SRS instance according to DeviceID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/GetSrsInstanceByDeviceId")]
        public JsonResult GetSrsInstanceByDeviceId(string deviceId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            ResponseStruct rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var rt = SystemApis.GetSrsManagerInstanceByDeviceId(deviceId);
            return Result.DelApisResult(rt, rs);
        }

        
        /// <summary>
        /// Get system information
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/GetSystemInfo")]
        public JsonResult GetSystemInfo()
        {
            var result = new JsonResult(SystemApis.GetSystemInfo());
            result.StatusCode = (int) HttpStatusCode.OK;
            return result;
        }

        /// <summary>
        /// Get the list of SRS instances
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/GetSrsInstanceList")]
        public JsonResult GetSrsInstanceList()
        {
            List<string> devs = SystemApis.GetAllSrsManagerDeviceId();
            List<SrsInstanceModule> simlist = new List<SrsInstanceModule>();
            foreach (var dev in devs)
            {
                SrsManager srs = SystemApis.GetSrsManagerInstanceByDeviceId(dev);
                if (srs != null)
                {
                    SrsInstanceModule sim = new SrsInstanceModule()
                    {
                        ConfigPath = srs.SrsConfigPath,
                        DeviceId = srs.SrsDeviceId,
                        IsInit = srs.IsInit,
                        IsRunning = srs.IsRunning,
                        PidValue = srs.SrsPidValue,
                        SrsInstanceWorkPath = srs.SrsWorkPath,
                        SrsProcessWorkPath = srs.SrsWorkPath + "srs",
                    };
                    simlist.Add(sim);
                }
            }

            var result = new JsonResult(simlist);
            result.StatusCode = (int) HttpStatusCode.OK;
            return result;
        }
    }
}