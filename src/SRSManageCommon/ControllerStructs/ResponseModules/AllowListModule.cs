using System;
using System.Collections.Generic;
using SRSManageCommon.ManageStructs;

namespace SRSManageCommon.ControllerStructs.ResponseModules
{
    /// <summary>
    /// allowkeylist return structure
    /// </summary>
    [Serializable]
    public class AllowListModule
    {
        private List<AllowKey> _allowKeys = new List<AllowKey>();

        /// <summary>
        /// AllowKey list
        /// </summary>
        public List<AllowKey> AllowKeys
        {
            get => _allowKeys;
            set => _allowKeys = value;
        }
    }
}