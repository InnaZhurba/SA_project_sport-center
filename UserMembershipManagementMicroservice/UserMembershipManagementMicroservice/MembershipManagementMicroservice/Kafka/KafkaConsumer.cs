using Confluent.Kafka;

namespace MembershipManagementMicroservice.Kafka;

/// <summary>
///      This class is used to consume messages from Kafka.
///    It implements the IKafkaConsumerService interface.
///  It is used by the KafkaConsumerController class.
/// </summary>
public class KafkaConsumer : IKafkaConsumerService
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly ILogger<KafkaConsumer> _logger;

    /// <summary>
    ///    This constructor is used to inject the ConsumerConfig and ILogger(KafkaConsumer) dependencies.
    /// </summary>
    /// <param name="consumerConfig">
    /// The consumerConfig argument represents the ConsumerConfig dependency used to configure the Kafka consumer
    /// </param>
    /// <param name="logger">
    /// The logger argument represents the ILogger(KafkaConsumer) dependency used to log messages to the console
    /// </param>
    public KafkaConsumer(ConsumerConfig consumerConfig, ILogger<KafkaConsumer> logger)
    {
        _consumerConfig = consumerConfig;
        _logger = logger;
    }

    /// <summary>
    ///  This method is used to consume messages from Kafka.
    ///  It accepts a topic as an argument and returns a string representing the consumed message from Kafka or null.
    /// </summary>
    /// <param name="topic">
    /// The topic argument represents the Kafka topic to consume messages from in string format
    /// </param>
    /// <returns>
    /// A string representing the consumed message from Kafka or null 
    /// </returns>
    ///  <exception cref="ConsumeException">
    ///  Thrown when an error occurs while consuming messages from Kafka
    ///  </exception>
    ///  <exception cref="OperationCanceledException">
    ///  Thrown when the consumer is closed
    ///  </exception>
    ///  <exception cref="Exception">
    ///  Thrown when an error occurs while consuming messages from Kafka
    ///  </exception>
    ///  <exception cref="ArgumentException">
    ///  Thrown when the topic argument is null or empty
    ///  </exception>
    public async Task<string?> ConsumeMessagesAsync(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
        consumer.Subscribe(topic);

        try
        {
            while (true)
            {
                //var consumeResult = await Task.Run(() => consumer.Consume(CancellationToken.None));

                var consumeResult = consumer.Consume();

                _logger.LogInformation($"Consumed message from topic: {consumeResult.Topic}, partition: {consumeResult.Partition}, offset: {consumeResult.Offset}");
                _logger.LogInformation($"Message: {consumeResult.Message.Value}");
                    
                // Process the consumed message
                var message = consumeResult.Message;

                // Commit the consumed message offset
                await Task.Run(() => consumer.Commit(consumeResult));
                    
                return message.Value;
            }
        }
        catch (ConsumeException ex)
        {
            _logger.LogError($"Failed to consume message: {ex.Error.Reason}");
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
            _logger.LogInformation("Consumer was closed.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while consuming messages: {ex.Message}");
            throw;
        }

        return null;
    }

}