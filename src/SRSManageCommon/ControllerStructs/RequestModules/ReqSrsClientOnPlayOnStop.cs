using System;

namespace SRSManageCommon.ControllerStructs.RequestModules
{
    /// <summary>
    /// Structure when SRS onClose
    /// </summary>
    [Serializable]
    public class ReqSrsClientOnPlayOnStop
    {
        private string? _action;
        private string? _device_id;
        private string? _clientId;
        private string? _ip;
        private string? _vhost;
        private string? _app;
        private string? _stream;
        private string? _pageUrl;

        /// <summary>
        /// action
        /// </summary>
        public string? Action
        {
            get => _action;
            set => _action = value;
        }

        /// <summary>
        /// srs instance id
        /// </summary>
        public string? Device_Id
        {
            get => _device_id;
            set => _device_id = value;
        }

        /// <summary>
        /// client id
        /// </summary>
        public string? Client_Id
        {
            get => _clientId;
            set => _clientId = value;
        }

        /// <summary>
        /// client ip
        /// </summary>
        public string? Ip
        {
            get => _ip;
            set => _ip = value;
        }

        /// <summary>
        /// vhost
        /// </summary>
        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        /// <summary>
        /// app
        /// </summary>
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        /// <summary>
        /// stream
        /// </summary>
        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        /// <summary>
        /// pageurl
        /// </summary>
        public string? PageUrl
        {
            get => _pageUrl;
            set => _pageUrl = value;
        }
    }
}