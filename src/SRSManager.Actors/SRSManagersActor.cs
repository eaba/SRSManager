using Akka.Actor;
using SharpPulsar;
using SRSManager.Messages;

namespace SRSManager.Actors
{
    public class SRSManagersActor : ReceiveActor
    {
        private PulsarSystem _pulsarSystem = PulsarSystem.GetInstance(Context.System, actorSystemName: "Pulsar");
        private Dictionary<string, IActorRef> _srs = new Dictionary<string, IActorRef>();
        public SRSManagersActor()
        {
            Receive<GlobalSrs>(g =>
            {
                var srs = SRSManager(g.DeviceId);
                srs.Forward(g);
            });
            Receive<VhostTranscode>(v =>
            {
                var srs = SRSManager(v.DeviceId);
                srs.Forward(v);
            });
        }
        private IActorRef SRSManager(string deviceId)
        {
            if (_srs.ContainsKey(deviceId))
                return _srs[deviceId];
            else
            {
                var s = Context.ActorOf(SRSManagerActor.Prop(_pulsarSystem));
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
