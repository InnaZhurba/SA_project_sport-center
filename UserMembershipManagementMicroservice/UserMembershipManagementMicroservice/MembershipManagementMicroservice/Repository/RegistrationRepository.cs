using MembershipManagementMicroservice.Infrastructure;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;

namespace MembershipManagementMicroservice.Repository;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly CassandraConfig _cassandraConfig;

    public RegistrationRepository(CassandraConfig cassandraConfig)
    {
        _cassandraConfig = cassandraConfig;
    }

    public async Task RegisterUserAsync(User user)
    {
        string query = "INSERT INTO users (Id, User_name, Email, Password) VALUES (?, ?, ?, ?)";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            user.Id, 
            user.Username, 
            user.Email, 
            user.Password);
    }

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
    
    // GetUserByEmailAsync
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
    
    // GetAllUsersWithEmailAsync
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

    // GetUserByUsernameAndPasswordAsync
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
    
    // GetUserByEmailAndPasswordAsync
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
    
    //  EditUserAsync(userById)
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