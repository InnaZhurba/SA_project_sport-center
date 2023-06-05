using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Services.Interfaces;

/// <summary>
///  This interface is used to interact with the Membership table of the database.
///  It is implemented by the MembershipService class.
///  It contains methods for creating a membership, getting a membership by id, getting all memberships with a given user id, and editing a membership.
/// </summary>
public interface IMembershipService
{
    Task<string> CreateMembershipAsync();
    Task<Membership> GetMembershipAsync(string membershipId);
    Task<List<Membership>> GetMembershipsByUserIdAsync(string userId);
    Task<Membership> EditMembershipAsync(string membershipId, Membership membership);
}