using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace SRSManageCommon.DBMoudle
{
    [Serializable]
    /// <summary>
    /// What to do when the limit is exceeded
    /// </summary>
    public enum OverStepPlan
    {
        StopDvr,
        DeleteFile,
    }

    [Table(Name = "StreamDvrPlan")]
    [Index("uk_dvrPlan_DeviceId", "DeviceId", false)]
    [Index("uk_dvrPlan_VhostDomain", "VhostDomain", false)]
    [Index("uk_dvrPlan_Stream", "Stream", false)]
    [Serializable]
    /// <summary>
    /// recording plan
    /// </summary>
    public class StreamDvrPlan
    {
        [Column(IsPrimary = true, IsIdentity = true )]
        [JsonIgnore]
        public int? Id { get; set; }

        public bool Enable { get; set; }

        public string DeviceId { get; set; } = null!;


        public string VhostDomain { get; set; } = null!;

        public string App { get; set; } = null!;

        public string Stream { get; set; } = null!;

        public long? LimitSpace { get; set; }

        public int? LimitDays { get; set; }
        [Column(MapType = typeof(string))] 
        public OverStepPlan? OverStepPlan { get; set; }

        [Navigate(nameof(DvrDayTimeRange.StreamDvrPlanId))]
        public List<DvrDayTimeRange> TimeRangeList { get; set; } = null!;
    }
}