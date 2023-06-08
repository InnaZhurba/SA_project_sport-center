using MembershipManagementMicroservice.Models;

namespace MembershipManagementMicroservice.Repository.Interfaces;

public interface IDiscountRepository
{
    Task CreateDiscountAsync(Discount discount);
    Task<Discount?> GetDiscountByIdAsync(string discountId);
    Task<Discount?> GetDiscountByUserIdAsync(string userId);
    Task UpdateDiscountAsync(Discount discount);
    Task DeleteDiscountByIdAsync(string discountId);
    Task DeleteDiscountByUserIdAsync(string userId);
    Task<List<Discount>> GetAllDiscountsByUserIdAsync(string userId);
}