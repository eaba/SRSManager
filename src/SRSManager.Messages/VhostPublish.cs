
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostPublish
    {
        public string? DeviceId { get; }
        public string?   VHostDomain { get; }
        public Publish? Publish { get; }
        public string? Method { get; }
        public VhostPublish(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;    
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostPublish(string deviceId, string vhostDomain, Publish publish, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Publish= publish;
        }
    }
}
