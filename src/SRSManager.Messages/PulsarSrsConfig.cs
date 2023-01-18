namespace SRSManager.Messages
{
    public readonly record struct PulsarSrsConfig(string Topic, string Tenant, string NameSpace, string BrokerUrl, string AdminUrl, string TrinoUrl);
}
