using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;
using MembershipManagementMicroservice.Services.Interfaces;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Services;

/// <summary>
///   This class implements the IMembershipService interface.
///  It is used to interact with the Membership table of the database.
///  It contains methods for creating a membership, getting a membership by id, getting all memberships with a given user id, and editing a membership.
/// </summary>
public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IKafkaConsumerService _kafkaConsumerService;
    private readonly ILogger<MembershipService> _logger;
    
    private readonly IRegistrationService _registrationService;
    private readonly IMembershipTypesService _membershipTypesService;
    private readonly IDiscountService _discountService;

    /// <summary>
    ///  The constructor of the MembershipService class.
    /// </summary>
    /// <param name="membershipRepository">
    /// A IMembershipRepository interface that is used to interact with the Membership table of the database.
    /// </param>
    /// <param name="kafkaConsumerService">
    /// A IKafkaConsumerService interface that is used to consume messages from a Kafka topic.
    /// </param>
    /// <param name="logger">
    /// A ILogger interface that is used to log information to the console.
    /// </param>
    /// <param name="registrationService">
    /// A IRegistrationService interface that is used to interact with the User table of the database.
    /// </param>
    public MembershipService(IMembershipRepository membershipRepository, IKafkaConsumerService kafkaConsumerService, ILogger<MembershipService> logger, IRegistrationService registrationService, IMembershipTypesService membershipTypesService, IDiscountService discountService)
    {
        _membershipRepository = membershipRepository;
        _kafkaConsumerService = kafkaConsumerService;
        _logger = logger;
        _registrationService = registrationService;
        _membershipTypesService = membershipTypesService;
        _discountService = discountService;
    }

    /// <summary>
    ///  This method is used to create a membership.
    /// </summary>
    /// <returns>
    /// A string that says whether the membership was created or not.
    /// </returns>
    public async Task<string> CreateMembershipAsync()
    {
        _logger.LogInformation("Creating membership unpacked...");
        // consume from kafka topic and create membership
        var membershipJson = await _kafkaConsumerService.ConsumeMessagesAsync("membership_post_topic");
        _logger.LogInformation($"Membership JSON: {membershipJson}");

        // unpack the membership object from the JSON string
        var membership = JsonConvert.DeserializeObject<Membership>(membershipJson);
        
        // ------
        //check if such membership exists
        var membershipByUserId = await _membershipRepository.GetMembershipsByUserIdAsync(membership.UserId);
        
        // check if in membershipByUserId exist membership with all the same properties
        if (membershipByUserId.Any(membershipByUserIdItem => membershipByUserIdItem.MembershipTypeId == membership.MembershipTypeId
                                                             && membershipByUserIdItem.IsActive == membership.IsActive 
                                                             && membershipByUserIdItem.UserId == membership.UserId
                                                             && membershipByUserIdItem.EndDate == membership.EndDate
                                                             && membershipByUserIdItem.StartDate == membership.StartDate))
        {
            _logger.LogInformation($"Membership with the same properties already exist: {membership.MembershipTypeId}, {membership.IsActive}");
            return "Membership with the same properties already exist";
        }
        
        // ----
        // check if membership type exists
        var membershipType = await _membershipTypesService.GetByIdMembershipTypeAsync(membership.MembershipTypeId.ToString());
        
        // check if membership type exist
        if (membershipType == null)
        {
            _logger.LogInformation($"Membership type with id: {membership.MembershipTypeId} does not exist");
            _logger.LogInformation("Creating membership failed");
            return "Creating membership failed - membership type does not exist.";
        }

        // generate id for membership
        membership.Id = Guid.NewGuid();
        
        //check if membership id exists
        var membershipById = await _membershipRepository.GetMembershipAsync(membership.Id.ToString());
        if (membershipById != null)
        {
            _logger.LogInformation($"Membership with id: {membership.Id} already exist");
            return "Membership with id already exist";
        }
        
        _logger.LogInformation($"Membership: {membership}");

        // ----
        // get user by id
        var user = await _registrationService.GetUserAsync(membership.UserId);
        
        // check if user exist
        if (user == null)
        {
            _logger.LogInformation($"User with id: {membership.UserId} does not exist");
            _logger.LogInformation("Creating membership failed");
            return "Creating membership failed - user does not exist.";
        }
        
        // ----
        // check if user has discount
        var discount = await _discountService.GetDiscountByUserIdAsync(user.Id.ToString());
        decimal user_discount = 0;
        
        // check if discount exist
        if (discount != null)
        {
            // check if discount is active and not expired
            if (discount.IsActive && discount.EndDate > DateTime.Now)
            {
                user_discount = discount.Percentage;
            }
        }
        
        // ----
        var real_price = membershipType.Price - (membershipType.Price * user_discount / 100);
        membership.Price = real_price;
        
        // Example implementation:
        try
        {
            await _membershipRepository.CreateMembershipAsync(membership);
            _logger.LogInformation($"Membership created: {membership}");
            //await _kafkaProducerService.SendMessageAsync("membership_post_topic", JsonConvert.SerializeObject(membership));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to create membership: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
        return "Membership created";
    }

    /// <summary>
    ///  This method is used to get a membership by id.
    /// </summary>
    /// <param name="membershipId">
    ///  A string that represents the id of the membership.
    /// </param>
    /// <returns>
    /// A Membership object that was asked.
    /// </returns>
    public async Task<Membership> GetMembershipAsync(string membershipId)
    {
        try
        {
            var membership = await _membershipRepository.GetMembershipAsync(membershipId);
            
            if (membership == null)
            {
                _logger.LogInformation($"Membership with id: {membershipId} does not exist");
                return null;
            }
            
            var membershipType = await _membershipTypesService.GetByIdMembershipTypeAsync(membership.MembershipTypeId.ToString());
            var user_discount = await _discountService.GetDiscountByUserIdAsync(membership.UserId);
            
            if (user_discount == null)
            {
                user_discount = new Discount();
                user_discount.Percentage = 0;
            }
            
            decimal real_price = membershipType.Price - (membershipType.Price * user_discount.Percentage / 100);
            
            membership.Price = real_price;
            
            _logger.LogInformation($"Membership retrieved: {membership.MembershipTypeId}, {membership.IsActive}, {membership.UserId}");
            return membership;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve membership: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
    }
    
    /// <summary>
    ///  This method is used to get all memberships with a given user id.
    /// </summary>
    /// <param name="userId">
    /// A string that represents the id of the user.
    /// </param>
    /// <returns>
    /// A list of Membership objects that were asked.
    /// </returns>
    public async Task<List<Membership>> GetMembershipsByUserIdAsync(string userId)
    {
        try
        {
            var memberships = await _membershipRepository.GetMembershipsByUserIdAsync(userId);

            foreach (var membership in memberships)
            {
                var membershipType = await _membershipTypesService.GetByIdMembershipTypeAsync(membership.MembershipTypeId.ToString());
                var user_discount = await _discountService.GetDiscountByUserIdAsync(membership.UserId);
                
                if (user_discount == null)
                {
                    user_discount = new Discount();
                    user_discount.Percentage = 0;
                }

                decimal real_price = membershipType.Price - (membershipType.Price * user_discount.Percentage / 100);
            
                membership.Price = real_price;
            }
            
            _logger.LogInformation($"Memberships retrieved: {memberships}");
            return memberships;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve memberships: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
    }
    
    
    /// <summary>
    ///  This method is used to edit a membership.
    /// </summary>
    /// <param name="membershipId">
    /// A string that represents the id of the membership.
    /// </param>
    /// <param name="membership">
    /// A Membership object that represents the membership that will be edited.
    /// </param>
    /// <returns>
    /// A Membership object that was edited.
    /// </returns>
    public async Task<Membership> EditMembershipAsync(string membershipId, Membership membership)
    {
        
        try
        {
            var membershipToEdit = await _membershipRepository.GetMembershipAsync(membershipId);
            if (membershipToEdit == null)
            {
                _logger.LogInformation($"Membership with id: {membershipId} does not exist");
                return null;
            }
            membershipToEdit.MembershipTypeId = membership.MembershipTypeId;
            membershipToEdit.IsActive = membership.IsActive;
            membershipToEdit.UserId = membership.UserId;
            
            var membershipType = await _membershipTypesService.GetByIdMembershipTypeAsync(membershipToEdit.MembershipTypeId.ToString());
            var user_discount = await _discountService.GetDiscountByUserIdAsync(membershipToEdit.UserId);
            
            if (user_discount == null)
            {
                user_discount = new Discount();
                user_discount.Percentage = 0;
            }

            
            decimal real_price = membershipType.Price - (membershipType.Price * user_discount.Percentage / 100);
            
            membershipToEdit.Price = real_price;
            
            await _membershipRepository.EditMembershipAsync(membershipToEdit);
            _logger.LogInformation($"Membership edited: {membershipToEdit}");
            return membershipToEdit;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to edit membership: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
    }
}
