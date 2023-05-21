using RegistrationService.Models;

public interface IRegistrationRepository
{
        
    Task SaveRegistration(Registration registration);
    Task<List<Registration>> GetAllRegistrations();
    Task<List<Registration>> GetRegistrationsByEmail(string email);
}