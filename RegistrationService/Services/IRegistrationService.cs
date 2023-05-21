using RegistrationService.Models;

public interface IRegistrationService
{
    void Register(Registration registration);
    List<Registration> GetAllRegistrations();
    List<Registration> GetRegistrationsByEmail(string email);
}