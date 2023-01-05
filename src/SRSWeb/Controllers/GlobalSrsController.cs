using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.RequestModules;
using SRSManageCommon.ManageStructs;
using SRSManager.Actors;
using SRSManager.Messages;
using SRSManager.Shared;
using System.Security.Claims;
using SRSWeb.Attributes;

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
        public GlobalSrsController(IRequiredActor<SRSManagersActor> actor)
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
        public async ValueTask<JsonResult> IsRunning(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
           // var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, "IsRunning"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Whether srs is initialized
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/IsInit")]
        public async ValueTask<JsonResult> IsInit(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, "IsInit"));
            return Result.DelApisResult(a.Rt, a.Rs);
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
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, "Start"));
            return Result.DelApisResult(a.Rt, a.Rs);
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
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, "Stop"));
            return Result.DelApisResult(a.Rt, a.Rs);
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
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, "Restart"));
            return Result.DelApisResult(a.Rt, a.Rs);
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
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, "Reload"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter Chunksize
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeChunksize")]
        public async ValueTask<JsonResult> GlobalChangeChunksize(string deviceId, ushort chunkSize)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, chunkSize });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, chunkSize, "ChangeChunksize"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter HttpApiListen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpApiListen")]
        public async ValueTask<JsonResult> GlobalChangeHttpApiListen(string deviceId, ushort port)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, port });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, port, "ChangeHttpApiListen"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter HttpApiEnable
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpApiEnable")]
        public async ValueTask<JsonResult> GlobalChangeHttpApiEnable(string deviceId, bool enable)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, enable });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, enable, "ChangeHttpApiEnable"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter Maxconnections
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeMaxConnections")]
        public async ValueTask<JsonResult> GlobalChangeMaxConnections(string deviceId, ushort max)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, max });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, max, "ChangeMaxConnections"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter rtmp listen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeRtmpListen")]
        public async ValueTask<JsonResult> GlobalChangeRtmpListen(string deviceId, ushort port)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, port });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, port, "ChangeRtmpListen"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter Httpserver listen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpServerListen")]
        public async ValueTask<JsonResult> GlobalChangeHttpServerListen(string deviceId, ushort port)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId , port });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, port, "ChangeHttpServerListen"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter HttpserverPath
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpServerPath")]
        public async ValueTask<JsonResult> GlobalChangeHttpServerPath(string deviceId, string path)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, path });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, path, "ChangeHttpServerPath"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameter Httpserver enable
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GlobalChangeHttpServerEnable")]
        public async ValueTask<JsonResult> GlobalChangeHttpServerEnable(string deviceId, bool enable)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId, enable });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, enable, "ChangeHttpServerEnable"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get the global parameters of the srs instance
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/GetGlobalParams")]
        public async ValueTask<JsonResult> GetGlobalParams(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] { deviceId });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(deviceId, "GetGlobalParams"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Modify the global parameters of the srs instance
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/GlobalSrs/ChangeGlobalParams")]
        public async ValueTask<JsonResult> ChangeGlobalParams(ReqChangeSrsGlobalParams req)
        {
            var rss = CommonFunctions.CheckParams(new object[] { req });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<DelApisResult>(new GlobalSrs(req.Gm, "ChangeGlobalParams"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
    }
}