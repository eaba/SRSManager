using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostDash
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public Dash? Dash { get; }
        public string? Method { get; }
        public VhostDash(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostDash(string deviceId, string vhostDomain, Dash dash, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Dash= dash;
        }
    }
}
