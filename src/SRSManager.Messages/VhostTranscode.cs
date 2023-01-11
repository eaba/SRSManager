

using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostTranscode
    {
        public string DeviceId { get; }
        public string? VHostDomain { get; }  
        public string? TranscodeInstanceName { get; } 
        public Transcode? Transcode { get; } 
        public string Method { get; }
        public VhostTranscode(string deviceId, string vhostDomain, string method)
        {
            DeviceId = deviceId;  
            VHostDomain = vhostDomain;  
            Method = method;    
        }
        public VhostTranscode(string deviceId, string vhostDomain, Transcode transcode, string method)
        {
            DeviceId = deviceId;
            VHostDomain = vhostDomain;
            Transcode = transcode;
            Method = method;    
        }
        public VhostTranscode(string deviceId, string vhostDomain, string transcodeInstanceName, string method)
        {
            DeviceId = deviceId;    
            VHostDomain = vhostDomain;  
            Method = method;
            TranscodeInstanceName = transcodeInstanceName;  
        }
    }
}
