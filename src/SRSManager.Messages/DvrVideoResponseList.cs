using SRSManageCommon.ControllerStructs.RequestModules;

namespace SRSManager.Messages
{
    [Serializable]
    public class DvrVideoResponseList
    {
        private List<DvrVideo>? _dvrVideoList;
        private ReqGetDvrVideo? _request;
        private long? _total;

        public List<DvrVideo>? DvrVideoList
        {
            get => _dvrVideoList;
            set => _dvrVideoList = value;
        }

        public ReqGetDvrVideo? Request
        {
            get => _request;
            set => _request = value;
        }

        public long? Total
        {
            get => _total;
            set => _total = value;
        }
    }
}
