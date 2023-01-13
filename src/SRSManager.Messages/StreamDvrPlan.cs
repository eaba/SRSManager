using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRSManageCommon.DBMoudle;

namespace SRSManager.Messages
{
    public readonly record struct StreamDvrPlan(int? Id, bool Enable, string DeviceId, string VhostDomain, string App, string Stream, long? LimitSpace, int? LimitDays, OverStepPlan? OverStepPlan, List<DvrDayTimeRange> TimeRangeList);
    public readonly record struct DvrDayTimeRange(int Id, int StreamDvrPlanId, DayOfWeek WeekDay, DateTime StartTime, DateTime EndTime);
}
