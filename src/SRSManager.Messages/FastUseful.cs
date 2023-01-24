
namespace SRSManager.Messages
{
    public readonly record struct FastUseful
    {
        public string? DeviceId { get; }
        public string? Id { get; }
        public string? VHostDomain { get; }
        public string? IngestName { get; }
        public bool? Enable { get; }
        public string? StreamId { get; }
        public string? ClientId { get; } 
        public string? VHostId { get; }
        public string? Method { get; }
        public FastUseful(string method)
        {
            Method = method;
        }
        public FastUseful(string deviceId, string method)
        {
            DeviceId= deviceId;
            Method = method;
        }
        public FastUseful(string deviceId, string vHostId, string method)
        {
            DeviceId = deviceId;
            VHostId= vHostId;
            Method = method;
        }
        public FastUseful(string deviceId, string vhostDomain, string ingestName, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            IngestName = ingestName;
            Method = method;
        }
        public FastUseful(string deviceId, string vhostDomain, bool enable, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Enable = enable;    
            Method = method;
        }
        
        public FastUseful(string? id, string? deviceId, string? streamId, string? clientId, string method)
        {
            DeviceId = deviceId;    
            Id= id;
            ClientId= clientId;
            StreamId = streamId;
            Method = method;
        }
       
    }

}
