using System;

namespace SrsConfFile.SRSConfClass
{
    [Serializable]
    public class BlackHole : SrsConfBase
    {
        private bool? enabled;
        private string? addr;

        public BlackHole()
        {
            SectionsName = "black_hole";
        }

        public bool? Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public string? Addr
        {
            get => addr;
            set => addr = value;
        }
    }
    
    [Serializable]
    public class Tcp : SrsConfBase
    {
        private bool? enabled;
        private ushort? listen;

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
    }
   
    [Serializable]
    public class SrsRtcServerConfClass : SrsConfBase
    {
        private bool? enabled;
        private ushort? listen;
        private Tcp? tcp;
        private string? protocol;
        private string? candidate;
        private bool? use_auto_detect_network_ip;
        private string?  ip_family;
        private bool? api_as_candidates;
        private bool? resolve_api_domain;
        private bool? keep_api_domain;
        private bool? ecdsa;
        private bool? encrypt;
        private ushort? reuseport;
        private bool? merge_nalus;
        private BlackHole? black_hole;

        public SrsRtcServerConfClass()
        {
            SectionsName = "rtc_server";
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

        public string? Candidate
        {
            get => candidate;
            set => candidate = value;
        }

        public bool? Ecdsa
        {
            get => ecdsa;
            set => ecdsa = value;
        }
                
        public Tcp? Tcp
        {
            get => tcp;
            set => tcp = value;
        }

        public bool? Encrypt
        {
            get => encrypt;
            set => encrypt = value;
        }

        public ushort? Reuseport
        {
            get => reuseport;
            set => reuseport = value;
        }

        public bool? Merge_nalus
        {
            get => merge_nalus;
            set => merge_nalus = value;
        }
        public string? Protocol
        {
            get => protocol;
            set => protocol = value;
        }

        public bool? UseAutoDetectNetworkIp
        {
            get => use_auto_detect_network_ip;
            set => use_auto_detect_network_ip = value;
        }
        public string? IpFamily
        {
            get => ip_family;
            set => ip_family = value;
        }

        public bool? ApiAsCandidates
        {
            get => api_as_candidates;
            set => api_as_candidates = value;
        }

        public bool? ResolveApiDomain
        {
            get => resolve_api_domain;
            set => resolve_api_domain = value;
        }
        public bool? KeepApiDomain
        {
            get => keep_api_domain;
            set => keep_api_domain = value;
        }
        public BlackHole? Black_hole
        {
            get => black_hole;
            set => black_hole = value;
        }

    }
}