using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Controllers;

/// <summary>
///  The MembershipTypesController class is a RESTful API controller class
///  It handles all HTTP requests to the /api/membershipTypes endpoint
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MembershipTypesController  : ControllerBase
{
    
    private readonly IMembershipTypesService _membershipTypesService;
    private readonly IKafkaProducerService _kafkaProducerService;
    private readonly ILogger<MembershipTypesController> _logger;
    
    /// <summary>
    ///  The constructor accepts two services and a logger as arguments and assigns them to private fields
    /// </summary>
    /// <param name="membershipTypesService">
    /// The membershipService argument represents the IMembershipTypesService interface
    /// </param>
    /// <param name="kafkaProducerService">
    /// The kafkaProducerService argument represents the IKafkaProducerService interface
    /// </param>
    /// <param name="logger">
    /// The logger argument represents the ILogger interface
    /// </param>
    public MembershipTypesController(IMembershipTypesService membershipTypesService, IKafkaProducerService kafkaProducerService, ILogger<MembershipTypesController> logger)
    {
        _membershipTypesService = membershipTypesService;
        _kafkaProducerService = kafkaProducerService;
        _logger = logger;
    }
    
    /// <summary>
    ///  The CreateMembershipTypeAsync method accepts a MembershipType object as an argument and returns an IActionResult response
    /// </summary>
    /// <param name="membershipType">
    /// The membershipType argument represents a MembershipType object
    /// </param>
    /// <returns>
    /// An IActionResult response
    /// </returns>
    ///  <example>
    /// POST /api/membershiptypes
    ///   {
    ///     "name" : "Premium",
    ///     "description" : "premium plan description",
    ///     "price" : 799.99
    /// }
    /// </example>
    [HttpPost]
    public async Task<IActionResult> CreateMembershipTypeAsync(MembershipType membershipType)
    {
        try
        {
            var membershiptypeJson = JsonConvert.SerializeObject(membershipType);
            
            var result1 = Task.Run(() => _kafkaProducerService.SendMessageAsync("membership_type_post_topic", membershiptypeJson));
            _logger.LogInformation("MembershipType sended to Kafka topic.");
            
            var result2 = Task.Run(() => _membershipTypesService.CreateMembershipTypeAsync());
            _logger.LogInformation("MembershipType sended to Cassandra database.");
            
            Task.WhenAll( result1, result2).GetAwaiter().GetResult();
            //var membershipTypeCreated = await _membershipTypesService.CreateMembershipTypeAsync(membershipType);
            _logger.LogInformation("MembershipType created successfully"); 
            
            return Ok(JsonConvert.SerializeObject(result2.Result));
            
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            
            //return StatusCode(500, "An error occurred while sending the membership creation request."); //new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return StatusCode(500, JsonConvert.SerializeObject(e.Message));
        }
    }
    
    /// <summary>
    ///  The GetByIdMembershipTypeAsync method accepts a string as an argument and returns an IActionResult response
    /// </summary>
    /// <param name="membershipTypeId">
    /// The membershipTypeId argument represents a string
    /// </param>
    /// <returns>
    /// An IActionResult response that is represented by a string
    /// </returns>
    ///  <example>
    ///  GET /api/membershiptypes/id/1
    ///  </example>
    [HttpGet("id/{membershipTypeId}")]
    public async Task<IActionResult> GetByIdMembershipTypeAsync(string membershipTypeId)
    {
        try
        {
            var membershipType = await _membershipTypesService.GetByIdMembershipTypeAsync(membershipTypeId);
            if (membershipType == null)
            {
                return StatusCode(404, "MembershipType by id not found.");
            }
            //return new OkObjectResult(membershipType);
            return Ok(JsonConvert.SerializeObject(membershipType));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            //return StatusCode(500, "An error occurred while sending the membership get by id request.");
            return StatusCode(500, JsonConvert.SerializeObject(e.Message));
        }
    }
    
    /// <summary>
    ///  The GetByNameMembershipTypeAsync method accepts a string as an argument and returns an IActionResult response
    /// </summary>
    /// <param name="membershipTypeName">
    /// The membershipTypeName argument represents a string
    /// </param>
    /// <returns>
    /// An IActionResult response that is represented by a string
    /// </returns>
    ///  <example>
    ///  GET /api/membershiptypes/name/Premium
    ///  </example>
    [HttpGet("name/{membershipTypeName}")]
    public async Task<IActionResult> GetByNameMembershipTypeAsync(string membershipTypeName)
    {
        try
        {
            var membershipType = await _membershipTypesService.GetByNameMembershipTypeAsync(membershipTypeName);
            if (membershipType == null)
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            //return new OkObjectResult(membershipType);
            return Ok(JsonConvert.SerializeObject(membershipType));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            //return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return StatusCode(500, JsonConvert.SerializeObject(e.Message));
        }
    }
    
    /// <summary>
    ///  The GetAllMembershipTypesAsync method that accepts no arguments and returns all MembershipType objects 
    /// </summary>
    /// <returns>
    ///     An IActionResult response that is represented by a list of MembershipType objects
    /// </returns>
    ///  <example>
    ///  GET /api/membershiptypes
    ///  </example>
    [HttpGet]
    public async Task<IActionResult> GetAllMembershipTypesAsync()
    {
        try
        {
            var membershipTypes = await _membershipTypesService.GetAllMembershipTypesAsync();
            if (membershipTypes == null)
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            //return new OkObjectResult(membershipTypes);
            return Ok(JsonConvert.SerializeObject(membershipTypes));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            //return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return StatusCode(500, JsonConvert.SerializeObject(e.Message));
        }
    }
    
    /// <summary>
    ///  The UpdateMembershipTypeAsync method accepts a MembershipType object as an argument and returns an IActionResult response
    /// </summary>
    /// <param name="membershipType">
    /// The membershipType argument represents a MembershipType object
    /// </param>
    /// <returns>
    /// An IActionResult response 
    /// </returns>
    ///  <example>
    ///  PUT /api/membershiptypes
    ///  {
    ///   "id" : "1",
    ///  "name" : "Premium",
    ///  "description" : "premium plan description",
    ///  "price" : 799.99
    ///  }
    ///  </example>
    [HttpPut]
    public async Task<IActionResult> UpdateMembershipTypeAsync(MembershipType membershipType)
    {
        try
        {
            var membershipTypeUpdated = await _membershipTypesService.UpdateMembershipTypeAsync(membershipType);
            if (membershipTypeUpdated == null)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            //return new OkObjectResult(membershipTypeUpdated);
            return Ok(JsonConvert.SerializeObject(membershipTypeUpdated));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
           // return new StatusCodeResult(StatusCodes.Status500InternalServerError);
              return StatusCode(500, JsonConvert.SerializeObject(e.Message));
        }
    }
    
    /// <summary>
    ///  The DeleteMembershipTypeAsync method accepts a string as an argument and returns an IActionResult response 
    /// </summary>
    /// <param name="membershipTypeId">
    /// The membershipTypeId argument represents a string
    /// </param>
    /// <returns>
    /// An IActionResult response
    /// </returns>
    ///  <example>
    ///  DELETE /api/membershiptypes/1
    ///  </example>
    [HttpDelete("{membershipTypeId}")]
    public async Task<IActionResult> DeleteMembershipTypeAsync(string membershipTypeId)
    {
        try
        {
            var membershipTypeDeleted = await _membershipTypesService.DeleteMembershipTypeAsync(membershipTypeId);
            if (membershipTypeDeleted == null)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            //return new OkObjectResult(membershipTypeDeleted);
            return Ok(JsonConvert.SerializeObject(membershipTypeDeleted));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            //return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return StatusCode(500, JsonConvert.SerializeObject(e.Message));
        }
    }
    
    
}