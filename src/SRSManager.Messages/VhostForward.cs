using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostForward
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public Forward? Forward { get; }
        public string? Method { get; }
        public VhostForward(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostForward(string deviceId, string vhostDomain, Forward forward, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Forward= forward;
        }
    }
}
