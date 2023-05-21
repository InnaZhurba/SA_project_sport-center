using RegistrationService.Models;

public interface IRegistrationRepository
{
        
    Task SaveRegistration(Registration registration);
}