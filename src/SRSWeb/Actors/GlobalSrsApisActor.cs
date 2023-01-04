using Akka.Actor;
using Akka.Event;
using SrsApis.SrsManager.Apis;
using SRSManageCommon.ManageStructs;
using SrsManageCommon;
using SharpPulsar;

namespace SRSWeb.Actors
{
    public class GlobalSrsApisActor: ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();
        public GlobalSrsApisActor() 
        {
            Receive<string>(deviceIdIf => deviceIdIf == "StartSrs", deviceId =>
            {
                ResponseStruct rss = CommonFunctions.CheckParams(new object[] { deviceId });
                if (rss.Code != ErrorNumber.None)
                {
                    Sender.Tell(new Messages.DelApisResult(null!, rss));
                }

                var rt = GlobalSrsApis.StartSrs(deviceId, out ResponseStruct rs);

                Sender.Tell(new Messages.DelApisResult(rt, rs));

            });
            Receive<string>(deviceIdIf => deviceIdIf == "StopSrs", deviceId => 
            {
                ResponseStruct rss = CommonFunctions.CheckParams(new object[] { deviceId });
                if (rss.Code != ErrorNumber.None)
                {
                   Sender.Tell(new Messages.DelApisResult(null!, rss));
                }

                var rt = GlobalSrsApis.StopSrs(deviceId, out ResponseStruct rs);
                
                Sender.Tell(new Messages.DelApisResult(rt, rs));

            });
            Receive<string>(deviceIdIf => deviceIdIf == "RestartSrs", deviceId =>
            {
                ResponseStruct rss = CommonFunctions.CheckParams(new object[] { deviceId });
                if (rss.Code != ErrorNumber.None)
                {
                    Sender.Tell(new Messages.DelApisResult(null!, rss));
                }

                var rt = GlobalSrsApis.RestartSrs(deviceId, out ResponseStruct rs);

                Sender.Tell(new Messages.DelApisResult(rt, rs));

            });
            Receive<string>(deviceIdIf => deviceIdIf == "ReloadSrs", deviceId =>
            {
                ResponseStruct rss = CommonFunctions.CheckParams(new object[] { deviceId });
                if (rss.Code != ErrorNumber.None)
                {
                    Sender.Tell(new Messages.DelApisResult(null!, rss));
                }

                var rt = GlobalSrsApis.ReloadSrs(deviceId, out ResponseStruct rs);

                Sender.Tell(new Messages.DelApisResult(rt, rs));

            });
        }
        public static Props Prop()
        {
            return Props.Create(() => new GlobalSrsApisActor());
        }
    }
}
