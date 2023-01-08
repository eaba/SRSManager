
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostHttpRemux
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public HttpRemux? HttpRemux { get; }
        public string Method { get; }
        public VhostHttpRemux(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, HttpRemux, Method)
          = (deviceId, vhostDomain, null!, method);
        }
        public VhostHttpRemux(string deviceId, string vhostDomain, HttpRemux httpRemux, string method)
        {
            (DeviceId, VHostDomain, HttpRemux, Method)
          = (deviceId, vhostDomain, httpRemux, method);
        }
    }
}
