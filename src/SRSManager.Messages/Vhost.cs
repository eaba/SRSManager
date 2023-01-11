using SrsApis.SrsManager;
using SrsConfFile.SRSConfClass;
using SRSManageCommon.ManageStructs;

namespace SRSManager.Messages
{
    public readonly record struct Vhost
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public string? NewVhostDomain { get; }   
        public VhostIngestInputType? VType { get; }  
        public SrsvHostConfClass? VHost { get; }
        public string? Method { get; }

        public Vhost(string deviceId, string method)
        {
            DeviceId = deviceId;
            Method = method;
        }
        public Vhost(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public Vhost(VhostIngestInputType vtype, string method)
        {
            VType= vtype;
            Method = method;
        }
        public Vhost(string deviceId, SrsvHostConfClass vhost, string method)
        {
            DeviceId = deviceId;
            VHost = vhost;
            Method = method;
        }
        public Vhost(string deviceId, string vhostDomain, string newVhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            NewVhostDomain= newVhostDomain;
        }
    }

    public readonly record struct CheckNewSrsInstanceListenRight(SrsManager Sm);
    public readonly record struct CheckNewSrsInstancePathRight(SrsManager Sm);
}
