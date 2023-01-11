using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostDvr
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public Dvr? Dvr { get; }
        public string? Method { get; }
        public VhostDvr(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostDvr(string deviceId, string vhostDomain, Dvr dvr, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Dvr= dvr;
        }
    }
}
