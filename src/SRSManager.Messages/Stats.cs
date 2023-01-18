using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct Stats
    {
        public string? DeviceId { get; }
        public SrsStatsConfClass? Stat { get; }
        public string? Method { get; }
        public Stats(string deviceId, string method)
        {
            DeviceId = deviceId;
            Method = method;
        }
        public Stats(string deviceId, SrsStatsConfClass stats, string method)
        {
            DeviceId = deviceId;
            Method = method;
            Stat = stats;
        }
    }
}
