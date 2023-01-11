
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostRefer
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public Refer? Refer { get; }
        public string? Method { get; }
        public VhostRefer(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;            
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostRefer(string deviceId, string vhostDomain, Refer refer, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Refer = refer;
        }
    }
}
