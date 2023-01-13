using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRSManager.Messages
{
    public readonly record struct PulsarSrsConfig(string Topic, string Tenant, string NameSpace, string BrokerUrl, string AdminUrl, string TrinoUrl);
}
