using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Services.Interfaces;

/// <summary>
///  This interface is used to interact with the MembershipType table of the database.
/// </summary>
public interface IMembershipTypesService
{
    Task<string> CreateMembershipTypeAsync();
    Task<MembershipType> GetByIdMembershipTypeAsync(string membershipTypeId);
    Task<MembershipType?> GetByNameMembershipTypeAsync(string membershipTypeName);
    Task<List<MembershipType>> GetAllMembershipTypesAsync();
    Task<string> UpdateMembershipTypeAsync(MembershipType membershipType);
    Task<string> DeleteMembershipTypeAsync(string membershipTypeId);
}