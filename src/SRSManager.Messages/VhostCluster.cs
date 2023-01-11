using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostCluster
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public Cluster? Cluster { get; }
        public string? Method { get; }
        public VhostCluster(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostCluster(string deviceId, string vhostDomain, Cluster cluster, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            Cluster= cluster;
        }
    }
}
