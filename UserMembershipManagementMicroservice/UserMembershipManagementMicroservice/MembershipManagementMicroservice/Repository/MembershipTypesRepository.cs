using System.Collections;
using Cassandra;
using MembershipManagementMicroservice.Infrastructure;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;

namespace MembershipManagementMicroservice.Repository;

public class MembershipTypesRepository : IMembershipTypesRepository
{
    private readonly CassandraConfig _cassandraConfig;
    
    public MembershipTypesRepository(CassandraConfig cassandraConfig)
    {
        _cassandraConfig = cassandraConfig;
    }
    
    public async Task CreateMembershipTypeAsync(MembershipType membershipType)
    {
        string query = "INSERT INTO membership_types (id, name, price, description) VALUES (?, ?, ?, ?)";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            membershipType.Id, 
            membershipType.Name, 
            membershipType.Price, 
            membershipType.Description);
    }
    
    public async Task<MembershipType?> GetMembershipTypeAsync(string membershipTypeId)
    {
        string query = "SELECT * FROM membership_types WHERE id = ?";
        var id = Guid.Parse(membershipTypeId);
        var result = await _cassandraConfig.ExecuteQuery(query, id);
        var row = result.FirstOrDefault();
        return (row == null
            ? null
            : new MembershipType
            {
                Id = row.GetValue<Guid>("id"),
                Name = row.GetValue<string>("name"),
                Price = row.GetValue<decimal>("price"),
                Description = row.GetValue<string>("description")
            });
    }
    
    public async Task<List<MembershipType>> GetALLMembershipTypesAsync()
    {
        string query = "SELECT * FROM membership_types";
        var result = await _cassandraConfig.ExecuteQuery(query);
        
        if (result == null)
        {
            return null;
        }
        
        var membershipTypes = new List<MembershipType>();
        
        foreach (var row in result)
        {
            var membershipType = new MembershipType
            {
                Id = row.GetValue<Guid>("id"),
                Name = row.GetValue<string>("name"),
                Price = row.GetValue<decimal>("price"),
                Description = row.GetValue<string>("description")
            };
            
            membershipTypes.Add(membershipType);
        }
        
        return membershipTypes;
    }
    
    public async Task UpdateMembershipTypeAsync(MembershipType membershipType)
    {
        string query = "UPDATE membership_types SET name = ?, price = ?, description = ? WHERE id = ?";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            membershipType.Name, 
            membershipType.Price, 
            membershipType.Description, 
            membershipType.Id);
    }
    
    public async Task DeleteMembershipTypeAsync(string membershipTypeId)
    {
        string query = "DELETE FROM membership_types WHERE id = ?";
        var id = Guid.Parse(membershipTypeId);
        var result = await _cassandraConfig.ExecuteQuery(query, id);
    }
    
   
    public async Task<MembershipType?> GetMembershipTypeByNameAsync(string membershipTypeName)
    {
        string query = "SELECT * FROM membership_types WHERE name = ? ALLOW FILTERING";
        var result = await _cassandraConfig.ExecuteQuery(query, membershipTypeName);

        var row = result.FirstOrDefault();
        return (row == null
            ? null
            : new MembershipType
            {
                Id = row.GetValue<Guid>("id"),
                Name = row.GetValue<string>("name"),
                Price = row.GetValue<decimal>("price"),
                Description = row.GetValue<string>("description")
            });
    }


}