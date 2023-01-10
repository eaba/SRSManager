using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsApis.SrsManager;
using SrsConfFile.SRSConfClass;
using SRSManageCommon.ManageStructs;

namespace SRSManager.Messages
{
    public readonly record struct Vhost
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public string NewVhostDomain { get; }   
        public VhostIngestInputType? VType { get; }  
        public SrsvHostConfClass? VHost { get; }
        public string Method { get; }

        public Vhost(string deviceId, string method)
        {
            (DeviceId, VHostDomain, NewVhostDomain, VType, VHost, Method)
          = (deviceId, string.Empty, string.Empty, null!, null!, method);
        }
        public Vhost(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, NewVhostDomain, VType, VHost, Method)
          = (deviceId, vhostDomain, string.Empty, null!, null!, method);
        }
        public Vhost(VhostIngestInputType vtype, string method)
        {
            (DeviceId, VHostDomain, NewVhostDomain, VType, VHost, Method)
          = (string.Empty, string.Empty, string.Empty, vtype, null!, method);
        }
        public Vhost(string deviceId, SrsvHostConfClass vhost, string method)
        {
            (DeviceId, VHostDomain, NewVhostDomain, VType, VHost, Method)
          = (deviceId, string.Empty, string.Empty, null!, vhost, method);
        }
        public Vhost(string deviceId, string vhostDomain, string newVhostDomain, string method)
        {
            (DeviceId, VHostDomain, NewVhostDomain, VType, VHost, Method)
          = (deviceId, vhostDomain, newVhostDomain, null!, null!, method);
        }
    }

    public readonly record struct CheckNewSrsInstanceListenRight(SrsManager Sm);
    public readonly record struct CheckNewSrsInstancePathRight(SrsManager Sm);
}
