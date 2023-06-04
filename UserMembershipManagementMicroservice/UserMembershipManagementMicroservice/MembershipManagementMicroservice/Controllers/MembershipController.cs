using Microsoft.AspNetCore.Mvc;
using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Services.Interfaces;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembershipController : ControllerBase
{
    private readonly IMembershipService _membershipService;
    private readonly IKafkaProducerService _kafkaProducerService;
    private readonly ILogger<MembershipController> _logger;

    public MembershipController(IMembershipService membershipService, IKafkaProducerService kafkaProducerService, ILogger<MembershipController> logger)
    {
        _membershipService = membershipService;
        _kafkaProducerService = kafkaProducerService;
        _logger = logger;
    }

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
    
    // get all memberships by user id
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
    
    // edit membership by membership id
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