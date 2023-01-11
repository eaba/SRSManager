using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostHds
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public Hds? Hds { get; }
        public string? Method { get; }
        public VhostHds(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostHds(string deviceId, string vhostDomain, Hds hds, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Hds= hds;
        }
    }
}
