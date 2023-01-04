using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.RequestModules;
using SRSManageCommon.ManageStructs;
using SRSWeb.Actors;
using SRSWeb.Attributes;
using SRSWeb.Messages;
using System.Security.Cryptography;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// Global SRS interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class GlobalSrsController : ControllerBase
    {
        private readonly IActorRef _actor;
        public GlobalSrsController(IRequiredActor<GlobalSrsApisActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Is srs running
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/IsRunning")]
        public JsonResult IsRunning(string deviceId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.IsRunning(deviceId, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Whether srs is initialized
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/IsInit")]
        public JsonResult IsInit(string deviceId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.IsInit(deviceId, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// start srs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/StartSrs")]
        public async ValueTask<JsonResult> StartSrs(string deviceId)
        {           
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, "StartSrs"));
            return Program.CommonFunctions.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// stop srs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/StopSrs")]
        public async ValueTask<JsonResult> StopSrs(string deviceId)
        {
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, "StopSrs"));
            return Program.CommonFunctions.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// restart srs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/RestartSrs")]
        public async ValueTask<JsonResult> RestartSrs(string deviceId)
        {           
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, "RestartSrs"));
            return Program.CommonFunctions.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Reload the srs configuration (srs.reload)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/ReloadSrs")]
        public async ValueTask<JsonResult> ReloadtSrs(string deviceId)
        {
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, "ReloadSrs"));
            return Program.CommonFunctions.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter Chunksize
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeChunksize")]
        public JsonResult GlobalChangeChunksize(string deviceId, ushort chunkSize)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, chunkSize});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.GlobalChangeChunksize(deviceId, chunkSize, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Modify the global parameter HttpApiListen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpApiListen")]
        public JsonResult GlobalChangeHttpApiListen(string deviceId, ushort port)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, port});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.GlobalChangeHttpApipListen(deviceId, port, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Modify the global parameter HttpApiEnable
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpApiEnable")]
        public JsonResult GlobalChangeHttpApiEnable(string deviceId, bool enable)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, enable});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.GlobalChangeHttpApiEnable(deviceId, enable, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Modify the global parameter Maxconnections
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeMaxConnections")]
        public JsonResult GlobalChangeMaxConnections(string deviceId, ushort max)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, max});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.GlobalChangeMaxConnections(deviceId, max, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Modify the global parameter rtmp listen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeRtmpListen")]
        public JsonResult GlobalChangeRtmpListen(string deviceId, ushort port)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, port});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.GlobalChangeRtmpListen(deviceId, port, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Modify the global parameter Httpserver listen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpServerListen")]
        public JsonResult GlobalChangeHttpServerListen(string deviceId, ushort port)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, port});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.GlobalChangeHttpServerListen(deviceId, port, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Modify the global parameter HttpserverPath
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpServerPath")]
        public JsonResult GlobalChangeHttpServerPath(string deviceId, string path)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, path});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.GlobalChangeHttpServerPath(deviceId, path, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Modify the global parameter Httpserver enable
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpServerEnable")]
        public JsonResult GlobalChangeHttpServerEnable(string deviceId, bool enable)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId, enable});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.GlobalChangeHttpServerEnable(deviceId, enable, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get the global parameters of the srs instance
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GetGlobalParams")]
        public JsonResult GetGlobalParams(string deviceId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.GetGlobalParams(deviceId, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Modify the global parameters of the srs instance
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/ChangeGlobalParams")]
        public JsonResult ChangeGlobalParams(ReqChangeSrsGlobalParams req)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {req});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = GlobalSrsApis.ChangeGlobalParams(req.DeviceId, req.Gm, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }
    }
}