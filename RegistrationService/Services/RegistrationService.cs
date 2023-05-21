using RegistrationService.DB;
using RegistrationService.Messaging;
using RegistrationService.Models;

namespace RegistrationService.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IMessageProducer _messageProducer;

        public RegistrationService(IRegistrationRepository registrationRepository, IMessageProducer messageProducer)
        {
            _registrationRepository = registrationRepository;
            _messageProducer = messageProducer;
        }

        public void Register(Registration registration)
        {
            //_registrationRepository.SaveRegistration(registration);
            _registrationRepository.SaveRegistration(registration);
            _messageProducer.ProduceMessage(registration.ToString());
        }
    }
}