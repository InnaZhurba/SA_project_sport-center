using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;
using MembershipManagementMicroservice.Services.Interfaces;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Services;

/// <summary>
///     This class implements the IDiscountService interface.
///   It is used to interact with the Discount table of the database.
///  It contains methods for creating a discount, getting a discount by id, getting a discount by user id, editing a discount, and deleting a discount.
/// </summary>
public class DiscountService : IDiscountService
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IKafkaConsumerService _kafkaConsumerService;
    private readonly ILogger<MembershipService> _logger;
    
    private readonly IRegistrationService _registrationService;

    /// <summary>
    ///  The constructor of the DiscountService class.
    /// </summary>
    /// <param name="discountRepository">
    /// A IDiscountRepository interface that is used to interact with the Discount table of the database.
    /// </param>
    /// <param name="kafkaConsumerService">
    /// A IKafkaConsumerService interface that is used to consume messages from a Kafka topic.
    /// </param>
    /// <param name="logger">
    /// A ILogger interface that is used to log information to the console.
    /// </param>
    public DiscountService(IDiscountRepository discountRepository, IKafkaConsumerService kafkaConsumerService, ILogger<MembershipService> logger, IRegistrationService registrationService)
    {
        _discountRepository = discountRepository;
        _kafkaConsumerService = kafkaConsumerService;
        _logger = logger;
        _registrationService = registrationService;
    }
    
    /// <summary>
    ///  This method is used to create a discount.
    ///  It consumes a message from a Kafka topic and creates a discount.
    ///  Check if the user exists. 
    /// </summary>
    /// <returns>
    /// A string that says whether the discount was created or not.
    /// </returns>
    public async Task<string> CreateDiscountAsync()
    {
        try { 
            
            
            _logger.LogInformation("Creating discount unpacked...");
            // consume from kafka topic and create discount
            var discountJson = await _kafkaConsumerService.ConsumeMessagesAsync("discount_post_topic");
            _logger.LogInformation($"Discount JSON: {discountJson}");

            // unpack the discount object from the JSON string
            var discount = JsonConvert.DeserializeObject<Discount>(discountJson);
            _logger.LogInformation($"Discount object: {discount}");
            
            // check if user exists
            var user = await _registrationService.GetUserAsync(discount.UserId.ToString());
            if (user == null) {
                _logger.LogInformation($"User with id {discount.UserId} does not exist!");
                return $"User with id {discount.UserId} does not exist!";
            }
            
            //create id for discount
            discount.Id = Guid.NewGuid();

            // create discount
            await _discountRepository.CreateDiscountAsync(discount);
            _logger.LogInformation("Discount created successfully!");
            return "Discount created successfully!";
        } catch (Exception e) {
            _logger.LogInformation($"Error creating discount: {e.Message}");
            return $"Error creating discount: {e.Message}";
        }
    }
    
    /// <summary>
    ///  This method is used to get a discount by id.
    /// </summary>
    /// <param name="discountId">
    /// The discountId argument represents the id of the discount to be retrieved from the database
    /// </param>
    /// <returns>
    /// A string that contains the discount object in JSON format.
    /// </returns>
    public async Task<string> GetDiscountByIdAsync(string discountId)
    {
       try {
            _logger.LogInformation("Getting discount...");
            var discount = await _discountRepository.GetDiscountByIdAsync(discountId);
            _logger.LogInformation("Discount retrieved successfully!");
            return JsonConvert.SerializeObject(discount);
        } catch (Exception e) {
            _logger.LogInformation($"Error getting discount: {e.Message}");
            return $"Error getting discount: {e.Message}";
        }
    }
    
    /// <summary>
    ///  This method is used to get a discount by user id.
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents the id of the user whose discount is to be retrieved from the database
    /// </param>
    /// <returns>
    /// A string that contains the discount object in JSON format.
    /// </returns>
    public async Task<string> GetDiscountByUserIdAsync(string userId)
    {
       try {
            _logger.LogInformation("Getting discount...");
            var discount = await _discountRepository.GetDiscountByUserIdAsync(userId);
            _logger.LogInformation("Discount retrieved successfully!");
            return JsonConvert.SerializeObject(discount);
        } catch (Exception e) {
            _logger.LogInformation($"Error getting discount: {e.Message}");
            return $"Error getting discount: {e.Message}";
        }
    }
    
    /// <summary>
    ///  This method is used to update a discount.
    /// </summary>
    /// <param name="discount">
    /// The discount argument represents the discount object to be updated in the database
    /// </param>
    /// <returns>
    /// A string that says whether the discount was updated or not.
    /// </returns>
    public async Task<string> UpdateDiscountAsync( Discount discount)
    {
        try {
            _logger.LogInformation("Updating discount...");
            await _discountRepository.UpdateDiscountAsync(discount);
            _logger.LogInformation("Discount updated successfully!");
            return "Discount updated successfully!";
        } catch (Exception e) {
            _logger.LogInformation($"Error updating discount: {e.Message}");
            return $"Error updating discount: {e.Message}";
        }
    }
    
    /// <summary>
    ///  This method is used to delete a discount by id.
    /// </summary>
    /// <param name="discountId">
    /// The discountId argument represents the id of the discount to be deleted from the database
    /// </param>
    /// <returns>
    /// A string that says whether the discount was deleted or not.
    /// </returns>
    public async Task<string> DeleteDiscountByIdAsync(string discountId)
    {
       try {
            _logger.LogInformation("Deleting discount...");
            await _discountRepository.DeleteDiscountByIdAsync(discountId);
            _logger.LogInformation("Discount deleted successfully!");
            return "Discount deleted successfully!";
        } catch (Exception e) {
            _logger.LogInformation($"Error deleting discount: {e.Message}");
            return $"Error deleting discount: {e.Message}";
        }
    }
    
    /// <summary>
    ///  This method is used to get all  discounts by user id.
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents the id of the user whose discounts are to be retrieved from the database
    /// </param>
    /// <returns>
    /// A string that contains the discount objects in JSON format.
    /// </returns>
    public async Task<string> GetAllDiscountsByUserIdAsync(string userId)
    {
        try {
            _logger.LogInformation("Getting all active discounts...");
            var discounts = await _discountRepository.GetAllDiscountsByUserIdAsync(userId);
            _logger.LogInformation("Discounts retrieved successfully!");
            return JsonConvert.SerializeObject(discounts);
        } catch (Exception e) {
            _logger.LogInformation($"Error getting discounts: {e.Message}");
            return $"Error getting discounts: {e.Message}";
        }
    }
}