using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Services.Interfaces;

/// <summary>
///     This interface is used to interact with the User table of the database.
///     It is implemented by the RegistrationService class.
///   It contains methods for registering a user, getting a user by id, getting a user by email, getting all users with a
/// </summary>
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