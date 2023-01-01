using System;

namespace SrsConfFile.SRSConfClass
{
    [Serializable]
    public class Sip : SrsConfBase
    {
        private ushort? ack_timeout;
        private bool? auto_play;

        private bool? enabled;
        private bool? invite_port_fixed;
        private ushort? keepalive_timeout;
        private ushort? listen;
        private ushort? query_catalog_interval;
        private string? realm;
        private string? serial;

        public Sip()
        {
            SectionsName = "sip";
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

        public string? Serial
        {
            get => serial;
            set => serial = value;
        }

        public string? Realm
        {
            get => realm;
            set => realm = value;
        }

        public ushort? Ack_timeout
        {
            get => ack_timeout;
            set => ack_timeout = value;
        }

        public ushort? Keepalive_timeout
        {
            get => keepalive_timeout;
            set => keepalive_timeout = value;
        }

        public bool? Auto_play
        {
            get => auto_play;
            set => auto_play = value;
        }

        public bool? Invite_port_fixed
        {
            get => invite_port_fixed;
            set => invite_port_fixed = value;
        }

        public ushort? Query_catalog_interval
        {
            get => query_catalog_interval;
            set => query_catalog_interval = value;
        }
    }

    public enum CasterEnum
    {
        mpegts_over_udp,
        rtsp,
        flv,
        gb28181
    }

    [Serializable]
    public class SrsStreamCasterConfClass : SrsConfBase
    {
        // To fix the latest gb28181 format
        private string? _instanceName;

        private bool? audio_enable;
        /*use for gb28181
          # The rtp packet idle waiting time, if no packet is received within the specified time
          # The rtp listening connection is automatically stopped, and the BYE command is sent
        */

        private bool? auto_create_channel;
        private CasterEnum? caster;
        private bool? enabled;
        private string? host; //use for gb28181 ,can be domain or ip address
        private ushort? listen; //open a port for pull stream

        private string? output; //rtmp path use for player
        /*use for gb28181
         # Whether to wait for the key frame before forwarding,
         # off:No need to wait, just forward
         # on:Wait for the first key frame before forwarding
        */

        private ushort? rtp_idle_timeout;
        private ushort? rtp_port_max; //user for rtsp&gb28181 caster
        private ushort? rtp_port_min; //use for rtsp&gb28181 caster
        /// <summary>
        ///  jitterbuffer_enable
        ///  # Whether to enable rtp buffering
        ///  # After opening, it can effectively solve problems such as rtp disorder
        /// </summary>
        private bool? jitterbuffer_enable;

        private Sip? ssip;
        /*use for gb28181
          # Whether to forward the audio stream
          # Currently only aac format is supported, so the device needs to support aac format
          # on:forward audio  
          # off:No audio forwarding, only video
          #*Note*!!!: flv only supports 11025 22050 44100
          # If the device does not have any of the three types, it will automatically select a format when forwarding
          # At the same time, the header of adts will be encapsulated in flv aac raw data
          # In this case, the player automatically selects the sampling frequency through the adts header
          # Like ffplay, vlc can be, but flash has no sound,
          # Because of flash, only support 11025 22050 44100
         */



        private bool? wait_keyframe;

        public SrsStreamCasterConfClass()
        {
            SectionsName = "stream_caster";
        }

        public bool? Jitterbuffer_Enable
        {
            get => jitterbuffer_enable;
            set => jitterbuffer_enable = value;
        }

        public Sip? sip
        {
            get => ssip;
            set => ssip = value;
        }

        public bool? Auto_create_channel
        {
            get => auto_create_channel;
            set => auto_create_channel = value;
        }

        public bool? Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public CasterEnum? Caster
        {
            get => caster;
            set => caster = value;
        }

        public string? Output
        {
            get => output;
            set => output = value;
        }

        public ushort? Listen
        {
            get => listen;
            set => listen = value;
        }

        public ushort? Rtp_port_min
        {
            get => rtp_port_min;
            set => rtp_port_min = value;
        }

        public ushort? Rtp_port_max
        {
            get => rtp_port_max;
            set => rtp_port_max = value;
        }

        public string? Host
        {
            get => host;
            set => host = value;
        }

        public bool? Audio_enable
        {
            get => audio_enable;
            set => audio_enable = value;
        }

        public bool? Wait_keyframe
        {
            get => wait_keyframe;
            set => wait_keyframe = value;
        }

        public ushort? Rtp_idle_timeout
        {
            get => rtp_idle_timeout;
            set => rtp_idle_timeout = value;
        }

        public string? InstanceName
        {
            get => _instanceName;
            set => _instanceName = value;
        }
    }
}