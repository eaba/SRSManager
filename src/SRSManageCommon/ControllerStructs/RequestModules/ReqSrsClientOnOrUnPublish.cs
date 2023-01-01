using System;

namespace SRSManageCommon.ControllerStructs.RequestModules
{
    /// <summary>
    /// Structure of SRS onpublish
    /// </summary>
    [Serializable]
    public class ReqSrsClientOnOrUnPublish
    {
        private string? _action;
        private string? _device_id;
        private string? _clientId;
        private string? _ip;
        private string? _vhost;
        private string? _app;
        private string? _tcUrl;
        private string? _stream;
        private string? _param;

        /// <summary>
        /// action,on_connect|on_close
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
        /// client ID
        /// </summary>
        public string? Client_Id
        {
            get => _clientId;
            set => _clientId = value;
        }

        /// <summary>
        /// client ip address
        /// </summary>
        public string? Ip
        {
            get => _ip;
            set => _ip = value;
        }

        /// <summary>
        /// Vhost
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
        /// rtmp address
        /// </summary>
        public string? TcUrl
        {
            get => _tcUrl;
            set => _tcUrl = value;
        }

        /// <summary>
        /// stream address
        /// </summary>
        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        /// <summary>
        /// parameter
        /// </summary>
        public string? Param
        {
            get => _param;
            set => _param = value;
        }
    }
}