
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostHls
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public Hls? HostHls { get; }
        public string? Method { get; }
        public VhostHls(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostHls(string deviceId, string vhostDomain, Hls hostHls, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            HostHls= hostHls;   
        }
    }
}
