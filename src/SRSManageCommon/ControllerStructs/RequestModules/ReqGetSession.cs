namespace SRSManageCommon.ControllerStructs.RequestModules
{
    /// <summary>
    ///  Get the request structure of the Session
    /// </summary>
    public class ReqGetSession
    {
        private string _allowKey = null!;

        /// <summary>
        /// allowkey
        /// </summary>
        public string AllowKey
        {
            get => _allowKey;
            set => _allowKey = value;
        }
    }
}