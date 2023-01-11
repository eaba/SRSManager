using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostHttpHooks
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public HttpHooks? HttpHooks { get; }
        public string? Method { get; }
        public VhostHttpHooks(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostHttpHooks(string deviceId, string vhostDomain, HttpHooks httpHooks, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            HttpHooks= httpHooks;
        }
    }
}
