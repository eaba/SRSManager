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
        public System(string deviceId, string method)
        {
            (DeviceId, VHostDomain, Sm, Method)
          = (deviceId, string.Empty, null!, method);
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

    public readonly record struct GetAllSrsManagerDeviceId
    {
        public static GetAllSrsManagerDeviceId Instance = new GetAllSrsManagerDeviceId();
    }
    public readonly record struct GetDriveDisksInfo
    {
        public static GetDriveDisksInfo Instance = new GetDriveDisksInfo();
    }

    public readonly record struct GetSystemInfo
    {
        public static GetSystemInfo Instance = new GetSystemInfo();
    }

    public readonly record struct GetNetworkAdapterList()
    {
        public static GetNetworkAdapterList Instance = new GetNetworkAdapterList();
    }
    public readonly record struct GetSrsInstanceTemplate()
    {
        public static GetSrsInstanceTemplate Instance = new GetSrsInstanceTemplate();
    }
    public readonly record struct GetSrsManager()
    {
        public static GetSrsManager Instance = new GetSrsManager();
    }
    public readonly record struct GetSrsManagers()
    {
        public static GetSrsManagers Instance = new GetSrsManagers();
    }
}
