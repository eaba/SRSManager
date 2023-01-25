using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SRSManager.Messages
{
    public readonly record struct LiveBroadcast
    {
        public PulsarSrsConfig? Config { get; }
        public LiveBroadcastPlan? Plan { get; }
        public OnlineClient? Client { get; } 
        public ReqLiveBroadcastPlan? Rlbp { get; }  
        public string? Method { get; }
        public LiveBroadcast(PulsarSrsConfig config, string method)
        {
            Config= config;
            Method = method;
        }
        public LiveBroadcast(OnlineClient client, string method)
        {            
            Client = client;
            Method = method;
        }
        public LiveBroadcast(LiveBroadcastPlan plan, string method)
        {
            Plan= plan;
            Method = method;
        }
        
        public LiveBroadcast(ReqLiveBroadcastPlan rlbp, string method)
        {
            Rlbp=rlbp;
            Method = method;
        }
    }

}
