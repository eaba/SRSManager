using SharpPulsar.Builder;
using SRSManageCommon.ManageStructs;

namespace SRSManager.Messages
{
    public readonly record struct GlobalSrs
    {
        public string? DeviceId { get; }
        public string? Method { get; }
       /// public string UserId { get; } 
        public ushort? Short { get; } 
        public bool? Enable { get; }
        public string? Path { get; }    
        public GlobalModule? Gm { get; }
        public PulsarClientConfigBuilder? Client { get; }
        public ProducerConfigBuilder<byte[]>? Producer { get; }
        public ReaderConfigBuilder<byte[]>? Reader { get; }
        public ConsumerConfigBuilder<byte[]>? Consumer { get; } 
        public byte[]? Data { get; }
        public GlobalSrs(string deviceId, string method)
        {
            DeviceId = deviceId;
            Method = method;
        }
        public GlobalSrs(string deviceId, byte[] data, string method)
        {
            DeviceId = deviceId;
            Data = data;    
            Method = method;
        }
        public GlobalSrs(string deviceId, ushort sh, string method)
        {
            DeviceId = deviceId;
            Short= sh;
            Method = method;
        }
        public GlobalSrs(string deviceId, GlobalModule gm, string method)
        {
            DeviceId = deviceId;
            Gm= gm;
            Method = method;
        }
        public GlobalSrs(string deviceId, PulsarClientConfigBuilder client, string method)
        {
            DeviceId = deviceId;
            Client= client;
            Method = method;
        }
        public GlobalSrs(string deviceId, ConsumerConfigBuilder<byte[]> consumer, string method)
        {
            DeviceId = deviceId;
            Consumer= consumer;
            Method = method;
        }
        public GlobalSrs(string deviceId, ProducerConfigBuilder<byte[]> producer, string method)
        {
            DeviceId = deviceId;
            Producer= producer;
            Method = method;
        }
        public GlobalSrs(string deviceId, ReaderConfigBuilder<byte[]> reader, string method)
        {
            DeviceId = deviceId;
            Reader = reader;
            Method = method;
        }
        public GlobalSrs(string deviceId, bool enable, string method)
        {
            DeviceId = deviceId;
            Enable= enable;
            Method = method;
        }
        public GlobalSrs(string deviceId, string path, string method)
        {
            DeviceId = deviceId;
            Path = path;    
            Method = method;
        }
    }
}
