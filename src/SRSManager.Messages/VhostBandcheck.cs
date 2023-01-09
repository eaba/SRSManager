using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostBandcheck
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public Bandcheck? Bandcheck { get; }
        public string Method { get; }
        public VhostBandcheck(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, Bandcheck, Method)
          = (deviceId, vhostDomain, null!, method);
        }
        public VhostBandcheck(string deviceId, string vhostDomain, Bandcheck bandcheck, string method)
        {
            (DeviceId, VHostDomain, Bandcheck, Method)
          = (deviceId, vhostDomain, bandcheck, method);
        }
    }
}
