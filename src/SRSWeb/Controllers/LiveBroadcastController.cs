using System.Reactive.Joins;
using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SrsManageCommon.ControllerStructs;
using SRSManager.Actors;
using SRSManager.Messages;
using SRSManager.Shared;
using SRSWeb.Attributes;
using ReqLiveBroadcastPlan = SRSManager.Messages.ReqLiveBroadcastPlan;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// Live plan interface class
    /// </summary>
    [ApiController]
    [Route("")]
    public class LiveBroadcastController : ControllerBase
    {
        private readonly IActorRef _actor;
        public LiveBroadcastController(IRequiredActor<SRSManagersActor> actor)
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
        [Route("/Live/PulsarSrsConfig")]
        public async ValueTask<JsonResult> PulsarSrsConfig(string topic, string tenant, string nameSpace, string brokerUrl, string adminUrl, string trinoUrl)
        {
            var client = new PulsarSrsConfig(topic, tenant, nameSpace, brokerUrl, adminUrl, trinoUrl);
            var rss = CommonFunctions.CheckParams(new object[] { client });
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var a = await _actor.Ask<ApisResult>(new LiveBroadcast(client, "PulsarSrsConfig"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        /// <summary>
        /// Check whether the push live broadcast connection is in the plan
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/Live/CheckIsLivePlan")]
        public async ValueTask<JsonResult> CheckIsLivePlan(LiveBroadcastPlan plan )
        {
            var rss = CommonFunctions.CheckParams(new object[] {plan});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new LiveBroadcast(plan, "CheckLivePlan"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// Check if the client connection is a live stream
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/Live/CheckLiveCh")]
        public async ValueTask<JsonResult> CheckLiveCh(OnlineClient client )
        {
            var rss = CommonFunctions.CheckParams(new object[] {client});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new LiveBroadcast(client, "CheckIsLiveCh"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Delete Live Plan byId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthVerify]
        [Log]
        [Route("/Live/DeleteLivePlanById")]
        public async ValueTask<JsonResult> DeleteLivePlanById(LiveBroadcastPlan id )
        {
            var rss = CommonFunctions.CheckParams(new object[] {id});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new LiveBroadcast(id, "DeleteLivePlanById"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Get a list of live broadcast plans
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/Live/GetLivePlanList")]
        public async ValueTask<JsonResult> GetLivePlanList(ReqLiveBroadcastPlan rlbp )
        {
            var rss = CommonFunctions.CheckParams(new object[] {rlbp});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new LiveBroadcast(rlbp, "GetLivePlanList"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// Create or modify a live schedule
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [Log]
        [Route("/Live/SetLivePlan")]
        public async ValueTask<JsonResult> SetLivePlan(ReqLiveBroadcastPlan rlbp )
        {
            var rss = CommonFunctions.CheckParams(new object[] {rlbp});
            if (rss.Code != ErrorNumber.None)
            {
                return Result.DelApisResult(null!, rss);
            }

            var a = await _actor.Ask<ApisResult>(new LiveBroadcast(rlbp, "SetLivePlan"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
        
    }
}