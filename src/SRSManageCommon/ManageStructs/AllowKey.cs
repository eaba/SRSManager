using System;
using System.Collections.Generic;

namespace SRSManageCommon.ManageStructs
{
    /// <summary>
    /// allowkey management class
    /// </summary>
    [Serializable]
    public class AllowKey
    {
        private List<string> _ipArray = new List<string>();
        private string _key = null!;

        /// <summary>
        /// key value
        /// </summary>
        public string Key
        {
            get => _key;
            set => _key = value;
        }

        /// <summary>
        /// ip address list
        /// </summary>
        public List<string> IpArray
        {
            get => _ipArray;
            set => _ipArray = value;
        }
    }
}