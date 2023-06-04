using Confluent.Kafka;

namespace MembershipManagementMicroservice.Kafka;

public class KafkaConsumer : IKafkaConsumerService
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(ConsumerConfig consumerConfig, ILogger<KafkaConsumer> logger)
    {
        _consumerConfig = consumerConfig;
        _logger = logger;
    }

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