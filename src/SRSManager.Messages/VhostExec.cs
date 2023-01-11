
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostExec
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public Exec? Exec { get; }
        public string? Method { get; }
        public VhostExec(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostExec(string deviceId, string vhostDomain, Exec exec, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Exec= exec;
        }
    }
}
