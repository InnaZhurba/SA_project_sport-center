using MembershipManagementMicroservice.Infrastructure;
using MembershipManagementMicroservice.Models;
using MembershipManagementMicroservice.Repository.Interfaces;

namespace MembershipManagementMicroservice.Repository;

/// <summary>
///  The DiscountRepository class implements the IDiscountRepository interface.
///  It is used to interact with the Discount table of the database.
/// </summary>
public class DiscountRepository : IDiscountRepository
{
    private readonly CassandraConfig _cassandraConfig;
    
    /// <summary>
    ///  This constructor is used to inject the CassandraConfig into the DiscountRepository class.
    /// </summary>
    /// <param name="cassandraConfig">
    /// The cassandraConfig argument represents the CassandraConfig dependency used to configure the Cassandra database
    /// </param>
    public DiscountRepository(CassandraConfig cassandraConfig)
    {
        _cassandraConfig = cassandraConfig;
    }
    
    /// <summary>
    ///  This method is used to create a discount in the database.
    /// </summary>
    /// <param name="discount">
    /// The discount argument represents the discount to be created in the database 
    /// </param>
    public async Task CreateDiscountAsync(Discount discount)
    {
        string query = "INSERT INTO discounts (id, user_id, percentage, start_date, end_date, is_active) VALUES (?, ?, ?, ?, ?, ?)";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            discount.Id, 
            discount.UserId, 
            discount.Percentage, 
            discount.StartDate, 
            discount.EndDate, 
            discount.IsActive);
    }
    
    /// <summary>
    ///  This method is used to get a discount from the database by id.
    /// </summary>
    /// <param name="discountId">
    /// The discountId argument represents the id of the discount to be retrieved from the database
    /// </param>
    /// <returns>
    /// The method returns a Discount object if the discount is found in the database, otherwise it returns null
    /// </returns>
    public async Task<Discount?> GetDiscountByIdAsync(string discountId)
    {
        string query = "SELECT * FROM discounts WHERE Id = ?";
        var id = Guid.Parse(discountId);
        var result = await _cassandraConfig.ExecuteQuery(query, id);
        var row = result.FirstOrDefault();
        return ( row == null ? null : new Discount
        {
            Id = row.GetValue<Guid>("id"),
            UserId = row.GetValue<Guid>("user_id"),
            Percentage = row.GetValue<decimal>("percentage"),
            StartDate = row.GetValue<DateTime>("start_date"),
            EndDate = row.GetValue<DateTime>("end_date"),
            IsActive = row.GetValue<bool>("is_active")
        });
    }
    
    /// <summary>
    ///  This method is used to get a discount from the database by user_id.
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents the user_id of the discount to be retrieved from the database
    /// </param>
    /// <returns>
    /// The method returns a Discount object if the discount is found in the database, otherwise it returns null
    /// </returns>
    public async Task<Discount?> GetDiscountByUserIdAsync(string userId)
    {
        string query = "SELECT * FROM discounts WHERE user_id = ? ALLOW FILTERING";
        var id = Guid.Parse(userId);
        var result = await _cassandraConfig.ExecuteQuery(query, id);
        var row = result.FirstOrDefault();
        return ( row == null ? null : new Discount
        {
            Id = row.GetValue<Guid>("id"),
            UserId = row.GetValue<Guid>("user_id"),
            Percentage = row.GetValue<decimal>("percentage"),
            StartDate = row.GetValue<DateTime>("start_date"),
            EndDate = row.GetValue<DateTime>("end_date"),
            IsActive = row.GetValue<bool>("is_active")
        });
    }
    
    /// <summary>
    ///  This method is used to update a discount in the database.
    /// </summary>
    /// <param name="discount">
    /// The discount argument represents the discount to be updated in the database
    /// </param>
    public async Task UpdateDiscountAsync(Discount discount)
    {
        string query = "UPDATE discounts SET percentage = ?, start_date = ?, end_date = ?, is_active = ? WHERE id = ?";
        var result = await _cassandraConfig.ExecuteQuery(query, 
            discount.Percentage, 
            discount.StartDate, 
            discount.EndDate, 
            discount.IsActive, 
            discount.Id);
    }
    
    /// <summary>
    ///  This method is used to delete a discount from the database by id.
    /// </summary>
    /// <param name="discountId">
    /// The discountId argument represents the id of the discount to be deleted from the database
    /// </param>
    public async Task DeleteDiscountByIdAsync(string discountId)
    {
        string query = "DELETE FROM discounts WHERE id = ?";
        var id = Guid.Parse(discountId);
        var result = await _cassandraConfig.ExecuteQuery(query, id);
    }
    
    /// <summary>
    ///  This method is used to delete a discount from the database by user_id.
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents the user_id of the discount to be deleted from the database
    /// </param>
    public async Task DeleteDiscountByUserIdAsync(string userId)
    {
        string query = "DELETE FROM discounts WHERE user_id = ?";
        var id = Guid.Parse(userId);
        var result = await _cassandraConfig.ExecuteQuery(query, id);
    }
    
    /// <summary>
    ///  This method is used to get all discounts from the database that are for some user.
    /// </summary>
    /// <param name="userId">
    /// The userId argument represents the user_id of the discounts to be retrieved from the database
    /// </param>
    /// <returns>
    /// The method returns a list of Discount objects if the discounts are found in the database, otherwise it returns an empty list
    /// </returns>
    public async Task<List<Discount>> GetAllDiscountsByUserIdAsync(string userId)
    {
        string query = "SELECT * FROM discounts WHERE user_id = ? ALLOW FILTERING";
        var id = Guid.Parse(userId);
        var result = await _cassandraConfig.ExecuteQuery(query, id);
        var discounts = new List<Discount>();
        foreach (var row in result)
        {
            discounts.Add(new Discount
            {
                Id = row.GetValue<Guid>("id"),
                UserId = row.GetValue<Guid>("user_id"),
                Percentage = row.GetValue<decimal>("percentage"),
                StartDate = row.GetValue<DateTime>("start_date"),
                EndDate = row.GetValue<DateTime>("end_date"),
                IsActive = row.GetValue<bool>("is_active")
            });
        }
        return discounts;
    }
}