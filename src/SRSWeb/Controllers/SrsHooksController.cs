using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.RequestModules;
using SRSManageCommon.ManageStructs;
using SRSManager.Actors;
using SRSManager.Messages;
using SRSWeb.Attributes;

namespace SRSWeb.Controllers
{
    /// <summary>
    /// SRSHooks control class
    /// </summary>
    [ApiController]
    [Route("")]
    public class SrsHooksController : ControllerBase
    {
        private readonly IActorRef _actor;
        public SrsHooksController(IRequiredActor<SRSManagersActor> actor)
        {
            _actor = actor.ActorRef;
        }

        /// <summary>
        /// Handle heartbeat information
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnHeartbeat")]
        public async ValueTask<JsonResult> OnHeartbeat(ReqSrsHeartbeat heartbeat, string brokerUrl, string tenant, string nameSpace, string trinoUrl)
        {
            var client = new PulsarSrsConfig(string.Empty, tenant, nameSpace, brokerUrl, string.Empty, trinoUrl);
            
            var a = await _actor.Ask<ApisResult>(new SrsHooks(heartbeat, client, "OnHeartbeat"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// When a camera stops streaming
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnUnPublish")]
        public async ValueTask<JsonResult> OnUnPublish(ReqSrsClientOnOrUnPublish client, string brokerUrl, string tenant, string nameSpace, string trinoUrl)
        {
            var ct = new PulsarSrsConfig(string.Empty, tenant, nameSpace, brokerUrl, string.Empty, trinoUrl);
            var tmpOnlineClient = new OnlineClient()
            {
                Device_Id = client.Device_Id,
                Client_Id =ushort.Parse(client.Client_Id!),
                ClientIp = client.Ip,
                ClientType = ClientType.Monitor,
                App = client.App,
                HttpUrl = "",
                IsOnline = true,
                Param = client.Param,
                RtmpUrl = client.TcUrl,
                Stream = client.Stream,
                UpdateTime = DateTime.Now,
                Vhost = client.Vhost,
            };

            var a = await _actor.Ask<ApisResult>(new SrsHooks(tmpOnlineClient, ct, "OnUnPublish"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// When the recording is complete
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnDvr")]
        public async ValueTask<JsonResult> OnDvr(ReqSrsDvr dvr, string brokerUrl, string tenant, string nameSpace, string trinoUrl)
        {
            var client = new PulsarSrsConfig(string.Empty, tenant, nameSpace, brokerUrl, string.Empty, trinoUrl);
            var currentTime = DateTime.Now;

            var tmpDvrVideo = new DvrVideo()
            {
                Device_Id = dvr.Device_Id,
                Client_Id = ushort.Parse(dvr.Client_Id!),
                ClientIp = dvr.Ip,
                ClientType = ClientType.Monitor,
                VideoPath = dvr.File,
                App = dvr.App,
                Stream = dvr.Stream,
                Param = dvr.Param,
                Vhost = dvr.Vhost,
                Dir = Path.GetDirectoryName(dvr.File),
            };

            var dvrFile = new FileInfo(dvr.File!);
            tmpDvrVideo.FileSize = dvrFile.Length;
            if (FFmpegGetDuration.GetDuration(Common.FFmpegBinPath, dvr.File!, out var duration,out var newPath))
            {
                tmpDvrVideo.VideoPath = newPath;
                tmpDvrVideo.Duration = duration;
                tmpDvrVideo.StartTime = currentTime.AddMilliseconds(duration * (-1));
                tmpDvrVideo.EndTime = currentTime;
            }
            else
            {
                tmpDvrVideo.Duration = -1;
                tmpDvrVideo.StartTime = currentTime;
                tmpDvrVideo.EndTime = currentTime;
            }

           
            var a = await _actor.Ask<ApisResult>(new SrsHooks(tmpDvrVideo, client, "OnDvr"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// When a client plays
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnPlay")]
        public async ValueTask<JsonResult> OnPlay(ReqSrsClientOnPlayOnStop client, string brokerUrl, string tenant, string nameSpace, string trinoUrl)
        {
            var ct = new PulsarSrsConfig(string.Empty, tenant, nameSpace, brokerUrl, string.Empty, trinoUrl);
            var tmpOnlineClient = new OnlineClient()
            {
                Device_Id = client.Device_Id,
                Client_Id = ushort.Parse(client.Client_Id!),
                ClientIp = client.Ip,
                ClientType = ClientType.Monitor,
                App = client.App,
                HttpUrl = "",
                IsOnline = true,
                IsPlay = true,
                Stream = client.Stream,
                UpdateTime = DateTime.Now,
                Vhost = client.Vhost,
                PageUrl = client.PageUrl,
            };
            
            var a = await _actor.Ask<ApisResult>(new SrsHooks(tmpOnlineClient, ct, "OnPlay"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }

        /// <summary>
        /// When the client stops playing
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnStop")]
        public async ValueTask<JsonResult> OnStop(ReqSrsClientOnPlayOnStop client, string brokerUrl, string tenant, string nameSpace, string trinoUrl)
        {
            var ct = new PulsarSrsConfig(string.Empty, tenant, nameSpace, brokerUrl, string.Empty, trinoUrl);
            var tmpOnlineClient = new OnlineClient()
            {
                Device_Id = client.Device_Id,
                Client_Id =ushort.Parse(client.Client_Id!),
                ClientIp = client.Ip,
                ClientType = ClientType.Monitor,
                App = client.App,
                HttpUrl = "",
                IsOnline = true,
                IsPlay = false,
                Stream = client.Stream,
                UpdateTime = DateTime.Now,
                Vhost = client.Vhost,
                PageUrl = client.PageUrl,
            };
            
            var a = await _actor.Ask<ApisResult>(new SrsHooks(tmpOnlineClient, ct, "OnStop"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }


        /// <summary>
        /// When there is a camera for streaming
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnPublish")]
        public async ValueTask<JsonResult> OnPublish(ReqSrsClientOnOrUnPublish client, string brokerUrl, string tenant, string nameSpace, string trinoUrl)
        {
            var ct = new PulsarSrsConfig(string.Empty, tenant, nameSpace, brokerUrl, string.Empty, trinoUrl);
            var tmpOnlineClient = new OnlineClient()
            {
                Device_Id = client.Device_Id,
                Client_Id =ushort.Parse(client.Client_Id!),
                ClientIp = client.Ip,
                ClientType = ClientType.Monitor,
                MonitorType = MonitorType.Onvif,
                App = client.App,
                HttpUrl = "",
                IsOnline = true,
                Param = client.Param,
                RtmpUrl = client.TcUrl,
                Stream = client.Stream,
                UpdateTime = DateTime.Now,
                Vhost = client.Vhost,
            };
            var a = await _actor.Ask<ApisResult>(new SrsHooks(tmpOnlineClient, ct, "OnPublish"));
            return Result.DelApisResult(a.Rt, a.Rs);
        }
    }
}