using SRSManageCommon.ManageStructs;

namespace SRSManager.Messages
{
    public record struct CutMergeTaskListAdd
    {
        public CutMergeTask Task { get;}
        public CutMergeTaskListAdd(CutMergeTask task)
        {
            Task= task;
        }
    }
    public readonly record struct CutMergeTaskList();
}
