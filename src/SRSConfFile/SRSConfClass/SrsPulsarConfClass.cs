using System;
using SharpPulsar.Builder;

namespace SrsConfFile.SRSConfClass
{
    [Serializable]
    public class SrsPulsarConfClass : SrsConfBase
    {
        private ConsumerConfigBuilder<byte[]>? _consumer;
        private ReaderConfigBuilder<byte[]>? _reader;
        private ProducerConfigBuilder<byte[]>? _producer;
        private PulsarClientConfigBuilder? _client;
        private string? _instanceName;
        private string? brokers;
        private bool? enabled;
        private string? topic;

        public SrsPulsarConfClass()
        {
            SectionsName = "pulsar";
        }


        public bool? Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public ConsumerConfigBuilder<byte[]>? Consumer
        {
            get => _consumer;
            set => _consumer = value;
        }
        public ReaderConfigBuilder<byte[]>? Reader
        {
            get => _reader;
            set => _reader = value;
        }
        public ProducerConfigBuilder<byte[]>? Producer
        {
            get => _producer;
            set => _producer = value;
        }
        public PulsarClientConfigBuilder? Client
        {
            get => _client;
            set => _client = value;
        }
        public string? Brokers
        {
            get => brokers;
            set => brokers = value;
        }

        public string? Topic
        {
            get => topic;
            set => topic = value;
        }

        public string? InstanceName
        {
            get => _instanceName;
            set => _instanceName = value;
        }
    }
}