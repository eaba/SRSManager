using Akka.Actor;
using SRSManager.Messages;

namespace SRSManager.Actors
{
    public class SRSManagersActor : ReceiveActor
    {
        private Dictionary<string, IActorRef> _srs = new Dictionary<string, IActorRef>();
        public SRSManagersActor()
        {
            Receive<GlobalSrs>(g =>
            {
                var srs = SRSManager(g.DeviceId);
                srs.Forward(g);
            });
        }
        private IActorRef SRSManager(string deviceId)
        {
            if (_srs.ContainsKey(deviceId))
                return _srs[deviceId];
            else
            {
                var s = Context.ActorOf(SRSManagerActor.Prop());
                _srs[deviceId] = s;
                return s;
            }

        }
        public static Props Prop()
        {
            return Props.Create(() => new SRSManagersActor());
        }
    }
}
