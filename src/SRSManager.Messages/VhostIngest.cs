
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostIngest
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public string? IngestName { get; } 
        public bool? Enable { get; }
        public Ingest? Ingest { get; }
        public string? Method { get; }
        public VhostIngest(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
        }
        public VhostIngest(string deviceId, string vhostDomain, string ingestInstanceName, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            IngestName = ingestInstanceName;    
        }
        public VhostIngest(string deviceId, string vhostDomain, string ingestInstanceName, bool enable, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            IngestName = ingestInstanceName;
            Enable = enable;
        }
        public VhostIngest(string deviceId, string vhostDomain, string ingestInstanceName, Ingest ingest, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Method = method;
            IngestName = ingestInstanceName;    
            Ingest = ingest;    
        }
    }
}
