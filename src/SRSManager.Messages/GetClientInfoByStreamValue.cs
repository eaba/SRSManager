
namespace SRSManager.Messages
{
    public readonly record struct GetClientInfoByStreamValue(string StreamId, string Tenant, string NameSpace, string TrinoUrl);
    public readonly record struct GetOnlinePlayerByDeviceId(string DeviceId, string Tenant, string NameSpace, string TrinoUrl);
    public readonly record struct GetOnlinePlayer(string Tenant, string NameSpace, string TrinoUrl);
    public readonly record struct GetOnPublishMonitorListById(string DeviceId, string Tenant, string NameSpace, string TrinoUrl);
    public readonly record struct GetOnPublishMonitorList(string Tenant, string NameSpace, string TrinoUrl);
    public readonly record struct GetOnPublishMonitorById(string Id, string Tenant, string NameSpace, string TrinoUrl);
    public readonly record struct GetOnvifMonitorIngestTemplate(string Username, string Password, string RtspUrl);
}
