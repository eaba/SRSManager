namespace SRSWeb.Messages
{
    public readonly record struct GlobalSrs
    {
        public string DeviceId { get; }
        public string Method { get; }
        
        public GlobalSrs(string deviceId, string method)
        {
            DeviceId = deviceId;
            Method = method;
        }
    }
}
