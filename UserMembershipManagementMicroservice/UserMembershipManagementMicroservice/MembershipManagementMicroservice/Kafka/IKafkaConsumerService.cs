
namespace MembershipManagementMicroservice.Kafka;

/// <summary>
///   This interface is used to consume messages from Kafka.
///  It is implemented by the KafkaConsumer class.
/// </summary>
public interface IKafkaConsumerService
{
    /// <summary>
    ///  This method is used to consume messages from Kafka.
    /// </summary>
    /// <param name="topic">
    /// The topic argument represents the Kafka topic to consume messages from in string format
    /// </param>
    /// <returns>
    /// A string representing the consumed message from Kafka or null
    /// </returns>
    Task<string?> ConsumeMessagesAsync(string topic);
}