using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;
using MembershipManagementMicroservice.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Services;

public class RegistrationService : IRegistrationService
{
    
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ILogger<RegistrationService> _logger;
    private readonly IKafkaConsumerService _kafkaConsumerService;
    
    public RegistrationService( IRegistrationRepository registrationRepository, ILogger<RegistrationService> logger, IKafkaConsumerService kafkaConsumerService)
    {
        _registrationRepository = registrationRepository;
        _logger = logger;
        _kafkaConsumerService = kafkaConsumerService;
    }
    
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
    
    // GetUserByEmailAsync
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
    
    // get all users with email
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
    
    //GetUserByUsernameAndPasswordAsync
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
    
    // GetUserByEmailAndPasswordAsync
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
    
    // EditUserAsync(userId, user)
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