using SRSManageCommon.ManageStructs;

namespace SRSManageCommon.ControllerStructs.RequestModules
{
    /// <summary>
    /// Set or add Allow's request structure
    /// </summary>
    public class ReqSetOrAddAllow
    {
        private AllowKey _allowkey = null!;
        private string _password = null!;

        /// <summary>
        /// password
        /// </summary>
        public string Password
        {
            get => _password;
            set => _password = value;
        }

        /// <summary>
        /// Authorized rhetorical key
        /// </summary>
        public AllowKey Allowkey
        {
            get => _allowkey;
            set => _allowkey = value;
        }
    }
}