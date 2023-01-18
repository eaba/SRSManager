using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct RtcServer
    {
        public string? DeviceId { get; }
        public bool? Enable { get; }
        public SrsRtcServerConfClass? Rtc { get; }
        public string? Method { get; }
        public RtcServer(string deviceId, string method)
        {
            DeviceId = deviceId;
            Method = method;
        }
        public RtcServer(string deviceId, bool enable, string method)
        {
            DeviceId = deviceId;
            Enable = enable;
            Method = method;
        }
        public RtcServer(string deviceId, SrsRtcServerConfClass rtc, string method)
        {
            DeviceId = deviceId;
            Method = method;
            Rtc= rtc;
        }
    }
    public readonly record struct GetRtcServerTemplate();
}
