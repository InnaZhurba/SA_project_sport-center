using Confluent.Kafka;

namespace MembershipManagementMicroservice.Kafka;

public class KafkaProducer : IKafkaProducerService
{
    private readonly ProducerConfig _producerConfig;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(ProducerConfig producerConfig, ILogger<KafkaProducer> logger)
    {
        _producerConfig = producerConfig;
        _logger = logger;
    }

    public async Task SendMessageAsync(string topic, string message)
    {
        try
        {
            
            using (var producer = new ProducerBuilder<string, string>(_producerConfig).Build())
            {
                var result = await producer.ProduceAsync(topic, new Message<string, string> { Key = null, Value = message });
                _logger.LogInformation($"Produced message to topic: {result.Topic}, partition: {result.Partition}, offset: {result.Offset}");
            }
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError($"Failed to produce message to topic: {topic}, error: {ex.Error.Reason}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while producing message: {ex.Message}");
            throw;
        }
    }
}