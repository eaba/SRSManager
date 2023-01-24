using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
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

            var a = await _actor.Ask<ApisResult>(new FastUseful(deviceId, vhostDomain, ingestName, "GetStreamInfoByVhostIngestName"));
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
        public async ValueTask<JsonResult> GetAllIngestByDeviceId(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new FastUseful(deviceId, "GetAllIngestByDeviceId"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// Set a vhost to low latency mode/normal mode
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/OnOrOffVhostMinDelay")]
        public async ValueTask<JsonResult> OnOrOffVhostMinDelay(string deviceId, string vhostDomain, bool enable)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, vhostDomain, enable});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new FastUseful(deviceId, vhostDomain, enable, "OnOrOffVhostMinDelay"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
                
        /// <summary>
        /// Obtain camera connection information through the value of stream
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetClientInfoByStreamValue")]
        public async ValueTask<JsonResult> GetClientInfoByStreamValue(string streamId, string tenant, string nameSpace, string trinoUrl)
        {
            var rss = CommonFunctions.CheckParams(new object[] {streamId, tenant, nameSpace, trinoUrl});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new GetClientInfoByStreamValue(streamId, tenant, nameSpace, trinoUrl));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get information about all running srs instances
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetRunningSrsInfoList")]
        public async ValueTask<JsonResult> GetRunningSrsInfoList()
        {
           
            var a = await _actor.Ask<ApisResult>(SRSManager.Messages.GetRunningSrsInfoList.Instance);
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Stop all running srs instances
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/StopAllSrs")]
        public async ValueTask<JsonResult> StopAllSrs()
        {
            var a = await _actor.Ask<ApisResult>(SRSManager.Messages.StopAllSrs.Instance);
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Start all unstarted srs instances
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/InitAndStartAllSrs")]
        public async ValueTask<JsonResult> InitAndStartAllSrs()
        {
            var a = await _actor.Ask<ApisResult>(SRSManager.Messages.InitAndStartAllSrs.Instance);
            return Result.DelApisResult(a.Rt, a.Rs);
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
        public async ValueTask<JsonResult> KickoffClient(string deviceId, string clientId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, clientId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

           
            var a = await _actor.Ask<ApisResult>(new FastUseful(id:"", deviceId: deviceId, streamId:"", clientId: clientId, "KickoffClient"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// Get the status information of Stream in SRS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetStreamStatusById")]
        public async ValueTask<JsonResult> GetStreamStatusById(string deviceId, string streamId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, streamId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            
            var a = await _actor.Ask<ApisResult>(new FastUseful(id: "", deviceId: deviceId, streamId: streamId, clientId: "", "GetStreamStatusByDeviceIdAndStreamId"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// Get the status information of StreamList in SRS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetStreamListStatusByDeviceId")]
        public async ValueTask<JsonResult> GetStreamListStatusByDeviceId(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new FastUseful(deviceId, "GetStreamListStatusByDeviceId"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Obtain the status information of Vhost in SRS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetVhostStatusById")]
        public async ValueTask<JsonResult> GetVhostStatusById(string deviceId, string vhostId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {vhostId, deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new FastUseful(deviceId, vhostId, "GetVhostStatusById"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// Obtain the status information of VhostList in SRS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetVhostListStatusByDeviceId")]
        public async ValueTask<JsonResult> GetVhostListStatusByDeviceId(string deviceId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new FastUseful(deviceId, "GetVhostListStatusByDeviceId"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// Obtain the online player client through the srs instance id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnOnlinePlayerByDeviceId")]
        public async ValueTask<JsonResult> GetOnOnlinePlayerByDeviceId(string deviceId, string tenant, string nameSpace, string trinoUrl)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, tenant, nameSpace, trinoUrl});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            var a = await _actor.Ask<ApisResult>(new GetOnlinePlayerByDeviceId(deviceId, tenant, nameSpace, trinoUrl));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// Get Streaming Client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnlinePlayer")]
        public async ValueTask<JsonResult> GetOnlinePlayer(string tenant, string nameSpace, string trinoUrl)
        {
            var rss = CommonFunctions.CheckParams(new object[] { tenant, nameSpace, trinoUrl });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            
            var a = await _actor.Ask<ApisResult>(new GetOnlinePlayer(tenant, nameSpace, trinoUrl));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get the list of online cameras through the srs instance id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnPublishMonitorListById")]
        public async ValueTask<JsonResult> GetOnPublishMonitorListById(string deviceId, string tenant, string nameSpace, string trinoUrl)
        {
            var rss = CommonFunctions.CheckParams(new object[] {deviceId, tenant, nameSpace, trinoUrl});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new GetOnPublishMonitorListById(deviceId, tenant, nameSpace, trinoUrl));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get a list of online cams
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnPublishMonitorList")]
        public async ValueTask<JsonResult> GetOnPublishMonitorList(string tenant, string nameSpace, string trinoUrl)
        {
            var rss = CommonFunctions.CheckParams(new object[] { tenant, nameSpace, trinoUrl });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            
            var a = await _actor.Ask<ApisResult>(new GetOnPublishMonitorList(tenant, nameSpace, trinoUrl));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get online camera ById, support multiple ids, separated by spaces
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnPublishMonitorById")]
        public async ValueTask<JsonResult> GetOnPublishMonitorById(string id, string tenant, string nameSpace, string trinoUrl)
        {
            var rss = CommonFunctions.CheckParams(new object[] {id, tenant, nameSpace, trinoUrl});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new GetOnPublishMonitorById(id, tenant, nameSpace, trinoUrl));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// Get an ingest configuration for rtsp streaming
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/FastUseful/GetOnvifMonitorIngestTemplate")]
        public async ValueTask<JsonResult> GetOnvifMonitorIngestTemplate(string? username, string? password, string rtspUrl)
        {
            var rss = CommonFunctions.CheckParams(new object[] { rtspUrl});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new GetOnvifMonitorIngestTemplate(username!, password!, rtspUrl));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
    }
}