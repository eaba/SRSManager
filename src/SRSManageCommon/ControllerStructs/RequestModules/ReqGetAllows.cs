using System;

namespace SRSManageCommon.ControllerStructs.RequestModules
{
    /// <summary>
    /// Request structure for obtaining authorized access list
    /// </summary>
    [Serializable]
    public class ReqGetAllows
    {
        private string password = null!;

        /// <summary>
        /// password
        /// </summary>
        public string Password
        {
            get => password;
            set => password = value;
        }
    }
}