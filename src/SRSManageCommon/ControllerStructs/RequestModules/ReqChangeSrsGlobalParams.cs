using System;
using SRSManageCommon.ManageStructs;

namespace SRSManageCommon.ControllerStructs.RequestModules
{
    /// <summary>
    /// Request to modify the structure of the global parameters of the srs instance
    /// </summary>
    [Serializable]
    public class ReqChangeSrsGlobalParams
    {
        private string _deviceId = null!;
        private GlobalModule _gm = null!;

        /// <summary>
        /// srsmanager
        /// </summary>
        public string DeviceId
        {
            get => _deviceId;
            set => _deviceId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// globalmodule
        /// </summary>
        public GlobalModule Gm
        {
            get => _gm;
            set => _gm = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}