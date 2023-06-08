using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;
using MembershipManagementMicroservice.Services.Interfaces;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Services;

/// <summary>
///  This class implements the IMembershipTypesService interface.
/// </summary>
public class MembershipTypesService : IMembershipTypesService
{
    private readonly IMembershipTypesRepository _membershipTypesRepository;
    private readonly IKafkaConsumerService _kafkaConsumerService;
    private readonly ILogger<MembershipService> _logger;
    
    
    public MembershipTypesService(IMembershipTypesRepository membershipTypesRepository, IKafkaConsumerService kafkaConsumerService, ILogger<MembershipService> logger)
    {
        _membershipTypesRepository = membershipTypesRepository;
        _kafkaConsumerService = kafkaConsumerService;
        _logger = logger;
    }
    
    /// <summary>
    ///  This method is used to create a membership type.
    /// </summary>
    /// <returns>
    /// A string that says whether the membership type was created or not.
    /// </returns>
    public async Task<string> CreateMembershipTypeAsync()
    {
        _logger.LogInformation("Creating membership type unpacked...");
        // consume from kafka topic and create membership
        var membershipTypeJson = await _kafkaConsumerService.ConsumeMessagesAsync("membership_type_post_topic");
        _logger.LogInformation($"Membership type JSON: {membershipTypeJson}");

        // unpack the membership object from the JSON string
        var membershipType_new = JsonConvert.DeserializeObject<MembershipType>(membershipTypeJson);
        
        membershipType_new.Id = Guid.NewGuid();
        
        await _membershipTypesRepository.CreateMembershipTypeAsync(membershipType_new);
        return "Membership type created";
    }
    
    /// <summary>
    ///  This method is used to get a membership type by id.
    /// </summary>
    /// <param name="membershipTypeId">
    /// A string that represents the id of the membership type.
    /// </param>
    /// <returns>
    /// A membership type object.
    /// </returns>
    public async Task<MembershipType> GetByIdMembershipTypeAsync(string membershipTypeId)
    {
        _logger.LogInformation("Getting membership type by id...");
        var membershipType = await _membershipTypesRepository.GetMembershipTypeAsync(membershipTypeId);
        return membershipType;
    }
    
    /// <summary>
    ///  This method is used to get a membership type by name.
    /// </summary>
    /// <param name="membershipTypeName">
    /// A string that represents the name of the membership type.
    /// </param>
    /// <returns>
    /// A membership type object.
    /// </returns>
    public async Task<MembershipType?> GetByNameMembershipTypeAsync(string membershipTypeName)
    {
        _logger.LogInformation("Getting membership type by name...");
        var membershipType = await _membershipTypesRepository.GetMembershipTypeByNameAsync(membershipTypeName);
        if( membershipType == null)
        {
            return null;
        }
        return membershipType;
    }
    
    /// <summary>
    ///  This method is used to get all membership types.
    /// </summary>
    /// <returns>
    /// A list of membership types.
    /// </returns>
    public async Task<List<MembershipType>> GetAllMembershipTypesAsync()
    {
        _logger.LogInformation("Getting all membership types...");
        var membershipTypes = await _membershipTypesRepository.GetALLMembershipTypesAsync();
        return membershipTypes;
    }
    
    /// <summary>
    ///  This method is used to update a membership type.
    /// </summary>
    /// <param name="membershipType">
    /// A membership type object.
    /// </param>
    /// <returns>
    /// A string that says whether the membership type was updated or not.
    /// </returns>
    public async Task<string> UpdateMembershipTypeAsync(MembershipType membershipType)
    {
        _logger.LogInformation("Updating membership type...");
        
        if( await _membershipTypesRepository.GetMembershipTypeAsync(membershipType.Id.ToString()) == null)
        {
            return "Membership type does not exist";
        }

        await _membershipTypesRepository.UpdateMembershipTypeAsync(membershipType);
        return "Membership type updated";
    }
    
    /// <summary>
    ///  This method is used to delete a membership type.
    /// </summary>
    /// <param name="membershipTypeId">
    /// A string that represents the id of the membership type.
    /// </param>
    /// <returns>
    /// A string that says whether the membership type was deleted or not.
    /// </returns>
    public async Task<string> DeleteMembershipTypeAsync(string membershipTypeId)
    {
        _logger.LogInformation("Deleting membership type...");
        await _membershipTypesRepository.DeleteMembershipTypeAsync(membershipTypeId);
        return "Membership type deleted";
    }
}