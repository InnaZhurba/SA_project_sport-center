using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Services.Interfaces;

public interface IMembershipService
{
    Task<string> CreateMembershipAsync();
    Task<Membership> GetMembershipAsync(string membershipId);
    Task<List<Membership>> GetMembershipsByUserIdAsync(string userId);
    Task<Membership> EditMembershipAsync(string membershipId, Membership membership);
}