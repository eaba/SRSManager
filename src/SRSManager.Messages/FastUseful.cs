using SRSManageCommon.ControllerStructs.RequestModules;

namespace SRSManager.Messages
{
    public readonly record struct FastUseful
    {
        public string? DeviceId { get; }
        public string? VHostDomain { get; }
        public PulsarSrsConfig? Client { get; }
        public string? Method { get; }
        public string? TaskId { get; }
        public ReqCutOrMergeVideoFile? Rcmv { get; }
        public long DvrVideoId { get; }
        public ReqGetDvrVideo? Rgdv { get; }

        public bool? Enable { get; }
        public ReqStreamDvrPlan? Sdp { get; }
        public ReqGetDvrPlan? Rdp { get; }
        public FastUseful(PulsarSrsConfig client, string method)
        {
            Client = client;
            Method = method;
        }

        public FastUseful(string method)
        {
            Method = method;
        }
        public FastUseful(string taskId, string method)
        {
            TaskId = taskId;
            Method = method;
        }
        
    }

}
