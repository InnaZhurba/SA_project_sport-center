using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Repository.Interfaces;

public interface IRegistrationRepository
{
    Task RegisterUserAsync(User user);
    Task<User?> GetUserAsync(string userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<List<User>> GetAllUsersWithEmailAsync(string email);
    Task<User?> GetUserByUsernameAndPasswordAsync(string username, string password);
    Task<User?> GetUserByEmailAndPasswordAsync(string email, string password);
    Task<string> EditUserAsync(User user);
}