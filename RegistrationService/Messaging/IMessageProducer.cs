namespace RegistrationService.Messaging;

public interface IMessageProducer
{
    Task ProduceMessage(string message);
}