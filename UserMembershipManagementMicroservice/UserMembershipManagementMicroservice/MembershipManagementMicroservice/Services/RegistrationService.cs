using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;
using MembershipManagementMicroservice.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Services;

/// <summary>
///   This class implements the IRegistrationService interface.
///  It is used to interact with the User table of the database.
///  It contains methods for registering a user, getting a user by id, getting a user by email, getting all users with a
/// </summary>
public class RegistrationService : IRegistrationService
{
    
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ILogger<RegistrationService> _logger;
    private readonly IKafkaConsumerService _kafkaConsumerService;
    
    /// <summary>
    ///  This constructor injects the IRegistrationRepository and ILogger interfaces.
    /// </summary>
    /// <param name="registrationRepository">
    /// A IRegistrationRepository interface that is used to interact with the User table of the database.
    /// </param>
    /// <param name="logger">
    /// A ILogger interface that is used to log information to the console.
    /// </param>
    /// <param name="kafkaConsumerService">
    /// A IKafkaConsumerService interface that is used to consume messages from a Kafka topic.
    /// </param>
    public RegistrationService( IRegistrationRepository registrationRepository, ILogger<RegistrationService> logger, IKafkaConsumerService kafkaConsumerService)
    {
        _registrationRepository = registrationRepository;
        _logger = logger;
        _kafkaConsumerService = kafkaConsumerService;
    }
    
    /// <summary>
    ///  This method is used to register a user.
    /// </summary>
    /// <returns>
    /// A string that says whether the user was registered or not.
    /// </returns>
    public async Task<string> RegisterUserAsync()
    {
        _logger.LogInformation("Registering user unpacked...");
        
        // consume from kafka topic and register user
        var userJson = await _kafkaConsumerService.ConsumeMessagesAsync("user_post_topic");
        _logger.LogInformation($"User JSON: {userJson}");
        
        // unpack the user object from the JSON string
        var user = JsonConvert.DeserializeObject<User>(userJson);
        _logger.LogInformation($"User: {user}");
        
        // check if user id exists
        var userById = await _registrationRepository.GetUserAsync(user.Id.ToString());
        if (userById != null)
        {
            _logger.LogInformation($"User with id: {user.Id} already exist");
            return "User with id already exist";
        }

        // get all users with the same emails
        var userByEmail = await _registrationRepository.GetAllUsersWithEmailAsync(user.Email);
        // check if in userByEmail exist user with the same name
        if (userByEmail.Any(t => t.Username == user.Username))
        {
            _logger.LogInformation($"User with the same name already exist: {user.Username}");
            return "User with the same name already exist";
        }

        // Example implementation:
        try
        {
            await _registrationRepository.RegisterUserAsync(user);
            _logger.LogInformation($"User registered: {user}");
            //await _kafkaProducerService.SendMessageAsync("user_post_topic", JsonConvert.SerializeObject(user));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to register user: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
        return "User registered";
    }
    
    /// <summary>
    ///  This method is used to get a user by id.
    /// </summary>
    /// <param name="userId">
    /// A string that represents the id of the user.
    /// </param>
    /// <returns>
    /// A User object that represents the user.
    /// </returns>
    public async Task<User?> GetUserAsync(string userId)
    {
        try
        {
            var user = await _registrationRepository.GetUserAsync(userId);
            if (user == null)
            {
                _logger.LogInformation($"User with id: {userId} does not exist");
                return null;
            }

            _logger.LogInformation($"User retrieved: {user.Username}, {user.Email}");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve user: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
    }
    
    /// <summary>
    ///  This method is used to get a user by email.
    /// </summary>
    /// <param name="email">
    /// A string that represents the email of the user.
    /// </param>
    /// <returns>
    /// A User object that represents the user.
    /// </returns>
    public async Task<User> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _registrationRepository.GetUserByEmailAsync(email);
            _logger.LogInformation($"User retrieved: {user.Username}, {user.Email}");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve user: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
    }
    
    /// <summary>
    ///  This method is used to get all users with a specific email.
    /// </summary>
    /// <param name="email">
    /// A string that represents the email of the user.
    /// </param>
    /// <returns>
    /// A List of User objects that represents the users.
    /// </returns>
    public async Task<List<User>> GetAllUsersWithEmailAsync(string email)
    {
        try
        {
            var users = await _registrationRepository.GetAllUsersWithEmailAsync(email);
            _logger.LogInformation($"Users retrieved: {users}");
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve users: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
    }
    
    /// <summary>
    ///  This method is used to get a user by username and password.
    /// </summary>
    /// <param name="username">
    /// A string that represents the username of the user.
    /// </param>
    /// <param name="password">
    /// A string that represents the password of the user.
    /// </param>
    /// <returns>
    /// A User object that represents the user.
    /// </returns>
    public async Task<User> GetUserByUsernameAndPasswordAsync(string username, string password)
    {
        try
        {
            var user = await _registrationRepository.GetUserByUsernameAndPasswordAsync(username, password);
            _logger.LogInformation($"User retrieved: {user.Username}, {user.Email}");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve user: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
    }
    
    /// <summary>
    ///  This method is used to get a user by email and password.
    /// </summary>
    /// <param name="email">
    /// A string that represents the email of the user.
    /// </param>
    /// <param name="password">
    /// A string that represents the password of the user.
    /// </param>
    /// <returns>
    /// A User object that represents the user.
    /// </returns>
    public async Task<User> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        try
        {
            var user = await _registrationRepository.GetUserByEmailAndPasswordAsync(email, password);
            _logger.LogInformation($"User retrieved: {user.Username}, {user.Email}");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve user: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
    }
    
    /// <summary>
    ///  This method is used to edit a user.
    /// </summary>
    /// <param name="userId">
    /// A string that represents the id of the user.
    /// </param>
    /// <param name="user">
    /// A User object that represents the user.
    /// </param>
    /// <returns>
    /// A string that represents the result of the operation.
    /// </returns>
    public async Task<string> EditUserAsync(string userId, User user)
    {
        try
        {
            var userById = await _registrationRepository.GetUserAsync(userId);
            if (userById == null)
            {
                _logger.LogInformation($"User with id: {userId} does not exist");
                return "User with the same name already exist";
            }
            
            // edit userById
            userById.Username = user.Username;
            userById.Email = user.Email;
            userById.Password = user.Password;
            
            await _registrationRepository.EditUserAsync(userById);
            _logger.LogInformation($"User edited: {userById}");
            return "User edited";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to edit user: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
    }
}