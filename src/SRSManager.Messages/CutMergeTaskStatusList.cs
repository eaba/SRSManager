using SRSManageCommon.ManageStructs;

namespace SRSManager.Messages
{
    public record struct CutMergeTaskStatusListAdd
    {
        public CutMergeTask Task { get; }
        public CutMergeTaskStatusListAdd(CutMergeTask task)
        {
            Task = task;
        }
    }
    public readonly record struct CutMergeTaskStatusList
    {
         public static CutMergeTaskStatusList Instance = new CutMergeTaskStatusList();  
    };
}
