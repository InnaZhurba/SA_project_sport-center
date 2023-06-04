using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Repository.Interfaces;

public interface IMembershipRepository
{
    Task CreateMembershipAsync(Membership membership);
    Task<Membership?> GetMembershipAsync(string membershipId);
    Task<List<Membership>> GetMembershipsByUserIdAsync(string userId);
    Task EditMembershipAsync(Membership membership);
}