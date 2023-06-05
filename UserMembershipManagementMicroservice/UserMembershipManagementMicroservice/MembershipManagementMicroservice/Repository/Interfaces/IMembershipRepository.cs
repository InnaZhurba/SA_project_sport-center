using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Repository.Interfaces;

/// <summary>
///  This interface is used to interact with the Membership table of the database.
///  It is implemented by the MembershipRepository class.
/// </summary>
public interface IMembershipRepository
{
    Task CreateMembershipAsync(Membership membership);
    Task<Membership?> GetMembershipAsync(string membershipId);
    Task<List<Membership>> GetMembershipsByUserIdAsync(string userId);
    Task EditMembershipAsync(Membership membership);
}