using MembershipManagementMicroservice.Infrastructure;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;

namespace MembershipManagementMicroservice.Repository;

public class MembershipRepository : IMembershipRepository
{
    private readonly CassandraConfig _cassandraConfig;

    public MembershipRepository(CassandraConfig cassandraConfig)
    {
        _cassandraConfig = cassandraConfig;
    }

    public async Task CreateMembershipAsync(Membership membership)
    {
        var user_id = Guid.Parse(membership.UserId);
        
        string query = "INSERT INTO memberships (Id, User_id, Membership_type, is_active, Start_date, End_date) VALUES (?, ?, ?, ?, ?, ?)";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            membership.Id, 
            Guid.Parse(membership.UserId),
            //membership.UserId, 
            membership.MembershipType, 
            membership.IsActive, 
            membership.StartDate, 
            membership.EndDate);
    }

    public async Task<Membership?> GetMembershipAsync(string membershipId)
    {
        string query = "SELECT * FROM memberships WHERE Id = ?";
        var id = Guid.Parse(membershipId);
        var result = await _cassandraConfig.ExecuteQuery(query, id);
        var row = result.FirstOrDefault();
        return (row == null
            ? null
            : new Membership
            {
                Id = row.GetValue<Guid>("id"),
                UserId = row.GetValue<Guid>("user_id").ToString(),
                MembershipType = row.GetValue<string>("membership_type"),
                StartDate = row.GetValue<DateTime>("start_date"),
                EndDate = row.GetValue<DateTime>("end_date"),
                IsActive = row.GetValue<bool>("is_active")
            });
    }

    // GetMembershipsByUserIdAsync
    public async Task<List<Membership>> GetMembershipsByUserIdAsync(string userId)
    {
        string query = "SELECT * FROM memberships WHERE User_id = ? ALLOW FILTERING";
        var id = Guid.Parse(userId);
        var result = await _cassandraConfig.ExecuteQuery(query, id);
        var rows = result.ToList();
        var memberships = new List<Membership>();
        foreach (var row in rows)
        {
            memberships.Add(new Membership
            {
                Id = row.GetValue<Guid>("id"),
                UserId = userId,
                MembershipType = row.GetValue<string>("membership_type"),
                StartDate = row.GetValue<DateTime>("start_date"),
                EndDate = row.GetValue<DateTime>("end_date"),
                IsActive = row.GetValue<bool>("is_active")
            });
        }
        return memberships;
    }
    
    // EditMembershipAsync
    public async Task EditMembershipAsync(Membership membership)
    {
        string query = "UPDATE memberships SET Membership_type = ?, is_active = ?, Start_date = ?, End_date = ? WHERE Id = ?";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            membership.MembershipType, 
            membership.IsActive, 
            membership.StartDate, 
            membership.EndDate,
            membership.Id);
    }
}