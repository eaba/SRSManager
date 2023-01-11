using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostBandcheck
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public Bandcheck? Bandcheck { get; }
        public string Method { get; }
        public VhostBandcheck(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostBandcheck(string deviceId, string vhostDomain, Bandcheck bandcheck, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Bandcheck= bandcheck;
        }
    }
}
