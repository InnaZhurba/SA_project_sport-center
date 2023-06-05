using Confluent.Kafka;

namespace MembershipManagementMicroservice.Kafka;

/// <summary>
///      This class is used to produce messages to Kafka.
/// </summary>
public class KafkaProducer : IKafkaProducerService
{
    private readonly ProducerConfig _producerConfig;
    private readonly ILogger<KafkaProducer> _logger;

    /// <summary>
    ///     This constructor is used to inject the ProducerConfig and ILogger(KafkaProducer) dependencies.
    /// </summary>
    /// <param name="producerConfig">
    /// The producerConfig argument represents the ProducerConfig dependency used to configure the Kafka producer
    /// </param>
    /// <param name="logger">
    /// The logger argument represents the ILogger(KafkaProducer) dependency used to log messages to the console
    /// </param>
    ///  <exception cref="ArgumentNullException">
    ///  Thrown when the producerConfig or logger argument is null
    ///  </exception>
    ///  <exception cref="ArgumentException">
    ///  Thrown when the producerConfig or logger argument is invalid
    ///  </exception>
    public KafkaProducer(ProducerConfig producerConfig, ILogger<KafkaProducer> logger)
    {
        _producerConfig = producerConfig;
        _logger = logger;
    }

    /// <summary>
    ///  This method is used to produce messages to Kafka.
    ///  It accepts a topic and message as arguments and returns a Task representing the asynchronous operation of producing a message to Kafka or null.
    /// </summary>
    /// <param name="topic">
    /// The topic argument represents the Kafka topic to produce messages to in string format
    /// </param>
    /// <param name="message">
    /// The message argument represents the message to produce to Kafka in string format
    /// </param>
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