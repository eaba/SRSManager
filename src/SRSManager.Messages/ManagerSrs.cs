using Akka.Actor;

namespace SRSManager.Messages
{
    public readonly record struct ManagerSrs(Dictionary<string, IActorRef> SRSs);
    public readonly record struct GetManagerSrs
    {
        public static GetManagerSrs Instance = new GetManagerSrs(); 
    }
}
