using System;

namespace SRSManageCommon.ControllerStructs.RequestModules
{
    /// <summary>
    ///  Delete a request structure that grants access
    /// </summary>
    [Serializable]
    public class ReqDelAllow
    {
        private string _key = null!; //app key value
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
        /// uuid
        /// </summary>
        public string Key
        {
            get => _key;
            set => _key = value;
        }
    }
}