using MembershipManagementMicroservice.Infrastructure;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;

namespace MembershipManagementMicroservice.Repository;

/// <summary>
///  This class is used to interact with the User table of the database.
///  It implements the IRegistrationRepository interface.
///  It is used by the RegistrationController class.
/// </summary>
public class RegistrationRepository : IRegistrationRepository
{
    private readonly CassandraConfig _cassandraConfig;

    /// <summary>
    ///  This constructor is used to inject the CassandraConfig into the RegistrationRepository class.
    /// </summary>
    /// <param name="cassandraConfig">
    /// The cassandraConfig argument represents the CassandraConfig dependency used to configure the Cassandra database
    /// </param>
    public RegistrationRepository(CassandraConfig cassandraConfig)
    {
        _cassandraConfig = cassandraConfig;
    }

    /// <summary>
    ///  This method is used to register a user in the database.
    /// </summary>
    /// <param name="user">
    /// The user argument represents the user to be registered in the database 
    /// </param>
    public async Task RegisterUserAsync(User user)
    {
        string query = "INSERT INTO users (Id, User_name, Email, Password) VALUES (?, ?, ?, ?)";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            user.Id, 
            user.Username, 
            user.Email, 
            user.Password);
    }

    /// <summary>
    ///  This method is used to get a user from the database.
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents the id of the user to be retrieved from the database
    /// </param>
    /// <returns>
    /// The method returns a User object if the user is found in the database, otherwise it returns null
    /// </returns>
    public async Task<User?> GetUserAsync(string userId)
    { 
        string query = "SELECT * FROM users WHERE Id = ?";
        var id = Guid.Parse(userId);
        var result = await _cassandraConfig.ExecuteQuery(query, id);
        var row = result.FirstOrDefault();
        return (row == null
            ? null
            : new User
            {
                Id = row.GetValue<Guid>("id"),
                Username = row.GetValue<string>("user_name"),
                Email = row.GetValue<string>("email"),
                Password = row.GetValue<string>("password")
            });
    }
    
    /// <summary>
    ///  This method is used to get a user from the database.
    /// </summary>
    /// <param name="email">
    /// The email argument represents the email of the user to be retrieved from the database
    /// </param>
    /// <returns>
    /// The method returns a User object if the user is found in the database, otherwise it returns null 
    /// </returns>
    public async Task<User?> GetUserByEmailAsync(string email)
    { 
        string query = "SELECT * FROM users WHERE Email = ? ALLOW FILTERING";
        var result = await _cassandraConfig.ExecuteQuery(query, email);
        var row = result.FirstOrDefault();
        return (row == null
            ? null
            : new User
            {
                Id = row.GetValue<Guid>("id"),
                Username = row.GetValue<string>("user_name"),
                Email = row.GetValue<string>("email"),
                Password = row.GetValue<string>("password")
            });
    }
    
    /// <summary>
    ///     This method is used to get all users with a given email from the database.
    /// </summary>
    /// <param name="email">
    ///    The email argument represents the email of the users to be retrieved from the database
    /// </param>
    /// <returns>
    ///   The method returns a list of User objects if the users are found in the database, otherwise it returns an empty list
    /// </returns>
    public async Task<List<User>> GetAllUsersWithEmailAsync(string email)
    {  
        string query = "SELECT * FROM users WHERE Email = ? ALLOW FILTERING";
        var result = await _cassandraConfig.ExecuteQuery(query, email);
        var users = new List<User>();
        foreach (var row in result)
        {
            users.Add(new User
            {
                Id = row.GetValue<Guid>("id"),
                Username = row.GetValue<string>("user_name"),
                Email = row.GetValue<string>("email"),
                Password = row.GetValue<string>("password")
            });
        }
        return users;
    }

    /// <summary>
    ///     This method is used to get user with a given username and password from the database.
    /// </summary>
    /// <param name="username">
    ///   The username argument represents the username of the users to be retrieved from the database
    /// </param>
    /// <param name="password">
    ///  The password argument represents the password of the users to be retrieved from the database
    /// </param>
    /// <returns>
    ///  The method returns a user object from User objects if the users are found in the database, otherwise it returns an empty list
    /// </returns>
    public async Task<User?> GetUserByUsernameAndPasswordAsync(string username, string password)
    { 
        string query = "SELECT * FROM users WHERE User_name = ? AND Password = ? ALLOW FILTERING";
        var result = await _cassandraConfig.ExecuteQuery(query, username, password);
        var row = result.FirstOrDefault();
        return (row == null
            ? null
            : new User
            {
                Id = row.GetValue<Guid>("id"),
                Username = row.GetValue<string>("user_name"),
                Email = row.GetValue<string>("email"),
                Password = row.GetValue<string>("password")
            });
    }
    
    /// <summary>
    ///   This method is used to get user with a given email and password from the database.
    /// </summary>
    /// <param name="email">
    ///  The email argument represents the email of the users to be retrieved from the database
    /// </param>
    /// <param name="password">
    /// The password argument represents the password of the users to be retrieved from the database
    /// </param>
    /// <returns>
    /// The method returns a user object from User objects if the users are found in the database, otherwise it returns an empty list
    /// </returns>
    public async Task<User?> GetUserByEmailAndPasswordAsync(string email, string password)
    { 
        string query = "SELECT * FROM users WHERE Email = ? AND Password = ? ALLOW FILTERING";
        var result = await _cassandraConfig.ExecuteQuery(query, email, password);
        var row = result.FirstOrDefault();
        return (row == null
            ? null
            : new User
            {
                Id = row.GetValue<Guid>("id"),
                Username = row.GetValue<string>("user_name"),
                Email = row.GetValue<string>("email"),
                Password = row.GetValue<string>("password")
            });
    }
    
    /// <summary>
    ///  This method is used to edit a user in the database.
    /// </summary>
    /// <param name="user">
    /// The user argument represents the user to be edited in the database
    /// </param>
    /// <returns>
    /// The method returns a string message if the user is edited successfully
    /// </returns>
    public async Task<string> EditUserAsync(User user)
    {
        string query = "UPDATE users SET User_name = ?, Email = ?, Password = ? WHERE Id = ?";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            user.Username, 
            user.Email, 
            user.Password,
            user.Id);
        return "User updated successfully";
    }
}