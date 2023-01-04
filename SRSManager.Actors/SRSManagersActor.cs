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
                var srs = SRSManager(g.UserId);
                srs.Forward(g);
            });
        }
        private IActorRef SRSManager(string userId)
        {
            if (_srs.ContainsKey(userId))
                return _srs[userId];
            else
            {
                var s = Context.ActorOf(SRSManagerActor.Prop());
                _srs[userId] = s;
                return s;
            }

        }
        public static Props Prop()
        {
            return Props.Create(() => new SRSManagersActor());
        }
    }
}
