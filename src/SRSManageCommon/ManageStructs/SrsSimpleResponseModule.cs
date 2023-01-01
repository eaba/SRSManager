using System;

namespace SRSManageCommon.ManageStructs
{
    [Serializable]
    public class SrsSimpleResponseModule
    {
        private int? _code;
        private int? _server;

        /// <summary>
        /// return status
        /// </summary>
        public int? Code
        {
            get => _code;
            set => _code = value;
        }

        /// <summary>
        /// srsid
        /// </summary>
        public int? Server
        {
            get => _server;
            set => _server = value;
        }
    }
}