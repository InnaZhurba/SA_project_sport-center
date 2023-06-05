
namespace MembershipManagementMicroservice.Kafka;

/// <summary>
///      This interface is used to produce messages to Kafka.
/// </summary>
public interface IKafkaProducerService
{
    /// <summary>
    ///   This method is used to produce messages to Kafka. 
    /// </summary>
    /// <param name="topic">
    /// The topic argument represents the Kafka topic to produce messages to in string format
    /// </param>
    /// <param name="message">
    /// The message argument represents the message to produce to Kafka in string format
    /// </param>
    /// <returns>
    /// A Task representing the asynchronous operation of producing a message to Kafka or null 
    /// </returns>
    Task SendMessageAsync(string topic, string message);
}