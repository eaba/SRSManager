using System;
using System.Text.Json.Serialization;
using FreeSql.DataAnnotations;

namespace SRSManageCommon.DBMoudle
{
    [Serializable]
    [Table(Name = "DvrDayTimeRange")]
    /// <summary>
    /// Recording time for each week
    /// </summary>
    public class DvrDayTimeRange
    {
        [Column(IsPrimary = true, IsIdentity = true )]
        [JsonIgnore]
        public int Id { get; set; }

        public int StreamDvrPlanId { get; set; }
        [Column(MapType = typeof(string))]
        public DayOfWeek WeekDay { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}