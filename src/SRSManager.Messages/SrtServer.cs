using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrsConfFile.SRSConfClass;

namespace SRSManager.Messages
{
    public readonly record struct SrtServer
    {
        public string? DeviceId { get; }
        public bool? Enable { get; }
        public SrsSrtServerConfClass? Srt { get; }
        public string? Method { get; }
        public SrtServer(string deviceId, string method)
        {
            DeviceId = deviceId;
            Method = method;
        }
        public SrtServer(string deviceId, bool enable, string method)
        {
            DeviceId = deviceId;
            Enable = enable;    
            Method = method;
        }
        public SrtServer(string deviceId, SrsSrtServerConfClass srt, string method)
        {
            DeviceId = deviceId;
            Method = method;
            Srt= srt;
        }
    }
    public readonly record struct GetSrtServerTemplate();
}
