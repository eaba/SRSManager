

namespace SRSManager.Messages
{
    [Serializable]
    public enum LiveBroadcastPlanStatus
    {
        WaitForExec,
        Living,
        Finished,
    }
    public class LiveBroadcastPlan
    {
        public long Id
        {
            get;
            set;
        }
        public bool? Delete
        {
            get;
            set;
        }
        public bool? Enable
        {
            get;
            set;
        }

        public string? DeviceId
        {
            get;
            set;
        }

        public string? App
        {
            get;
            set;
        }

        public string? Vhost
        {
            get;
            set;
        }

        public string? Stream
        {
            get;
            set;
        }

        public DateTime? StartTime
        {
            get;
            set;
        }

        public DateTime? EndTime
        {
            get;
            set;
        }

       
        public LiveBroadcastPlanStatus? PlanStatus
        {
            get;
            set;
        }

        public DateTime? UpdateTime
        {
            get;
            set;
        }

        public string? PublishIpAddr
        {
            get;
            set;
        }
    }
}
