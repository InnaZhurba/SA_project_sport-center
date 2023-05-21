using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RegistrationService.Messaging
{
    public class KafkaMessageProducer : IMessageProducer
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaMessageProducer> _logger;

        public KafkaMessageProducer(IConfiguration configuration, ILogger<KafkaMessageProducer> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task ProduceMessage(string message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                ClientId = _configuration["Kafka:ClientId"]
            };

            using var producer = new ProducerBuilder<string, string>(config).Build();

            try
            {
                var deliveryResult = await producer.ProduceAsync(_configuration["Kafka:Topic"], new Message<string, string> { Value = message });
                _logger.LogInformation($"Message '{deliveryResult.Value}' sent to topic '{deliveryResult.TopicPartitionOffset}'");
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError($"Error producing message: {ex.Error.Reason}");
            }
        }
    }
}