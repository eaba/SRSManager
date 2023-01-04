using SRSManageCommon.ManageStructs;

namespace SRSManager.Messages
{
    public readonly record struct GlobalSrs
    {
        public string DeviceId { get; }
        public string Method { get; }
        public string UserId { get; } 
        public ushort Short { get; } 
        public bool Enable { get; }
        public string Path { get; }    
        public GlobalModule? Gm { get; }

        public GlobalSrs(string deviceId, string method, string userId)
        {
            (DeviceId, Method, UserId, Enable, Short, Path, Gm)
          = (deviceId, method, userId, false, 0, string.Empty, null);
        }

        public GlobalSrs(string deviceId, ushort sh, string method, string userId)
        {
            (DeviceId, Method, UserId, Enable, Short, Path, Gm)
           = (deviceId, method, userId, false, sh, string.Empty, null);
        }
        public GlobalSrs(GlobalModule gm, string method, string userId)
        {
            (DeviceId, Method, UserId, Enable, Short, Path, Gm)
           = (string.Empty, method, userId, false, 0, string.Empty, gm);
        }
        public GlobalSrs(string deviceId, bool enable, string method, string userId)
        {
            (DeviceId, Method, UserId, Enable, Short, Path, Gm)
          = (deviceId, method, userId, enable, 0, string.Empty, null);
        }
        public GlobalSrs(string deviceId, string path, string method, string userId)
        {
            (DeviceId, Method, UserId, Enable, Short, Path, Gm)
           = (deviceId, method, userId,false, 0, path, null);
        }
    }
}
