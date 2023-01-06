using SharpPulsar.Builder;
using SRSManageCommon.ManageStructs;

namespace SRSManager.Messages
{
    public readonly record struct GlobalSrs
    {
        public string DeviceId { get; }
        public string Method { get; }
       /// public string UserId { get; } 
        public ushort Short { get; } 
        public bool Enable { get; }
        public string Path { get; }    
        public GlobalModule? Gm { get; }
        public PulsarClientConfigBuilder Client { get; }
        public ProducerConfigBuilder<byte[]> Producer { get; }
        public ReaderConfigBuilder<byte[]> Reader { get; }
        public ConsumerConfigBuilder<byte[]> Consumer { get; } 
        public byte[] Data { get; }
        public GlobalSrs(string deviceId, string method)
        {
            (DeviceId, Method, Enable, Short, Path, Gm, Client, Producer, Consumer, Reader, Data)
          = (deviceId, method, false, 0, string.Empty, null!, null!, null!, null!, null!, null!);
        }
        public GlobalSrs(string deviceId, byte[] data, string method)
        {
            (DeviceId, Method, Enable, Short, Path, Gm, Client, Producer, Consumer, Reader, Data)
          = (deviceId, method, false, 0, string.Empty, null!, null!, null!, null!, null!, data);
        }
        public GlobalSrs(string deviceId, ushort sh, string method)
        {
            (DeviceId, Method, Enable, Short, Path, Gm, Client, Producer, Consumer, Reader, Data)
           = (deviceId, method, false, sh, string.Empty, null!, null!, null!, null!, null!, null!);
        }
        public GlobalSrs(string deviceId, GlobalModule gm, string method)
        {
            (DeviceId, Method, Enable, Short, Path, Gm, Client, Producer, Consumer, Reader, Data)
           = (deviceId, method, false, 0, string.Empty, gm, null!, null!, null!, null!, null!);
        }
        public GlobalSrs(string deviceId, PulsarClientConfigBuilder client, string method)
        {
            (DeviceId, Method, Enable, Short, Path, Gm, Client, Producer, Consumer, Reader, Data)
           = (deviceId, method, false, 0, string.Empty, null!, client, null!, null!, null!, null!);
        }
        public GlobalSrs(string deviceId, ConsumerConfigBuilder<byte[]> consumer, string method)
        {
            (DeviceId, Method, Enable, Short, Path, Gm, Client, Producer, Consumer, Reader, Data)
           = (deviceId, method, false, 0, string.Empty, null!, null!, null!, consumer, null!, null!);
        }
        public GlobalSrs(string deviceId, ProducerConfigBuilder<byte[]> producer, string method)
        {
            (DeviceId, Method, Enable, Short, Path, Gm, Client, Producer, Consumer, Reader, Data)
           = (deviceId, method, false, 0, string.Empty, null!, null!, producer, null!, null!, null!);
        }
        public GlobalSrs(string deviceId, ReaderConfigBuilder<byte[]> reader, string method)
        {
            (DeviceId, Method, Enable, Short, Path, Gm, Client, Producer, Consumer, Reader, Data)
           = (deviceId, method, false, 0, string.Empty, null!, null!, null!, null!, reader, null!);
        }
        public GlobalSrs(string deviceId, bool enable, string method)
        {
            (DeviceId, Method, Enable, Short, Path, Gm, Client, Producer, Consumer, Reader, Data)
          = (deviceId, method, enable, 0, string.Empty, null!, null!, null!, null!, null!, null!);
        }
        public GlobalSrs(string deviceId, string path, string method)
        {
            (DeviceId, Method, Enable, Short, Path, Gm, Client, Producer, Consumer, Reader, Data)
           = (deviceId, method, false, 0, path, null!, null!, null!, null!, null!, null!);
        }
    }
}
