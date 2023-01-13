﻿
using SrsManageCommon;

namespace SRSManager.Messages
{
    public readonly record struct DvrVideo(long Id, string? Device_Id, ushort? Client_Id, string? ClientIp, 
        ClientType? ClientType, MonitorType? MonitorType, string? VideoPath, long? FileSize, string? Vhost, string? Dir, string? Stream, string? App, long? Duration, DateTime? StartTime, DateTime? EndTime,
        string? Param, bool? Deleted, DateTime? UpdateTime, string? RecordDate, string? Url, bool? Undo);

}
