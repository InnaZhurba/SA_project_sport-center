using Microsoft.AspNetCore.Mvc;
using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Services.Interfaces;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Controllers;

/// <summary>
///  The MembershipController class is a RESTful API controller class
///  It handles all HTTP requests to the /api/membership endpoint
///  It contains methods to create, retrieve, update, and membership data
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MembershipController : ControllerBase
{
    private readonly IMembershipService _membershipService;
    private readonly IKafkaProducerService _kafkaProducerService;
    private readonly ILogger<MembershipController> _logger;

    /// <summary>
    ///  The constructor accepts two services and a logger as arguments and assigns them to private fields 
    /// </summary>
    /// <param name="membershipService">
    ///  The membershipService argument represents the IMembershipService interface
    /// </param>
    /// <param name="kafkaProducerService">
    /// The kafkaProducerService argument represents the IKafkaProducerService interface
    /// </param>
    /// <param name="logger">
    /// The logger argument represents the ILogger interface
    /// </param>
    public MembershipController(IMembershipService membershipService, IKafkaProducerService kafkaProducerService, ILogger<MembershipController> logger)
    {
        _membershipService = membershipService;
        _kafkaProducerService = kafkaProducerService;
        _logger = logger;
    }

    /// <summary>
    ///  The CreateMembership method accepts a Membership object as an argument and returns an IActionResult response 
    /// </summary>
    /// <param name="membership">
    /// The membership argument represents a Membership object
    /// </param>
    /// <returns>
    /// An IActionResult response
    /// </returns>
    ///  <remarks>
    ///  Sample request:
    ///   POST /api/membership
    ///  {
    ///  "membershipType": "Gold",
    ///  "isActive": "Active",
    ///   "startDate": "2021-01-01",
    ///  "endDate": "2022-01-01",
    ///  "userId": "1234567890"
    ///  }
    ///  </remarks>
    [HttpPost]
    public async Task<IActionResult> CreateMembership(Membership membership)
    {
        try
        {
            // Convert the membership object to JSON/string format
            string membershipJson = JsonConvert.SerializeObject(membership);
            _logger.LogInformation($"Membership JSON: {membershipJson}");

            // Send the command and membership data to Kafka
            var result1 = Task.Run(() => _kafkaProducerService.SendMessageAsync("membership_post_topic", membershipJson));
            _logger.LogInformation("Membership creation request sent to Kafka.");
            var result2 = Task.Run(() => _membershipService.CreateMembershipAsync());
            
            Task.WhenAll(result1, result2).GetAwaiter().GetResult();

            // Return a successful response
            return Ok(result2.Result); //Ok("Membership creation request sent to Kafka.");
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the process
            // Log the error or return an appropriate error response
            _logger.LogError($"An error occurred while sending the membership creation request: {ex.Message}");
            return StatusCode(500, "An error occurred while sending the membership creation request.");
        }
    }
 
    /// <summary>
    ///  The GetMembership method accepts a membership ID as an argument and returns an IActionResult response
    ///  It invokes the GetMembershipAsync method from the membership service
    /// </summary>
    /// <param name="membershipId">
    /// The membershipId argument represents a membership ID as a string
    /// </param>
    /// <returns>
    ///  An IActionResult response
    /// </returns>
    /// <remarks>
    ///  Sample request:
    ///  GET /api/membership/1234567890
    ///  </remarks>
    ///  <response code="200">Membership data retrieved successfully</response>
    ///  <response code="404">Membership not found</response>
    ///  <response code="500">An error occurred while retrieving the membership data</response>
    [HttpGet("{membershipId}")]
    public async Task<IActionResult> GetMembership(string membershipId)
    {
        try
        {
            _logger.LogInformation($"Membership ID: {membershipId} requested for retrieval.");
            // Invoke the appropriate method from the membership service
            var membership = await _membershipService.GetMembershipAsync(membershipId);
            _logger.LogInformation($"Membership: {membership}");
            
            return (membership != null) ? Ok(membership) : NotFound($"Membership with ID {membershipId} not found.");
            
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the process
            // Log the error or return an appropriate error response
            return StatusCode(500, "An error occurred while retrieving the membership data.");
        }
    }
    
    /// <summary>
    ///  The GetMembershipsByUserId method accepts a user ID as an argument and returns an IActionResult response
    ///  It invokes the GetMembershipsByUserIdAsync method from the membership service
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents a user ID as a string
    /// </param>
    /// <returns>
    /// An IActionResult response
    /// </returns>
    ///  <remarks>
    ///  Sample request:
    ///  GET /api/membership/user/1234567890
    ///  </remarks>
    ///  <response code="200">Memberships data retrieved successfully</response>
    ///  <response code="404">Memberships not found</response>
    ///  <response code="500">An error occurred while retrieving the memberships data</response>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetMembershipsByUserId(string userId)
    {
        try
        {
            _logger.LogInformation($"User ID: {userId} requested for retrieval.");
            // Invoke the appropriate method from the membership service
            var memberships = await _membershipService.GetMembershipsByUserIdAsync(userId);
            _logger.LogInformation($"Memberships: {memberships}");
            
            return (memberships != null) ? Ok(memberships) : NotFound($"Memberships with user ID {userId} not found.");
            
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the process
            // Log the error or return an appropriate error response
            return StatusCode(500, "An error occurred while retrieving the memberships data.");
        }
    }
    
    /// <summary>
    ///  The EditMembership method accepts a membership ID and a Membership object as arguments and returns an IActionResult response
    ///  It invokes the EditMembershipAsync method from the membership service
    /// </summary>
    /// <param name="membershipId">
    /// The membershipId argument represents a membership ID as a string
    /// </param>
    /// <param name="membership">
    ///  The membership argument represents a Membership object
    /// </param>
    /// <returns>
    /// An IActionResult response
    /// </returns>
    /// <remarks>
    ///  Sample request:
    ///  PUT /api/membership/edit/1234567890
    ///  {
    ///  "membershipType": "Gold",
    ///  "isActive": "Active",
    ///  "startDate": "2021-01-01",
    ///  "endDate": "2022-01-01",
    ///  "userId": "1234567890"
    ///  }
    ///  </remarks>
    [HttpPut("edit/{membershipId}")]
    public async Task<IActionResult> EditMembership(string membershipId, Membership membership)
    {
        try
        {
            _logger.LogInformation($"Membership ID: {membershipId} requested for update.");
            // Invoke the appropriate method from the membership service
            var result = await _membershipService.EditMembershipAsync(membershipId, membership);
            _logger.LogInformation($"Membership: {result}");
            
            return (result != null) ? Ok(result) : NotFound($"Membership with ID {membershipId} not found.");
            
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the process
            // Log the error or return an appropriate error response
            return StatusCode(500, "An error occurred while updating the membership data.");
        }
    }
}