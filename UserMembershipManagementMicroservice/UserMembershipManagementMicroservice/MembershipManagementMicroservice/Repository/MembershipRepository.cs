using MembershipManagementMicroservice.Infrastructure;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;

namespace MembershipManagementMicroservice.Repository;

/// <summary>
///  This class is used to interact with the Membership table of the database.
///  It implements the IMembershipRepository interface.
/// </summary>
public class MembershipRepository : IMembershipRepository
{
    private readonly CassandraConfig _cassandraConfig;

    /// <summary>
    ///  This constructor is used to inject the CassandraConfig into the MembershipRepository class.
    /// </summary>
    /// <param name="cassandraConfig">
    /// The cassandraConfig argument represents the CassandraConfig dependency used to configure the Cassandra database
    /// </param>
    public MembershipRepository(CassandraConfig cassandraConfig)
    {
        _cassandraConfig = cassandraConfig;
    }

    /// <summary>
    ///  This method is used to create a membership in the database.
    /// </summary>
    /// <param name="membership">
    /// The membership argument represents the membership to be created in the database
    /// </param>
    public async Task CreateMembershipAsync(Membership membership)
    {
        var user_id = Guid.Parse(membership.UserId);
        
        string query = "INSERT INTO memberships (Id, User_id, Membership_type_id, is_active, Start_date, End_date, price) VALUES (?, ?, ?, ?, ?, ?, ?)";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            membership.Id, 
            Guid.Parse(membership.UserId),
            //membership.UserId, 
            membership.MembershipTypeId, 
            membership.IsActive, 
            membership.StartDate, 
            membership.EndDate,
            membership.Price);
    }

    /// <summary>
    ///  This method is used to get a membership from the database.
    /// </summary>
    /// <param name="membershipId">
    /// The membershipId argument represents the id of the membership to be retrieved from the database
    /// </param>
    /// <returns>
    /// The method returns a Membership object if the membership is found in the database, otherwise it returns null
    /// </returns>
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
                MembershipTypeId = row.GetValue<Guid>("membership_type_id"),
                StartDate = row.GetValue<DateTime>("start_date"),
                EndDate = row.GetValue<DateTime>("end_date"),
                IsActive = row.GetValue<bool>("is_active"),
                Price = row.GetValue<decimal>("price")
            });
    }

    /// <summary>
    ///      This method is used to get a list of memberships from the database.
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents the id of the user whose memberships are to be retrieved from the database 
    /// </param>
    /// <returns>
    /// The method returns a list of Membership objects if the user has memberships in the database, otherwise it returns an empty list of Membership objects
    /// </returns>
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
                MembershipTypeId = row.GetValue<Guid>("membership_type_id"),
                StartDate = row.GetValue<DateTime>("start_date"),
                EndDate = row.GetValue<DateTime>("end_date"),
                IsActive = row.GetValue<bool>("is_active"),
                Price = row.GetValue<decimal>("price")
            });
        }
        return memberships;
    }
    
    /// <summary>
    ///  This method is used to edit a membership in the database.
    /// </summary>
    /// <param name="membership">
    /// The membership argument represents the membership to be edited in the database 
    /// </param>
    public async Task EditMembershipAsync(Membership membership)
    {
        string query = "UPDATE memberships SET Membership_type_id = ?, is_active = ?, Start_date = ?, End_date = ?, price = ? WHERE Id = ?";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            membership.MembershipTypeId, 
            membership.IsActive, 
            membership.StartDate, 
            membership.EndDate,
            membership.Price,
            membership.Id);
    }
}