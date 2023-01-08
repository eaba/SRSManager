using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostDash
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public Dash? Dash { get; }
        public string Method { get; }
        public VhostDash(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, Dash, Method)
          = (deviceId, vhostDomain, null!, method);
        }
        public VhostDash(string deviceId, string vhostDomain, Dash dash, string method)
        {
            (DeviceId, VHostDomain, Dash, Method)
          = (deviceId, vhostDomain, dash, method);
        }
    }
}
