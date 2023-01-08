using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostDvr
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public Dvr? Dvr { get; }
        public string Method { get; }
        public VhostDvr(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, Dvr, Method)
          = (deviceId, vhostDomain, null!, method);
        }
        public VhostDvr(string deviceId, string vhostDomain, Dvr dvr, string method)
        {
            (DeviceId, VHostDomain, Dvr, Method)
          = (deviceId, vhostDomain, dvr, method);
        }
    }
}
