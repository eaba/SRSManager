
using SrsManageCommon;

namespace SRSManager.Messages
{
    [Serializable]
    public enum EventMethod
    {
        Connect,
        Close,
        Publish,
        UnPublish,
        Play,
        Stop
    }
    public class ClientLog
    {
        
        public long Id
        {
            get;
            set;
        }

        public string? Device_Id
        {
            get;
            set;
        }

        public string? MonitorIp
        {
            get;
            set;
        }

        public ushort? Client_Id
        {
            get;
            set;
        }

        public string? ClientIp
        {
            get;
            set;
        }

        
        public ClientType? ClientType
        {
            get;
            set;
        }

        public MonitorType? MonitorType
        {
            get;
            set;
        }

        public string? RtmpUrl
        {
            get;
            set;
        }

        public string? HttpUrl
        {
            get;
            set;
        }
        public string? RtspUrl
        {
            get;
            set;
        }

        public string? Vhost
        {
            get;
            set;
        }

        public string? App
        {
            get;
            set;
        }

        public string? Stream
        {
            get;
            set;
        }

        public string? Param
        {
            get;
            set;
        }

        public bool? IsOnline
        {
            get;
            set;
        }

        public bool? IsPlay
        {
            get;
            set;
        }

        public string? PageUrl
        {
            get;
            set;
        }

        public DateTime? UpdateTime
        {
            get;
            set;
        }

        public EventMethod EventMethod
        {
            get;
            set;
        }
    }
}
