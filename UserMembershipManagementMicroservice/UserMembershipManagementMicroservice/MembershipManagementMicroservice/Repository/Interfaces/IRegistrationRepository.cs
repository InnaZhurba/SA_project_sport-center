using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Repository.Interfaces;

/// <summary>
///  This interface is used to interact with the User table of the database.
///  It is implemented by the RegistrationRepository class.
///  It contains methods for registering a user, getting a user by id, getting a user by email, getting all users with a given email, getting a user by username and password, getting a user by email and password, and editing a user.
///  It is used by the RegistrationController class.
/// </summary>
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