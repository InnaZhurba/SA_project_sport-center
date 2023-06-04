using Microsoft.AspNetCore.Mvc;
using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Services.Interfaces;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Controllers;


//use D doccumentation

/// <summary>
///   The MembershipController class
///  Contains all endpoints for the MembershipManagementMicroservice API
///  </summary>
///  <remarks>
///  <para>This class can create, retrieve, update user data.</para>
///  </remarks>
///  <response code="200">Returns the newly created user</response>
///  <response code="400">If the item is null</response>
///  <response code="500">If there was an internal server error</response>
///  <returns></returns>
[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<RegistrationController> _logger;
    private readonly IKafkaProducerService _kafkaProducerService;

    public RegistrationController(IRegistrationService registrationService, IKafkaProducerService kafkaProducerService, ILogger<RegistrationController> logger)
    {
        _registrationService = registrationService;
        _kafkaProducerService = kafkaProducerService;
        _logger = logger;
    }

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
    
    // get user by email
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
    
    // get user by username and password
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
    
    // get user by email and password
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
    
    // edit user by id
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