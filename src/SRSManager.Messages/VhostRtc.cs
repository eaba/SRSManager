using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostRtc
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public Rtc? Rtc { get; }
        public string? Method { get; }
        public VhostRtc(string deviceId, string vhostDomain, string method)
        {
            DeviceId  = deviceId;  
            VHostDomain = vhostDomain;  
            Method = method;
        }
        public VhostRtc(string deviceId, string vhostDomain, Rtc rtc, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Rtc = rtc;
        }
    }
}
