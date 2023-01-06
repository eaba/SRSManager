using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPulsar;
using SharpPulsar.Builder;
using SharpPulsar.User;

namespace SRSManager.Actors.PulsarActor
{
    internal class PulsarClientActor
    {
        private PulsarClient _client;
        public PulsarClientActor(PulsarSystem pulsarSystem, ConsumerConfigBuilder<byte[]> consumer)
        {

        }
    }
    internal class PulsarClientReaderActor
    {
        private PulsarClient _client;
        public PulsarClientReaderActor(PulsarSystem pulsarSystem, ReaderConfigBuilder<byte[]> reader)
        {

        }
    }
    internal class PulsarClientProducerActor
    {
        private PulsarClient _client;
        public PulsarClientProducerActor(PulsarSystem pulsarSystem, ProducerConfigBuilder<byte[]> producer)
        {

        }
    }
}
