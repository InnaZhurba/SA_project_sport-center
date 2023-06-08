using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Services.Interfaces;

/// <summary>
///  This interface is used to interact with the Discount table of the database.
/// </summary>
public interface IDiscountService
{
    Task<string> CreateDiscountAsync();
    Task<string> GetDiscountByIdAsync(string discountId);
    Task<string> GetDiscountByUserIdAsync(string userId);
    Task<string> UpdateDiscountAsync(Discount discount);
    Task<string> DeleteDiscountByIdAsync(string discountId);
    Task<string> GetAllDiscountsByUserIdAsync(string userId);
}