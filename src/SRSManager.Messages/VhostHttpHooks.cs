using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostHttpHooks
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public HttpHooks? HttpHooks { get; }
        public string Method { get; }
        public VhostHttpHooks(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, HttpHooks, Method)
          = (deviceId, vhostDomain, null!, method);
        }
        public VhostHttpHooks(string deviceId, string vhostDomain, HttpHooks httpHooks, string method)
        {
            (DeviceId, VHostDomain, HttpHooks, Method)
          = (deviceId, vhostDomain, httpHooks, method);
        }
    }
}
