using SRSManageCommon.DBMoudle;

namespace SRSManager.Messages
{
    public record struct StreamDvrPlan(long StreamDvrPlanId, bool Enable, string DeviceId, string VhostDomain, string App, string Stream, long? LimitSpace, int? LimitDays, OverStepPlan? OverStepPlan, List<DvrDayTimeRange> TimeRangeList, bool delete);
    public readonly record struct DvrDayTimeRange(DayOfWeek WeekDay, DateTime StartTime, DateTime EndTime);
}
