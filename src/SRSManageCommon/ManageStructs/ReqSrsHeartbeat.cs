using System;

namespace SRSManageCommon.ManageStructs
{
    [Serializable]
    public class ReqSrsHeartbeat
    {
        private string? deviceId;
        private string? ip;

        public string? Device_Id
        {
            get => deviceId;
            set => deviceId = value;
        }

        public string? Ip
        {
            get => ip;
            set => ip = value;
        }
    }
}