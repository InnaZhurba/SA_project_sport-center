using Microsoft.AspNetCore.Mvc;
using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Services.Interfaces;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Controllers;

/// <summary>
///   The RegistrationController class
///  Contains endpoints for the MembershipManagementMicroservice API
///  </summary>
///  <remarks>
///  <para>This class can create, retrieve, update user data.</para>
///  </remarks>
///  <response code="200">Returns the newly created user</response>
///  <response code="400">If the item is null</response>
///  <response code="500">If there was an internal server error</response>
[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    /// <summary>
    ///  The registration service
    /// </summary>
    ///  <remarks>
    ///  <para>This service contains all methods for user registration.</para>
    ///  </remarks>
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<RegistrationController> _logger;
    private readonly IKafkaProducerService _kafkaProducerService;

    /// <summary>
    ///  The constructor for the RegistrationController class
    /// </summary>
    /// <param name="registrationService">
    /// The registrationService argument represents the IRegistrationService interface
    /// </param>
    /// <param name="kafkaProducerService">
    /// The kafkaProducerService argument represents the IKafkaProducerService interface
    /// </param>
    /// <param name="logger">
    /// The logger argument represents the ILogger interface
    /// </param>
    ///  <remarks>
    ///  <para>This constructor takes in a registration service, a Kafka producer service, and a logger.</para>
    ///  </remarks>
    public RegistrationController(IRegistrationService registrationService, IKafkaProducerService kafkaProducerService, ILogger<RegistrationController> logger)
    {
        _registrationService = registrationService;
        _kafkaProducerService = kafkaProducerService;
        _logger = logger;
    }

    /// <summary>
    ///  The RegisterUser method for the RegistrationController class that creates a new user
    /// </summary>
    /// <param name="user">
    /// The user argument represents a User object
    /// </param>
    /// <returns>
    ///  A newly created user
    /// </returns>
    ///  <remarks>
    ///  <para>This method takes in a user object and returns a newly created user.</para>
    ///  Sample request:
    ///   POST /api/registration
    ///  {
    ///  "Id": "string",
    ///  "userName": "string",
    ///  "email": "string",
    ///  "password": "string"
    ///  }
    ///  </remarks>
    ///  <response code="200">Returns the newly created user</response>
    ///  <response code="400">If the item is null</response>
    ///  <response code="500">If there was an internal server error</response>
    [HttpPost]
    public async Task<IActionResult> RegisterUser(User user)
    {
        try
        {
            // Convert the membership object to JSON/string format
            string userJson = JsonConvert.SerializeObject(user);
            _logger.LogInformation($"User JSON: {userJson}");

            // Send the command and user data to Kafka
            var result1 = Task.Run(() => _kafkaProducerService.SendMessageAsync("user_post_topic", userJson));
            _logger.LogInformation("User creation request sent to Kafka.");
            //await _registrationService.RegisterUserAsync();
            var result2 = Task.Run(() => _registrationService.RegisterUserAsync());
            
            Task.WhenAll(result1, result2).GetAwaiter().GetResult();

            // Return a successful response
            return Ok(result2.Result); //Ok("User creation request sent to Kafka.");
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the process
            // Log the error or return an appropriate error response
            _logger.LogError($"An error occurred while sending the user creation request: {ex.Message}");
            return StatusCode(500, "An error occurred while sending the user creation request.");
        }
    }

    /// <summary>
    ///  The GetUser method for the RegistrationController class that retrieves a user by ID
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents a user ID in string format
    /// </param>
    /// <returns>
    /// A user with the specified ID or a 404 Not Found response
    /// </returns>
    ///  <remarks>
    ///  <para>This method takes in a user ID and returns a user with the specified ID.</para>
    ///  Sample request:
    ///  GET /api/registration/1
    ///  </remarks>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        try
        {
            _logger.LogInformation($"User ID: {userId} requested for retrieval.");
            // Invoke the appropriate method from the membership service
            var user = await _registrationService.GetUserAsync(userId);
            _logger.LogInformation($"User: {user}");
            
            return (user != null) ? Ok(user) : NotFound($"User with ID {userId} not found.");
            
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the process
            // Log the error or return an appropriate error response
            return StatusCode(500, "An error occurred while retrieving the membership data.");
        }
    }
    
    /// <summary>
    ///  The GetUserByEmail method for the RegistrationController class that retrieves a user by email
    /// </summary>
    /// <param name="email">
    /// The email argument represents a user email in string format
    /// </param>
    /// <returns>
    /// A user with the specified email or a 404 Not Found response 
    /// </returns>
    ///  <remarks>
    ///  <para>This method takes in an email and returns a user with the specified email.</para>
    ///  Sample request:
    ///  GET /api/registration/email/{email}
    ///  </remarks>
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        try
        {
            _logger.LogInformation($"User email: {email} requested for retrieval.");
            // Invoke the appropriate method from the membership service
            var user = await _registrationService.GetUserByEmailAsync(email);
            _logger.LogInformation($"User: {user}");
            
            return (user != null) ? Ok(user) : NotFound($"User with email {email} not found.");
            
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the process
            // Log the error or return an appropriate error response
            return StatusCode(500, "An error occurred while retrieving the membership data.");
        }
    }
    
    /// <summary>
    ///  The GetUserByUsernameAndPassword method for the RegistrationController class that retrieves a user by username and password
    /// </summary>
    /// <param name="username">
    /// The username argument represents a user username in string format
    /// </param>
    /// <param name="password">
    /// The password argument represents a user password in string format
    /// </param>
    /// <returns>
    /// A user with the specified username and password or a 404 Not Found response
    /// </returns>
    ///  <remarks>
    ///  <para>This method takes in a username and password and returns a user with the specified username and password.</para>
    ///  Sample request:
    ///   GET /api/registration/username/{username}/password/{password}
    /// </remarks>
    [HttpGet("username/{username}/password/{password}")]
    public async Task<IActionResult> GetUserByUsernameAndPassword(string username, string password)
    {
        try
        {
            _logger.LogInformation($"User username: {username} and password: {password} requested for retrieval.");
            // Invoke the appropriate method from the membership service
            var user = await _registrationService.GetUserByUsernameAndPasswordAsync(username, password);
            _logger.LogInformation($"User: {user}");
            
            return (user != null) ? Ok(user) : NotFound($"User with username {username} and password {password} not found.");
            
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the process
            // Log the error or return an appropriate error response
            return StatusCode(500, "An error occurred while retrieving the membership data.");
        }
    }
    
    /// <summary>
    ///  The GetUserByEmailAndPassword method for the RegistrationController class that retrieves a user by email and password
    /// </summary>
    /// <param name="email">
    /// The email argument represents a user email in string format
    /// </param>
    /// <param name="password">
    /// The password argument represents a user password in string format
    /// </param>
    /// <returns>
    /// A user with the specified email and password or a 404 Not Found response
    /// </returns>
    ///  <remarks>
    ///  <para>This method takes in a user object and returns a newly created user.</para>
    ///  Sample request:
    ///   GET /api/registration/email/{email}/password/{password}
    ///  </remarks>
    [HttpGet("email/{email}/password/{password}")]
    public async Task<IActionResult> GetUserByEmailAndPassword(string email, string password)
    {
        try
        {
            _logger.LogInformation($"User email: {email} and password: {password} requested for retrieval.");
            // Invoke the appropriate method from the membership service
            var user = await _registrationService.GetUserByEmailAndPasswordAsync(email, password);
            _logger.LogInformation($"User: {user}");
            
            return (user != null) ? Ok(user) : NotFound($"User with email {email} and password {password} not found.");
            
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the process
            // Log the error or return an appropriate error response
            return StatusCode(500, "An error occurred while retrieving the membership data.");
        }
    }
    
    /// <summary>
    ///  The EditUser method for the RegistrationController class that edits a user by ID
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents a user ID in string format
    /// </param>
    /// <param name="user">
    /// The user argument represents a user object
    /// </param>
    /// <returns>
    /// A user with the specified ID that is edited or a 404 Not Found response
    /// </returns>
    ///  <remarks>
    ///  Sample request:
    ///   PUT /api/registration/edit/{userId}
    ///  {
    ///   "userId": "1",
    ///  "username": "user1",
    ///  "password": "password1",
    ///  "email": "user1@gmail"
    ///  }
    ///  </remarks>
    [HttpPut("edit/{userId}")]
    public async Task<IActionResult> EditUser(string userId, User user)
    {
        try
        {
            _logger.LogInformation($"User ID: {userId} requested for edit.");
            // Invoke the appropriate method from the membership service
            var result = await _registrationService.EditUserAsync(userId, user);
            _logger.LogInformation($"User: {result}");
            
            return (result != null) ? Ok(result) : NotFound($"User with ID {userId} not found.");
            
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the process
            // Log the error or return an appropriate error response
            return StatusCode(500, "An error occurred while retrieving the membership data.");
        }
    }
}