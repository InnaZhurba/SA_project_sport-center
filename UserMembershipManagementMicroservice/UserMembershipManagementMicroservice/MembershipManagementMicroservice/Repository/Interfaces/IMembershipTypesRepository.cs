using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Repository.Interfaces;

public interface IMembershipTypesRepository
{
    Task CreateMembershipTypeAsync(MembershipType membershipType);
    Task<MembershipType?> GetMembershipTypeAsync(string membershipTypeId);
    Task<List<MembershipType>> GetALLMembershipTypesAsync();
    Task UpdateMembershipTypeAsync(MembershipType membershipType);
    Task DeleteMembershipTypeAsync(string membershipTypeId);
    Task<MembershipType?> GetMembershipTypeByNameAsync(string membershipTypeName);
}