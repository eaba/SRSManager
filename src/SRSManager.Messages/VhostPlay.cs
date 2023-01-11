using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostPlay
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public Play? Play { get; }
        public string? Method { get; }
        public VhostPlay(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostPlay(string deviceId, string vhostDomain, Play play, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Play= play;
        }
    }
}
