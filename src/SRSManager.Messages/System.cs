using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsApis.SrsManager;

namespace SRSManager.Messages
{
    public readonly record struct System
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public SrsManager? Sm { get; }   
        public string Method { get; }
        public System(string method)
        {
            (DeviceId, VHostDomain, Sm, Method)
          = (string.Empty, string.Empty, null!, method);
        }
        public System(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, Sm, Method)
          = (deviceId, vhostDomain, null!, method);
        }
        public System(SrsManager sm, string method)
        {
            (DeviceId, VHostDomain, Sm, Method)
          = (string.Empty, string.Empty, sm, method);
        }
    }
}
