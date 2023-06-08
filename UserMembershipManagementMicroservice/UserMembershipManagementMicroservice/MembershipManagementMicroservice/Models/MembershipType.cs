namespace MembershipManagementMicroservice.Models;

/// <summary>
///  The MembershipType class represents a MembershipType object
///  It contains properties that represent the database columns
///  It shows the different types of memberships that a user can have
/// </summary>
public class MembershipType
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}