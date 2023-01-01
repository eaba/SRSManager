using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SrsManageCommon.ControllerStructs;
using SRSManageCommon.DBMoudle;
using SRSManageCommon.ManageStructs;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// Live plan interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class LiveBroadcastController : ControllerBase
    {
        /// <summary>
        /// Check whether the push live broadcast connection is in the plan
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/Live/CheckIsLivePlan")]
        public JsonResult CheckIsLivePlan(LiveBroadcastPlan plan )
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {plan});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = LiveBroadcastApis.CheckLivePlan(plan, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }


        /// <summary>
        /// Check if the client connection is a live stream
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/Live/CheckLiveCh")]
        public JsonResult CheckLiveCh(OnlineClient client )
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {client});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = LiveBroadcastApis.CheckIsLiveCh(client, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Delete Live Plan byId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Live/DeleteLivePlanById")]
        public JsonResult DeleteLivePlanById(long id )
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {id});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = LiveBroadcastApis.DeleteLivePlanById(id, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Get a list of live broadcast plans
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/Live/GetLivePlanList")]
        public JsonResult GetLivePlanList(ReqLiveBroadcastPlan rlbp )
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {rlbp});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = LiveBroadcastApis.GetLivePlanList(rlbp, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }

        /// <summary>
        /// Create or modify a live schedule
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/Live/SetLivePlan")]
        public JsonResult SetLivePlan(ReqLiveBroadcastPlan rlbp )
        {
            ResponseStruct rss = CommonFunctions.CheckParams(new object[] {rlbp});
            if (rss.Code != ErrorNumber.None)
            {
                return Program.CommonFunctions.DelApisResult(null!, rss);
            }

            var rt = LiveBroadcastApis.SetLivePlan(rlbp, out ResponseStruct rs);
            return Program.CommonFunctions.DelApisResult(rt, rs);
        }
        
    }
}