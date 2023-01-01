using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.RequestModules;
using SRSManageCommon.DBMoudle;
using SRSManageCommon.ManageStructs;
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

        /// <summary>
        /// Handle heartbeat information
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnHeartbeat")]
        public int OnHeartbeat(ReqSrsHeartbeat heartbeat)
        {
            var rt = SrsHooksApis.OnHeartbeat(heartbeat, out ResponseStruct rs);
            if (rt)
            {
                return 0;
            }

            return -1;
        }

        /*
        /// <summary>
        /// When there is a client or the camera is turned off
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnClose")]
        public int OnClose(ReqSrsClientOnClose client)
        {
            OnlineClient tmpOnlineClient = new OnlineClient()
            {
                Device_Id = client.Device_Id,
                Client_Id = ushort.Parse(client.Client_Id!),
                ClientIp = client.Ip,
                App = client.App,
                Vhost = client.Vhost,
            };
            var rt = SrsHooksApis.OnClose(tmpOnlineClient);
            if (rt) return 0;
            return -1;
        }
        */
        /// <summary>
        /// When a camera stops streaming
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnUnPublish")]
        public int OnUnPublish(ReqSrsClientOnOrUnPublish client)
        {
            OnlineClient tmpOnlineClient = new OnlineClient()
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
            var rt = SrsHooksApis.OnUnPublish(tmpOnlineClient);
            if (rt) return 0;
            return -1;
        }


        /// <summary>
        /// When the recording is complete
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnDvr")]
        public int OnDvr(ReqSrsDvr dvr)
        {
            DateTime currentTime = DateTime.Now;

            DvrVideo tmpDvrVideo = new DvrVideo()
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

            FileInfo dvrFile = new FileInfo(dvr.File);
            tmpDvrVideo.FileSize = dvrFile.Length;
            if (FFmpegGetDuration.GetDuration(Common.FFmpegBinPath, dvr.File!, out long duration,out string newPath))
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

            SrsHooksApis.OnDvr(tmpDvrVideo);
            return 0;
        }


        /// <summary>
        /// When a client plays
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnPlay")]
        public int OnPlay(ReqSrsClientOnPlayOnStop client)
        {
            OnlineClient tmpOnlineClient = new OnlineClient()
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
            var rt = SrsHooksApis.OnPlay(tmpOnlineClient);
            if (rt) return 0;
            return -1;
        }

        /// <summary>
        /// When the client stops playing
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnStop")]
        public int OnStop(ReqSrsClientOnPlayOnStop client)
        {
            OnlineClient tmpOnlineClient = new OnlineClient()
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
            var rt = SrsHooksApis.OnStop(tmpOnlineClient);
            if (rt) return 0;
            return -1;
        }


        /// <summary>
        /// When there is a camera for streaming
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnPublish")]
        public int OnPublish(ReqSrsClientOnOrUnPublish client)
        {
            OnlineClient tmpOnlineClient = new OnlineClient()
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
            var rt = SrsHooksApis.OnPublish(tmpOnlineClient);
            if (rt) return 0;
            return -1;
        }
/*
        /// <summary>
        /// When a client or camera is connected
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthVerify]
        [LogSrsCallBack]
        [Route("/SrsHooks/OnConnect")]
        public int OnConnect(ReqSrsClientOnConnect client)
        {
            OnlineClient tmpOnlineClient = new OnlineClient()
            {
                MonitorType = MonitorType.Unknow,
                Device_Id = client.Device_Id,
                Client_Id = ushort.Parse(client.Client_Id!),
                ClientIp = client.Ip,
                ClientType = ClientType.User,
                App = client.App,
                HttpUrl = client.PageUrl,
                IsOnline = true,
                Param = "",
                RtmpUrl = client.TcUrl,
                Stream = "",
                UpdateTime = DateTime.Now,
                Vhost = client.Vhost,
            };
            var rt = SrsHooksApis.OnConnect(tmpOnlineClient);
            if (rt) return 0;
            return -1;
        }
   */
    }
}