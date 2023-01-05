

using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostTranscode
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }  
        public string TranscodeInstanceName { get; } 
        public Transcode? Transcode { get; } 
        public string Method { get; }
        public VhostTranscode(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, TranscodeInstanceName, Transcode, Method)
          = (deviceId, vhostDomain, string.Empty, null, method);
        }
        public VhostTranscode(string deviceId, string vhostDomain, Transcode transcode, string method)
        {
            (DeviceId, VHostDomain, TranscodeInstanceName, Transcode, Method)
          = (deviceId, vhostDomain, string.Empty, transcode, method);
        }
        public VhostTranscode(string deviceId, string vhostDomain, string transcodeInstanceName, string method)
        {
            (DeviceId, VHostDomain, TranscodeInstanceName, Transcode, Method)
          = (deviceId, vhostDomain, transcodeInstanceName, null, method);
        }
    }
}
