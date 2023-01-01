using System;

namespace SrsConfFile.SRSConfClass
{
    [Serializable]
    public class RawApi : SrsConfBase
    {
        private bool? allow_reload;
        private bool? enabled;

        public RawApi()
        {
            SectionsName = "raw_api";
        }

        public bool? Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public bool? Allow_reload
        {
            get => allow_reload;
            set => allow_reload = value;
        }

    }

    [Serializable]
    public class SrsHttpApiConfClass : SrsConfBase
    {
        private bool? crossdomain; //if true can crossdomain
        private bool? enabled;
        private string? instanceName;
        private ushort? listen; //httpapi port
        private RawApi? raw_Api;

        public SrsHttpApiConfClass()
        {
            SectionsName = "http_api";
        }


        public bool? Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public ushort? Listen
        {
            get => listen;
            set => listen = value;
        }

        public bool? Crossdomain
        {
            get => crossdomain;
            set => crossdomain = value;
        }

        public RawApi? Raw_Api
        {
            get => raw_Api;
            set => raw_Api = value;
        }

        public string? InstanceName
        {
            get => instanceName;
            set => instanceName = value;
        }
    }
}