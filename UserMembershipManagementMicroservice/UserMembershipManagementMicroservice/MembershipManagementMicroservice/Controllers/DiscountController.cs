using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace MembershipManagementMicroservice.Controllers;

/// <summary>
///  The MembershipTypesController class is a RESTful API controller class
///  It handles all HTTP requests to the /api/membershipTypes endpoint
///  It inherits ControllerBase
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DiscountController : ControllerBase
{
    private readonly IDiscountService _discountService;
    private readonly IKafkaProducerService _kafkaProducerService;
    private readonly ILogger<MembershipController> _logger;
    
    /// <summary>
    ///  The constructor accepts two services and a logger as arguments and assigns them to private fields
    /// </summary>
    /// <param name="discountService">
    /// The discountService argument represents the IDiscountService interface
    /// </param>
    /// <param name="kafkaProducerService">
    /// The kafkaProducerService argument represents the IKafkaProducerService interface
    /// </param>
    /// <param name="logger">
    /// The logger argument represents the ILogger interface
    /// </param>
    public DiscountController(IDiscountService discountService, IKafkaProducerService kafkaProducerService, ILogger<MembershipController> logger)
    {
        _discountService = discountService;
        _kafkaProducerService = kafkaProducerService;
        _logger = logger;
    }
    
    /// <summary>
    ///  The CreateDiscountAsync method accepts a Discount object as an argument and returns an IActionResult response
    /// </summary>
    /// <param name="discount">
    /// The discount argument represents a Discount object
    /// </param>
    /// <returns>
    /// An IActionResult response as string with the result of the operation
    /// </returns>
    /// <example>
    ///  POST: api/discount
    /// {
    ///     "UserId": "123e4552-e89b-12d3-a456-426614164400",
    ///     "Percentage": 17.3,
    ///     "StartDate": "2023-06-29T10:00:00Z",
    ///     "EndDate": "2023-08-05T10:00:00Z",
    ///     "IsActive": false
    /// }
    /// </example>
    [HttpPost]
    public async Task<IActionResult> CreateDiscountAsync(Discount discount)
    {
        try
        {
            var result1 = Task.Run( () => _kafkaProducerService.SendMessageAsync( "discount_post_topic", JsonConvert.SerializeObject(discount)));
            _logger.LogInformation("Discount sended to Kafka topic.");
            
            var result2 = Task.Run(() => _discountService.CreateDiscountAsync());
            _logger.LogInformation("Discount created.");
            
            Task.WhenAll(result1, result2).GetAwaiter().GetResult();
            
            return Ok(result2.Result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode( StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    
    /// <summary>
    ///  The GetAllDiscountsAsync gets all Discount objects from the database with some id and returns an IActionResult response
    /// </summary>
    /// <param name="discountId">
    /// The discountId argument represents a Discount object
    /// </param>
    /// <returns>
    /// An IActionResult response as string with the result of the operation (string with result)
    /// </returns>
    ///  <example>
    /// GET: api/discount/id/123e4552-e89b-12d3-a456-426614164400
    ///  </example>
    [HttpGet("id/{discountId}")]
    public async Task<IActionResult> GetDiscountByIdAsync(string discountId)
    {
        try
        {
            var result = Task.Run(() => _discountService.GetDiscountByIdAsync(discountId));
            _logger.LogInformation("Discount retrieved.");
            return (result.Result != null) ? Ok(result.Result) : NotFound("Discount not found.");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    
    /// <summary>
    ///  The GetAllDiscountsAsync gets all Discount objects from the database with some userId and returns an IActionResult response
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents a Discount object
    /// </param>
    /// <returns>
    /// An IActionResult response as string with the result of the operation (string with the result)
    /// </returns>
    /// <example>
    ///  GET: api/discount/user/123e4552-e89b-12d3-a456-426614164400
    /// </example>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetDiscountByUserIdAsync(string userId)
    {
        try
        {
            var result = Task.Run(() => _discountService.GetDiscountByUserIdAsync(userId));
            _logger.LogInformation("Discount retrieved.");
            return (result.Result != null) ? Ok(result.Result) : NotFound("Discount not found.");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode( StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    
    /// <summary>
    /// The UpdateDiscountAsync method updating a Discount object and returns an IActionResult response
    /// </summary>
    /// <param name="discount">
    /// The discount argument represents a Discount object
    /// </param>
    /// <returns>
    /// An IActionResult response as string with the result of the operation (string with the result)
    /// </returns>
    ///
    ///  <example>
    /// PUT: api/discount
    ///  {
    ///  "Id": "123e4552-e89b-1862-a456-426676796900",
    ///   "UserId": "123e4552-e89b-12d3-a456-426614164400",
    ///  "Percentage": 17.3,
    ///  "StartDate": "2023-06-29T10:00:00Z",
    ///  "EndDate": "2023-08-05T10:00:00Z",
    ///  "IsActive": false
    ///  }
    ///  </example>
    [HttpPut]
    public async Task<IActionResult> UpdateDiscountAsync(Discount discount)
    {
       try 
       {
           var result = _discountService.UpdateDiscountAsync(discount);
              _logger.LogInformation("Discount updated.");
              
              return Ok(result.Result);
       }
       catch (Exception e)
       {
           _logger.LogError(e.Message);
           return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
       }
    }
    
    /// <summary>
    ///  The DeleteDiscountByIdAsync method deleting a Discount object and returns an IActionResult response
    /// </summary>
    /// <param name="discountId">
    /// The discountId argument represents a id of Discount object as string
    /// </param>
    /// <returns>
    /// An IActionResult response as string with the result of the operation (string with the result)
    /// </returns>
    ///  <example>
    ///  DELETE: api/discount/123e4552-e89b-12d3-a456-426614164400
    ///  </example>
    [HttpDelete("{discountId}")]
    public async Task<IActionResult> DeleteDiscountByIdAsync(string discountId)
    {
        try
        {
            var result =  _discountService.DeleteDiscountByIdAsync(discountId);
            _logger.LogInformation("Discount deleted.");
            
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode( StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    ///  The GetActiveDiscountsByUserIdAsync gets all Discount objects from the database with some userId and returns an IActionResult response
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents a Discount object
    /// </param>
    /// <returns>
    /// An IActionResult response as string with the result of the operation (string with the result)
    /// </returns>
    ///  <example>
    ///  GET: api/discount/user/all/123e4552-e89b-12d3-a456-426614164400
    ///  </example>
    [HttpGet("user/all/{userId}")]
    public async Task<IActionResult> GetDiscountsByUserIdAsync(string userId)
    {
        try
        {
            var result = Task.Run(() => _discountService.GetAllDiscountsByUserIdAsync( userId ));
            _logger.LogInformation("Discount retrieved.");
            return (result.Result != null) ? Ok(result.Result) : NotFound("Discount not found.");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return  StatusCode( StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}