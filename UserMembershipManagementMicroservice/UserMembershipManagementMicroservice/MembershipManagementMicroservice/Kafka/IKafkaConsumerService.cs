
namespace MembershipManagementMicroservice.Kafka;

public interface IKafkaConsumerService
{
    Task<string?> ConsumeMessagesAsync(string topic);
}