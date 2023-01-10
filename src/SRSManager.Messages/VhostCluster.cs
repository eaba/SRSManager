﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct VhostCluster
    {
        public string DeviceId { get; }
        public string VHostDomain { get; }
        public Cluster? Cluster { get; }
        public string Method { get; }
        public VhostCluster(string deviceId, string vhostDomain, string method)
        {
            (DeviceId, VHostDomain, Cluster, Method)
          = (deviceId, vhostDomain, null!, method);
        }
        public VhostCluster(string deviceId, string vhostDomain, Cluster cluster, string method)
        {
            (DeviceId, VHostDomain, Cluster, Method)
          = (deviceId, vhostDomain, cluster, method);
        }
    }
}