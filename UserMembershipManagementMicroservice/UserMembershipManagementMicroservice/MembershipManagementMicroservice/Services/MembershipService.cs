using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;
using MembershipManagementMicroservice.Services.Interfaces;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Services;

public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IKafkaConsumerService _kafkaConsumerService;
    private readonly ILogger<MembershipService> _logger;
    
    private readonly IRegistrationService _registrationService;

    public MembershipService(IMembershipRepository membershipRepository, IKafkaConsumerService kafkaConsumerService, ILogger<MembershipService> logger, IRegistrationService registrationService)
    {
        _membershipRepository = membershipRepository;
        _kafkaConsumerService = kafkaConsumerService;
        _logger = logger;
        _registrationService = registrationService;
    }

    public async Task<string> CreateMembershipAsync()
    {
        _logger.LogInformation("Creating membership unpacked...");
        // consume from kafka topic and create membership
        var membershipJson = await _kafkaConsumerService.ConsumeMessagesAsync("membership_post_topic");
        _logger.LogInformation($"Membership JSON: {membershipJson}");

        // unpack the membership object from the JSON string
        var membership = JsonConvert.DeserializeObject<Membership>(membershipJson);

        //check if such membership exists
        var membershipByUserId = await _membershipRepository.GetMembershipsByUserIdAsync(membership.UserId);
        
        // check if in membershipByUserId exist membership with all the same properties
        if (membershipByUserId.Any(membershipByUserIdItem => membershipByUserIdItem.MembershipType == membership.MembershipType 
                                                             && membershipByUserIdItem.IsActive == membership.IsActive 
                                                             && membershipByUserIdItem.UserId == membership.UserId
                                                             && membershipByUserIdItem.EndDate == membership.EndDate
                                                             && membershipByUserIdItem.StartDate == membership.StartDate))
        {
            _logger.LogInformation($"Membership with the same properties already exist: {membership.MembershipType}, {membership.IsActive}");
            return "Membership with the same properties already exist";
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

        // get user by id
        var user = await _registrationService.GetUserAsync(membership.UserId);
        
        // check if user exist
        if (user == null)
        {
            _logger.LogInformation($"User with id: {membership.UserId} does not exist");
            _logger.LogInformation("Creating membership failed");
            return "Creating membership failed - user does not exist.";
        }

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

    public async Task<Membership> GetMembershipAsync(string membershipId)
    {
        try
        {
            var membership = await _membershipRepository.GetMembershipAsync(membershipId);
            _logger.LogInformation($"Membership retrieved: {membership.MembershipType}, {membership.IsActive}, {membership.UserId}");
            return membership;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve membership: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
    }
    
    //GetMembershipsByUserIdAsync
    public async Task<List<Membership>> GetMembershipsByUserIdAsync(string userId)
    {
        try
        {
            var memberships = await _membershipRepository.GetMembershipsByUserIdAsync(userId);
            _logger.LogInformation($"Memberships retrieved: {memberships}");
            return memberships;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve memberships: {ex.Message}");
            throw; // Handle or propagate the exception as needed
        }
    }
    
    // EditMembershipAsync
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
            membershipToEdit.MembershipType = membership.MembershipType;
            membershipToEdit.IsActive = membership.IsActive;
            membershipToEdit.UserId = membership.UserId;
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
