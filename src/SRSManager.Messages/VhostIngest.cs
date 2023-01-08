
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostIngest
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public string IngestName { get; } 
        public bool Enable { get; }
        public Ingest? Ingest { get; }
        public string Method { get; }
        public VhostIngest(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, IngestName, Enable, Ingest, Method)
          = (deviceId, vhostDomain, string.Empty, false, null!, method);
        }
        public VhostIngest(string deviceId, string vhostDomain, string ingestInstanceName, string method)
        {
            (DeviceId, VHostDomain, IngestName, Enable, Ingest, Method)
          = (deviceId, vhostDomain, ingestInstanceName, false, null!, method);
        }
        public VhostIngest(string deviceId, string vhostDomain, string ingestInstanceName, bool enable, string method)
        {
            (DeviceId, VHostDomain, IngestName, Enable, Ingest, Method)
          = (deviceId, vhostDomain, ingestInstanceName, enable, null!, method);
        }
        public VhostIngest(string deviceId, string vhostDomain, string ingestInstanceName, Ingest ingest, string method)
        {
            (DeviceId, VHostDomain, IngestName, Enable, Ingest, Method)
          = (deviceId, vhostDomain, ingestInstanceName, false, ingest, method);
        }
    }
}
