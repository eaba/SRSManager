
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostHttpStatic
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public HttpStatic? HttpStatic { get; }
        public string? Method { get; }
        public VhostHttpStatic(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostHttpStatic(string deviceId, string vhostDomain, HttpStatic httpStatic, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            HttpStatic= httpStatic;
        }
    }
}
