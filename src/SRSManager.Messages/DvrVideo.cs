
using SrsManageCommon;

namespace SRSManager.Messages
{
    internal class DvrVideo
    {
        public long Id { get; set; }

        public string? Device_Id { get; set; }

        public ushort? Client_Id { get; set; }

        public string? ClientIp { get; set; }

        public ClientType? ClientType { get; set; }

        public MonitorType? MonitorType { get; set; }

        public string? VideoPath { get; set; }

        public long? FileSize { get; set; }

        public string? Vhost { get; set; }

        public string? Dir { get; set; }

        public string? Stream { get; set; }

        public string? App { get; set; }
        public long? Duration { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string? Param { get; set; }

        public bool? Deleted { get; set; }

        public DateTime? UpdateTime { get; set; }
        public string? RecordDate { get; set; }

        public string? Url { get; set; }

        public bool? Undo { get; set; }
    }
}
