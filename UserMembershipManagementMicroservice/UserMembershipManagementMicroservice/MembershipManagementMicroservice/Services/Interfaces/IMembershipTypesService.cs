using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Services.Interfaces;

public interface IMembershipTypesService
{
    Task<string> CreateMembershipTypeAsync();
    Task<MembershipType> GetByIdMembershipTypeAsync(string membershipTypeId);
    Task<MembershipType?> GetByNameMembershipTypeAsync(string membershipTypeName);
    Task<List<MembershipType>> GetAllMembershipTypesAsync();
    Task<string> UpdateMembershipTypeAsync(MembershipType membershipType);
    Task<string> DeleteMembershipTypeAsync(string membershipTypeId);
}