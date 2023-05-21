using RegistrationService.DB;
using RegistrationService.Messaging;
using RegistrationService.Models;
using Microsoft.Extensions.Logging;

namespace RegistrationService.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IMessageProducer _messageProducer;
        private readonly ILogger<RegistrationService> _logger;

        public RegistrationService(IRegistrationRepository registrationRepository, IMessageProducer messageProducer, ILogger<RegistrationService> logger)
        {
            _registrationRepository = registrationRepository;
            _messageProducer = messageProducer;
            _logger = logger;
        }

        public void Register(Registration registration)
        {
            try
            {
                _registrationRepository.SaveRegistration(registration);
                _messageProducer.ProduceMessage(registration.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving registration in DB");
                throw; // Rethrow the exception to be handled at the higher level
            }
        }
        
        // get all registrations
        public List<Registration> GetAllRegistrations()
        {
            try
            {
                var registrations = _registrationRepository.GetAllRegistrations();

                _logger.LogInformation($"Found {registrations.Result.Count} registrations");
                //show all registrations Name and Email
                foreach (var registration in registrations.Result)
                {
                    _logger.LogInformation($"Registration: {registration.FirstName} - {registration.Email}");
                }
                //convert to list
                return registrations.Result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all registrations");
                throw; // Rethrow the exception to be handled at the higher level
            }
        }
    }
}