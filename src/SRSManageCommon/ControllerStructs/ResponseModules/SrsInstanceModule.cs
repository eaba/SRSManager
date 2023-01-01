using System;

namespace SRSManageCommon.ControllerStructs.ResponseModules
{
    /// <summary>
    /// SRS instance model
    /// </summary>
    [Serializable]
    public class SrsInstanceModule
    {
        private string _configPath = null!;
        private string _deviceId = null!;
        private bool _isInit;
        private bool _isRunning;
        private string _pidValue = null!;
        private string _srsInstanceWorkPath = null!;
        private string _srsProcessWorkPath = null!;

        /// <summary>
        /// device ID
        /// </summary>
        public string DeviceId
        {
            get => _deviceId;
            set => _deviceId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Whether to initialize
        /// </summary>
        public bool IsInit
        {
            get => _isInit;
            set => _isInit = value;
        }

        /// <summary>
        /// is it running
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
            set => _isRunning = value;
        }

        /// <summary>
        /// configuration file path
        /// </summary>
        public string ConfigPath
        {
            get => _configPath;
            set => _configPath = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// pid value
        /// </summary>
        public string PidValue
        {
            get => _pidValue;
            set => _pidValue = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// srs process running directory
        /// </summary>
        public string SrsProcessWorkPath
        {
            get => _srsProcessWorkPath;
            set => _srsProcessWorkPath = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// srs instance running directory
        /// </summary>
        public string SrsInstanceWorkPath
        {
            get => _srsInstanceWorkPath;
            set => _srsInstanceWorkPath = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}