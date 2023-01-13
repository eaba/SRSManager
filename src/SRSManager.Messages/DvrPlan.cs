using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPulsar.Builder;
using SharpPulsar.User;
using SrsConfFile.SRSConfClass;
using SRSManageCommon.ControllerStructs.RequestModules;
using SRSManageCommon.DBMoudle;

namespace SRSManager.Messages
{
    public readonly record struct DvrPlan
    {
        public PulsarSrsConfig? Client { get; }
        public string? Method { get; }
        public string? TaskId { get; }
        public ReqCutOrMergeVideoFile? Rcmv { get; } 
        public long? DvrVideoId { get; }
        public ReqGetDvrVideo? Rgdv { get; }
       
        public bool? Enable { get; } 
        public ReqStreamDvrPlan? Sdp { get; }   
        public ReqGetDvrPlan? Rdp { get; }
        public DvrPlan(PulsarSrsConfig client, string method)
        {
            Client = client;
            Method = method;
        }
        
        public DvrPlan(string method)
        {
            Method = method;
        }
        public DvrPlan(string taskId, string method)
        {
            TaskId = taskId;
            Method = method;    
        }
        public DvrPlan(ReqCutOrMergeVideoFile rcmv, string method)
        {
            Rcmv= rcmv;
            Method = method;
        }
        public DvrPlan(long dvrVideoId, string method)
        {
            DvrVideoId= dvrVideoId; 
            Method = method;
        }
        public DvrPlan(ReqGetDvrVideo rgdv, string method)
        {
            Rgdv = rgdv;
            Method = method;
        }
        
        public DvrPlan(long dvrVideoId, bool enable, string method)
        {
            DvrVideoId = dvrVideoId;
            Enable = enable;
            Method = method;
        }
        public DvrPlan(long dvrVideoId, ReqStreamDvrPlan sdp, string method)
        {
            DvrVideoId = dvrVideoId;
            Sdp = sdp;
            Method = method;
        }
        public DvrPlan(ReqStreamDvrPlan sdp, string method)
        {
            Sdp = sdp;
            Method = method;
        }
        public DvrPlan(ReqGetDvrPlan rdp, string method)
        {
            Rdp= rdp;
            Method = method;
        }
    }
    
}
