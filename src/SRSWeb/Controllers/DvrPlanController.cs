using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.RequestModules;
using SRSManageCommon.ManageStructs;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// Authorized Access Interface Class
    /// </summary>
    [ApiController]
    [Route("")]
    public class DvrPlanController : ControllerBase
    {

        /// <summary>
        /// Get Merge Crop Task Backlog List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/GetBacklogTaskList")]
        public JsonResult GetBacklogTaskList()
        {
            var rt = DvrPlanApis.GetBacklogTaskList( out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Get the progress information of the merge clip task
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/GetMergeTaskStatus")]
        public JsonResult GetMergeTaskStatus(string taskId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {taskId});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = DvrPlanApis.GetMergeTaskStatus(taskId, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Cut and merge video files, if the callbackurl is empty and the time interval does not exceed 10 minutes, it will return synchronously, otherwise, the callbackurl address will be called back asynchronously after completion
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/CutOrMergeVideoFile")]
        public JsonResult CutOrMergeVideoFile(ReqCutOrMergeVideoFile rcmv)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {rcmv});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = DvrPlanApis.CutOrMergeVideoFile(rcmv, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Recover soft-deleted video files
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/UndoSoftDelete")]
        public JsonResult UndoSoftDelete(long dvrVideoId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {dvrVideoId});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = DvrPlanApis.UndoSoftDelete(dvrVideoId, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Delete a video file ById (hard delete, delete the file immediately, mark the database as delete)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/HardDeleteDvrVideoById")]
        public JsonResult HardDeleteDvrVideoById(long dvrVideoId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {dvrVideoId});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = DvrPlanApis.HardDeleteDvrVideoById(dvrVideoId, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Delete a video file ById (soft delete, only mark, do not delete the file, the file will be deleted after 24 hours)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/SoftDeleteDvrVideoById")]
        public JsonResult SoftDeleteDvrVideoById(long dvrVideoId)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {dvrVideoId});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = DvrPlanApis.SoftDeleteDvrVideoById(dvrVideoId, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get video files (conditions are flexible)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/GetDvrVideoList")]
        public JsonResult GetDvrVideoList(ReqGetDvrVideo rgdv)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {rgdv});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = DvrPlanApis.GetDvrVideoList(rgdv, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Delete a recording plan ById
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/DeleteDvrPlanById")]
        public JsonResult DeleteDvrPlanById(long id)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {id});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = DvrPlanApis.DeleteDvrPlanById(id, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Enable or disable a recording schedule
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/OnOrOffDvrPlanById")]
        public JsonResult OnOrOffDvrPlanById(long id, bool enable)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {id, enable});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = DvrPlanApis.OnOrOffDvrPlanById(id, enable, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
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
        public JsonResult SetDvrPlanById(int id,ReqStreamDvrPlan sdp)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {id,sdp});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = DvrPlanApis.SetDvrPlanById(id,sdp, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Create a recording schedule
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/DvrPlan/CreateDvrPlan")]
        public JsonResult CreateDvrPlan(ReqStreamDvrPlan sdp)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {sdp});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = DvrPlanApis.CreateDvrPlan(sdp, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
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
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }
            var rt = DvrPlanApis.GetDvrPlanById(id,out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
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
        public JsonResult GetDvrPlanList(ReqGetDvrPlan rdp)
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {rdp});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = DvrPlanApis.GetDvrPlanList(rdp, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }
    }
}