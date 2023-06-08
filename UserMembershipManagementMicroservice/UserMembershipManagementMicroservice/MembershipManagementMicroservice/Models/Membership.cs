namespace MembershipManagementMicroservice.Models;

/// <summary>
///  The Membership class represents a membership in the MembershipManagementMicroservice
///  It contains properties representing the membership's Id, UserId, MembershipType, StartDate, EndDate, and IsActive
///  It is used by the MembershipController class
/// </summary>
public class Membership
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Guid MembershipTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } // active is mean that it is paid
    public decimal Price { get; set; }
}