

using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostSecurity
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public Security? Security { get; }
        public string? Method { get; }
        public VhostSecurity(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId; 
            VHostDomain = vhostDomain; 
            Method = method;    
        }
        public VhostSecurity(string deviceId, string vhostDomain, Security security, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Security = security;
        }
    }
}
