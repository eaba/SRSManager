using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostHds
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public Hds? Hds { get; }
        public string Method { get; }
        public VhostHds(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, Hds, Method)
          = (deviceId, vhostDomain, null!, method);
        }
        public VhostHds(string deviceId, string vhostDomain, Hds hds, string method)
        {
            (DeviceId, VHostDomain, Hds, Method)
          = (deviceId, vhostDomain, hds, method);
        }
    }
}
