using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostForward
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public Forward? Forward { get; }
        public string Method { get; }
        public VhostForward(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, Forward, Method)
          = (deviceId, vhostDomain, null!, method);
        }
        public VhostForward(string deviceId, string vhostDomain, Forward forward, string method)
        {
            (DeviceId, VHostDomain, Forward, Method)
          = (deviceId, vhostDomain, forward, method);
        }
    }
}
