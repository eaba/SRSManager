using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostRtc
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public Rtc? Rtc { get; }
        public string Method { get; }
        public VhostRtc(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, Rtc, Method)
          = (deviceId, vhostDomain, null!, method);
        }
        public VhostRtc(string deviceId, string vhostDomain, Rtc rtc, string method)
        {
            (DeviceId, VHostDomain, Rtc, Method)
          = (deviceId, vhostDomain, rtc, method);
        }
    }
}
