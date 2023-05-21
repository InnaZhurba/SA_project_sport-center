using RegistrationService.Models;

public interface IRegistrationRepository
{
        
    Task SaveRegistration(Registration registration);
    Task<List<Registration>> GetAllRegistrations();
}