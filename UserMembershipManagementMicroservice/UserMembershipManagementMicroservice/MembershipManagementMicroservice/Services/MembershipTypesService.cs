using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;
using MembershipManagementMicroservice.Services.Interfaces;
using Newtonsoft.Json;

namespace MembershipManagementMicroservice.Services;

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
    
    public async Task<string> CreateMembershipTypeAsync()
    {
        _logger.LogInformation("Creating membership type unpacked...");
        // consume from kafka topic and create membership
        var membershipTypeJson = await _kafkaConsumerService.ConsumeMessagesAsync("membership_type_get_topic");
        _logger.LogInformation($"Membership type JSON: {membershipTypeJson}");

        // unpack the membership object from the JSON string
        var membershipType_new = JsonConvert.DeserializeObject<MembershipType>(membershipTypeJson);
        
        membershipType_new.Id = Guid.NewGuid();
        
        await _membershipTypesRepository.CreateMembershipTypeAsync(membershipType_new);
        return "Membership type created";
    }
    
    public async Task<MembershipType> GetByIdMembershipTypeAsync(string membershipTypeId)
    {
        _logger.LogInformation("Getting membership type by id...");
        var membershipType = await _membershipTypesRepository.GetMembershipTypeAsync(membershipTypeId);
        return membershipType;
    }
    
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
    
    public async Task<List<MembershipType>> GetAllMembershipTypesAsync()
    {
        _logger.LogInformation("Getting all membership types...");
        var membershipTypes = await _membershipTypesRepository.GetALLMembershipTypesAsync();
        return membershipTypes;
    }
    
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
    
    public async Task<string> DeleteMembershipTypeAsync(string membershipTypeId)
    {
        _logger.LogInformation("Deleting membership type...");
        await _membershipTypesRepository.DeleteMembershipTypeAsync(membershipTypeId);
        return "Membership type deleted";
    }
}