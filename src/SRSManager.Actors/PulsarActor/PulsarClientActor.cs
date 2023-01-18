using SharpPulsar;
using SharpPulsar.Builder;

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
