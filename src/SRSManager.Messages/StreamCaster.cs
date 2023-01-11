using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct StreamCaster
    {
        public string DeviceId { get; }
        public string? InstanceName { get; }
        public string? NewInstanceName { get; }   
        public bool? Enable { get; }
        public SrsStreamCasterConfClass? Streamcaster { get; }
        public string Method { get; }

        public StreamCaster(string deviceId, string method) 
        {            
           DeviceId = deviceId;
           Method = method;
        }
        public StreamCaster(string deviceId, SrsStreamCasterConfClass streamcaster, string method)
        {
            DeviceId = deviceId;
            Streamcaster = streamcaster;
            Method = method;
        }
       
        public StreamCaster(string deviceId, string instanceName, string method)
        {
            DeviceId = deviceId;
            Method = method;
            InstanceName = instanceName;
        }
        public StreamCaster(string deviceId, string instanceName, string newInstanceName, string method)
        {
            DeviceId = deviceId;
            Method = method;
            InstanceName = instanceName;
            NewInstanceName = newInstanceName;   
        }
        public StreamCaster(string deviceId, string instanceName, bool enable, string method)
        {
            DeviceId = deviceId;
            Method = method;
            InstanceName = instanceName;
            Enable = enable;
        }
    }
    public readonly record struct GetStreamCasterTemplate(CasterEnum CasterType);
}
