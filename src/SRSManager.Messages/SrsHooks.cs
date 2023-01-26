
using SRSManageCommon.ManageStructs;

namespace SRSManager.Messages
{
    public readonly record struct SrsHooks
    {
        public OnlineClient? Client { get; }
        public  PulsarSrsConfig Config { get; } 
        public DvrVideo Dvr { get; }
        public ReqSrsHeartbeat? HeartBeat { get; } 
        public string Method { get; }   
        public SrsHooks(OnlineClient client, PulsarSrsConfig config, string method)
        {
            Client= client;
            Config= config;
            Method= method;
        }
        public SrsHooks(DvrVideo dvr, PulsarSrsConfig config, string method)
        {
            Dvr= dvr;
            Config= config; 
            Method = method;
        }
        public SrsHooks(ReqSrsHeartbeat heath, PulsarSrsConfig config, string method)
        {
            HeartBeat= heath;
            Config= config; 
            Method = method;
        }
    }
}
