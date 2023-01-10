using System.Net;
using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.ResponseModules;
using SRSManageCommon.ManageStructs;
using SRSManager.Actors;
using SRSManager.Messages;
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
        private readonly IActorRef _actor;
        public SystemController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Write the configuration file of the SRS instance to disk
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/RefreshSrsObject")]
        public async ValueTask<JsonResult> RefreshSrsObject(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new SRSManager.Messages.System(deviceId, "RefreshSrsObject"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }


        /// <summary>
        /// Get deviceid in all Srs managers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/GetAllSrsManagerDeviceId")]
        public async ValueTask<JsonResult> GetAllSrsManagerDeviceId()
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            
            var rt = await _actor.Ask<List<string>>(SRSManager.Messages.GetAllSrsManagerDeviceId.Instance);            
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
        public async ValueTask<JsonResult> CreateNewSrsInstance(SrsManager sm)
        {
            var rss = CommonFunctions.CheckParams(new object[] {sm});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new SRSManager.Messages.System(sm, "CreateNewSrsInstance"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }


        /// <summary>
        /// Get the SRS instance template
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/GetSrsInstanceTemplate")]
        public async ValueTask<JsonResult> GetSrsInstanceTemplate()
        {
            var rt = await _actor.Ask<ApisResult>(SRSManager.Messages.GetSrsInstanceTemplate.Instance);
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }


        /// <summary>
        /// Delete an SRS instance
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/DelSrsByDevId")]
        public async ValueTask<JsonResult> DelSrsInstanceByDeviceId(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new SRSManager.Messages.System(deviceId, "DelSrsInstanceByDeviceId"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        /// <summary>
        /// Get the SRS instance according to DeviceID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/GetSrsInstanceByDeviceId")]
        public async ValueTask<JsonResult> GetSrsInstanceByDeviceId(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = await _actor.Ask<ApisResult>(new SRSManager.Messages.System(deviceId, "GetSrsInstanceByDeviceId"));
            return Result.DelApisResult(rt.Rt, rt.Rs);
        }

        
        /// <summary>
        /// Get system information
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/System/GetSystemInfo")]
        public async ValueTask<JsonResult> GetSystemInfo()
        {
            var info = await _actor.Ask<SystemInfoModule>(SRSManager.Messages.GetSystemInfo.Instance);
            var result = new JsonResult(info);
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
        public async ValueTask<JsonResult> GetSrsInstanceList()
        {
            var devs = await _actor.Ask<List<string>>(SRSManager.Messages.GetAllSrsManagerDeviceId.Instance);
            var simlist = new List<SrsInstanceModule>();
            foreach (var dev in devs)
            {
                var srs = await _actor.Ask<SrsManager>(new SRSManager.Messages.System(dev, "GetSrsManagerInstanceByDeviceId"));
                if (srs != null)
                {
                    var sim = new SrsInstanceModule()
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