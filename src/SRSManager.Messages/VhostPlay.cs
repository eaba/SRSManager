using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostPlay
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public Play? Play { get; }
        public string Method { get; }
        public VhostPlay(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, Play, Method)
          = (deviceId, vhostDomain, null!, method);
        }
        public VhostPlay(string deviceId, string vhostDomain, Play play, string method)
        {
            (DeviceId, VHostDomain, Play, Method)
          = (deviceId, vhostDomain, play, method);
        }
    }
}
