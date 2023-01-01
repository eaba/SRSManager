using System;

namespace SrsConfFile.SRSConfClass
{
    [Serializable]
    public class SrsSrtServerConfClass : SrsConfBase
    {
        private string? _instanceName;
        private int? connect_timeout;
        private string? default_app;
        private bool? enabled;
        private ushort? listen;
        private int? maxbw;
        private int? peerlatency;
        private int? recvlatency;
        private int? sendbuf;
        private int? recvbuf;
        private bool? tlpktdrop;
        private bool? tsbpdmode;
        private int? latency;
        private string? passphrase;
        private ushort? pbkeylen;
        public SrsSrtServerConfClass()
        {
            SectionsName = "srt_server";
        }

        public string? Default_app
        {
            get => default_app;
            set => default_app = value;
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

        public int? Maxbw
        {
            get => maxbw;
            set => maxbw = value;
        }

        public int? Connect_timeout
        {
            get => connect_timeout;
            set => connect_timeout = value;
        }

        public int? Peerlatency
        {
            get => peerlatency;
            set => peerlatency = value;
        }

        public int? Recvlatency
        {
            get => recvlatency;
            set => recvlatency = value;
        }


        public string? InstanceName
        {
            get => _instanceName;
            set => _instanceName = value;
        }
        
        public int? Sendbuf
        {
            get => sendbuf;
            set => sendbuf = value;
        }
        public int? Recvbuf
        {
            get => recvbuf;
            set => recvbuf = value;
        }
        public bool? Tlpktdrop
        {
            get => tlpktdrop;
            set => tlpktdrop = value;
        }
        public bool? Tsbpdmode
        {
            get => tsbpdmode;
            set => tsbpdmode = value;
        }
        public int? Latency
        {
            get => latency;
            set => latency = value;
        }

        public string? Passphrase
        {
            get => passphrase;
            set => passphrase = value;
        }
        public ushort? Pbkeylen
        {
            get => pbkeylen;
            set => pbkeylen = value;
        }
    }
}