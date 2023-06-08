namespace MembershipManagementMicroservice.Models;

/// <summary>
///  The Discount class represents a Discount object
///  It contains properties that represent the database columns
///  Discount needed to see if a user is eligible for a discount
/// </summary>
public class Discount
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}