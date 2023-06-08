using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Services.Interfaces;

/// <summary>
///  This interface is used to interact with the Discount table of the database.
/// </summary>
public interface IDiscountService
{
    Task<string> CreateDiscountAsync();
    Task<Discount?> GetDiscountByIdAsync(string discountId);
    Task<Discount?> GetDiscountByUserIdAsync(string userId);
    Task<string> UpdateDiscountAsync(Discount discount);
    Task<string> DeleteDiscountByIdAsync(string discountId);
    Task<List<Discount?>> GetAllDiscountsByUserIdAsync(string userId);
}