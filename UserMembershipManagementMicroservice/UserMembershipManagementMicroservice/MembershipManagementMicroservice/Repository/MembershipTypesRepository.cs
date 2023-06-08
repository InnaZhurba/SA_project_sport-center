using System.Collections;
using Cassandra;
using MembershipManagementMicroservice.Infrastructure;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;

namespace MembershipManagementMicroservice.Repository;

/// <summary>
///  This class is used to interact with the MembershipTypes table of the database.
///  It implements the IMembershipTypesRepository interface.
/// </summary>
public class MembershipTypesRepository : IMembershipTypesRepository
{
    private readonly CassandraConfig _cassandraConfig;
    
    /// <summary>
    ///  This constructor is used to inject the CassandraConfig into the MembershipTypesRepository class.
    /// </summary>
    /// <param name="cassandraConfig">
    /// The cassandraConfig argument represents the CassandraConfig dependency used to configure the Cassandra database
    /// </param>
    public MembershipTypesRepository(CassandraConfig cassandraConfig)
    {
        _cassandraConfig = cassandraConfig;
    }
    
    /// <summary>
    ///  This method is used to create a membership type in the database.
    /// </summary>
    /// <param name="membershipType">
    /// The membershipType argument represents the membership type to be created in the database
    /// </param>
    public async Task CreateMembershipTypeAsync(MembershipType membershipType)
    {
        string query = "INSERT INTO membership_types (id, name, price, description) VALUES (?, ?, ?, ?)";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            membershipType.Id, 
            membershipType.Name, 
            membershipType.Price, 
            membershipType.Description);
    }
    
    /// <summary>
    ///  This method is used to get a membership type from the database by id.
    /// </summary>
    /// <param name="membershipTypeId">
    /// The membershipTypeId argument represents the id of the membership type to be retrieved from the database
    /// </param>
    /// <returns>
    /// The method returns a MembershipType object if the membership type is found in the database, otherwise it returns null
    /// </returns>
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
    
    /// <summary>
    ///  This method is used to get all membership types from the database.
    /// </summary>
    /// <returns>
    /// The method returns a list of MembershipType objects if membership types are found in the database, otherwise it returns null
    /// </returns>
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
    
    /// <summary>
    ///  This method is used to update a membership type in the database.
    /// </summary>
    /// <param name="membershipType">
    /// The membershipType argument represents the membership type to be updated in the database
    /// </param>
    public async Task UpdateMembershipTypeAsync(MembershipType membershipType)
    {
        string query = "UPDATE membership_types SET name = ?, price = ?, description = ? WHERE id = ?";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            membershipType.Name, 
            membershipType.Price, 
            membershipType.Description, 
            membershipType.Id);
    }
    
    /// <summary>
    ///  This method is used to delete a membership type from the database by type id.
    /// </summary>
    /// <param name="membershipTypeId">
    /// The membershipTypeId argument represents the id of the membership type to be deleted from the database
    /// </param>
    public async Task DeleteMembershipTypeAsync(string membershipTypeId)
    {
        string query = "DELETE FROM membership_types WHERE id = ?";
        var id = Guid.Parse(membershipTypeId);
        var result = await _cassandraConfig.ExecuteQuery(query, id);
    }
    
   
    /// <summary>
    ///  This method is used to get a membership type from the database by name.
    /// </summary>
    /// <param name="membershipTypeName">
    /// The membershipTypeName argument represents the name of the membership type to be retrieved from the database
    /// </param>
    /// <returns>
    ///  The method returns a MembershipType object if the membership type is found in the database, otherwise it returns null
    /// </returns>
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