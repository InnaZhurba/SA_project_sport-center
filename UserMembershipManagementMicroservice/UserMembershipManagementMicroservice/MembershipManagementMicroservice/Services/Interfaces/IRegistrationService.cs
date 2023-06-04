using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Services.Interfaces;

public interface IRegistrationService
{
    Task<string> RegisterUserAsync();
    Task<User?> GetUserAsync(string userId);
    Task<User> GetUserByEmailAsync(string email);
    Task<List<User>> GetAllUsersWithEmailAsync(string email);
    Task<User> GetUserByUsernameAndPasswordAsync(string username, string password);
    Task<User> GetUserByEmailAndPasswordAsync(string email, string password);
    Task<string> EditUserAsync(string userId, User user);
}