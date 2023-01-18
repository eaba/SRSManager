using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using SRSManager.Actors;
using SRSManager.Messages;
using SRSManager.Shared;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// Quick use of interface classes
    /// </summary>
    [ApiController]
    [Route("")]
    public class FastUsefulController : ControllerBase
    {
        private readonly IActorRef _actor;
        public FastUsefulController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// TESTTEST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/FastUseful/Test")]
        public int Test(CutMergeTaskResponse obj)
        {
            Console.WriteLine("Here is the test output:" + JsonHelper.ToJson(obj));
            return 0;
        }


        /// <summary>
        /// Get Flow Information ByIngestName
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetStreamInfoByVhostIngestName")]
        public async ValueTask<JsonResult> GetStreamInfoByVhostIngestName(string deviceId, string vhostDomain, string ingestName)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, ingestName});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.GetStreamInfoByVhostIngestName(deviceId, vhostDomain, ingestName,
                out var rs);
            var a = await _actor.Ask<ApisResult>(new GlobalSrs(deviceId, "IsRunning"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get Ingest list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetAllIngestByDeviceId")]
        public JsonResult GetAllIngestByDeviceId(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.GetAllIngestByDeviceId(deviceId, out var rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Set a vhost to low latency mode/normal mode
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/OnOrOffVhostMinDelay")]
        public JsonResult OnOrOffVhostMinDelay(string deviceId, string vhostDomain, bool enable)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, enable});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.OnOrOffVhostMinDelay(deviceId, vhostDomain, enable, out var rs);
            return Result.DelApisResult(rt, rs);
        }
                
        /// <summary>
        /// Obtain camera connection information through the value of stream
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetClientInfoByStreamValue")]
        public JsonResult GetClientInfoByStreamValue(string stream)
        {
            var rss = CommonFunctions.CheckParams(new object[] {stream});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.GetClientInfoByStreamValue(stream, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get information about all running srs instances
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetRunningSrsInfoList")]
        public JsonResult GetRunningSrsInfoList()
        {
            var rt = FastUsefulApis.GetRunningSrsInfoList(out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Stop all running srs instances
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/StopAllSrs")]
        public JsonResult StopAllSrs()
        {
            var rt = FastUsefulApis.StopAllSrs(out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Start all unstarted srs instances
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/InitAndStartAllSrs")]
        public JsonResult InitAndStartAllSrs()
        {
            var rt = FastUsefulApis.InitAndStartAllSrs(out var rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Kick a camera or a player from the Client list
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/KickoffClient")]
        public JsonResult KickoffClient(string deviceId, string clientId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, clientId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.KickoffClient(deviceId, clientId, out var rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Get the status information of Stream in SRS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetStreamStatusById")]
        public JsonResult GetStreamStatusById(string deviceId, string streamId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, streamId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.GetStreamStatusByDeviceIdAndStreamId(deviceId, streamId, out var rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Get the status information of StreamList in SRS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetStreamListStatusByDeviceId")]
        public JsonResult GetStreamListStatusByDeviceId(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.GetStreamListStatusByDeviceId(deviceId, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Obtain the status information of Vhost in SRS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetVhostStatusById")]
        public JsonResult GetVhostStatusById(string deviceId, string vhostId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {vhostId, deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.GetVhostStatusByDeviceIdAndVhostId(deviceId, vhostId, out var rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Obtain the status information of VhostList in SRS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetVhostListStatusByDeviceId")]
        public JsonResult GetVhostListStatusByDeviceId(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.GetVhostListStatusByDeviceId(deviceId, out var rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Obtain the online player client through the srs instance id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnOnlinePlayerByDeviceId")]
        public JsonResult GetOnOnlinePlayerByDeviceId(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.GetOnlinePlayerByDeviceId(deviceId, out var rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Get Streaming Client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnOnlinePlayer")]
        public JsonResult GetOnOnlinePlayer()
        {
            var rt = FastUsefulApis.GetOnlinePlayer(out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get the list of online cameras through the srs instance id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnPublishMonitorListById")]
        public JsonResult GetOnPublishMonitorListById(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.GetOnPublishMonitorListByDeviceId(deviceId, out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get a list of online cams
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnPublishMonitorList")]
        public JsonResult GetOnPublishMonitorList()
        {
            var rt = FastUsefulApis.GetOnPublishMonitorList(out var rs);
            return Result.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get online camera ById, support multiple ids, separated by spaces
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnPublishMonitorById")]
        public JsonResult GetOnPublishMonitorById(string id)
        {
            var rss = CommonFunctions.CheckParams(new object[] {id});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.GetOnPublishMonitorById(id, out var rs);
            return Result.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Get an ingest configuration for rtsp streaming
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnvifMonitorIngestTemplate")]
        public JsonResult GetOnvifMonitorIngestTemplate(string? username, string? password, string rtspUrl)
        {
            var rss = CommonFunctions.CheckParams(new object[] { rtspUrl});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var rt = FastUsefulApis.GetOnvifMonitorIngestTemplate(username!, password!, rtspUrl, out var rs);
            return Result.DelApisResult(rt, rs);
        }
    }
}