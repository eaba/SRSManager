using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.RequestModules;
using SRSManager.Actors;
using SRSManager.Messages;
using SRSManager.Shared;
using SRSWeb.Attributes;
// Pulsar and Trino
namespace SRSWeb.Controllers
{
    /// <summary>
    /// Authorized Access Interface Class
    /// </summary>
    [ApiController]
    [Route("")]
    public class DvrPlanController : ControllerBase
    {
        private readonly IActorRef _actor;
        public DvrPlanController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }
        /// <summary>
        /// Set Pulsar Client
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/PulsarSrsConfig")]
        public async ValueTask<JsonResult> PulsarSrsConfig(string topic, string tenant, string nameSpace, string brokerUrl, string adminUrl, string trinoUrl)
        {
            var client = new PulsarSrsConfig(topic, tenant, nameSpace, brokerUrl, adminUrl, trinoUrl);
            var rss = CommonFunctions.CheckParams(new object[] { client });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new DvrPlan(client, "PulsarSrsConfig"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        

        /// <summary>
        /// Get Merge Crop Task Backlog List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/GetBacklogTaskList")]
        public async ValueTask<JsonResult> GetBacklogTaskList()
        {
            var a = await _actor.Ask<ApisResult>(new DvrPlan("GetBacklogTaskList"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// Get the progress information of the merge clip task
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/GetMergeTaskStatus")]
        public async ValueTask<JsonResult> GetMergeTaskStatus(string taskId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {taskId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new DvrPlan(taskId, "GetMergeTaskStatus"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// Cut and merge video files, if the callbackurl is empty and the time interval does not exceed 10 minutes, it will return synchronously, otherwise, the callbackurl address will be called back asynchronously after completion
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/CutOrMergeVideoFile")]
        public async ValueTask<JsonResult> CutOrMergeVideoFile(ReqCutOrMergeVideoFile rcmv)
        {
            var rss = CommonFunctions.CheckParams(new object[] {rcmv});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new DvrPlan(rcmv, "CutOrMergeVideoFile"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Recover soft-deleted video files
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/UndoSoftDelete")]
        public async ValueTask<JsonResult> UndoSoftDelete(long dvrVideoId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {dvrVideoId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new DvrPlan(dvrVideoId, "UndoSoftDelete"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Delete a video file ById (hard delete, delete the file immediately, mark the database as delete)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/HardDeleteDvrVideoById")]
        public async ValueTask<JsonResult> HardDeleteDvrVideoById(long dvrVideoId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {dvrVideoId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new DvrPlan(dvrVideoId, "HardDeleteDvrVideoById"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Delete a video file ById (soft delete, only mark, do not delete the file, the file will be deleted after 24 hours)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/SoftDeleteDvrVideoById")]
        public async ValueTask<JsonResult> SoftDeleteDvrVideoById(long dvrVideoId)
        {
            var rss = CommonFunctions.CheckParams(new object[] {dvrVideoId});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new DvrPlan(dvrVideoId, "SoftDeleteDvrVideoById"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get video files (conditions are flexible)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/GetDvrVideoList")]
        public async ValueTask<JsonResult> GetDvrVideoList(ReqGetDvrVideo rgdv)
        {
            var rss = CommonFunctions.CheckParams(new object[] {rgdv});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new DvrPlan(rgdv, "GetDvrVideoList"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Delete a recording plan ById
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/DeleteDvrPlanById")]
        public async ValueTask<JsonResult> DeleteDvrPlanById(long id)
        {
            var rss = CommonFunctions.CheckParams(new object[] {id});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new DvrPlan(id, "DeleteDvrPlanById"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Enable or disable a recording schedule
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/OnOrOffDvrPlanById")]
        public async ValueTask<JsonResult> OnOrOffDvrPlanById(long id, bool enable)
        {
            var rss = CommonFunctions.CheckParams(new object[] {id, enable});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new DvrPlan(id, enable, "OnOrOffDvrPlanById"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }




        /// <summary>
        /// Personalized Recording Plan ById
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sdp"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/SetDvrPlanById")]
        public async ValueTask<JsonResult> SetDvrPlanById(int id,ReqStreamDvrPlan sdp)
        {
            var rss = CommonFunctions.CheckParams(new object[] {id,sdp});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new DvrPlan(id, sdp, "SetDvrPlanById"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Create a recording schedule
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/CreateDvrPlan")]
        public async ValueTask<JsonResult> CreateDvrPlan(ReqStreamDvrPlan sdp)
        {
            var rss = CommonFunctions.CheckParams(new object[] {sdp});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new DvrPlan(sdp, "CreateDvrPlan"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /*
        /// <summary>
        /// Get recording plan ById
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/GetDvrPlanById")]
        public JsonResult GetDvrPlanById(long id)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[]{id});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            var rt = DvrPlanApis.GetDvrPlanById(id,out ResponseStruct rs);
            return Result.DelApisResult(rt, rs);
        }
        */

        /// <summary>
        /// Get Recording Schedule
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/GetDvrPlan")]
        public async ValueTask<JsonResult> GetDvrPlanList(ReqGetDvrPlan rdp)
        {
            var rss = CommonFunctions.CheckParams(new object[] {rdp});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new DvrPlan(rdp, "GetDvrPlanList"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
    }
}