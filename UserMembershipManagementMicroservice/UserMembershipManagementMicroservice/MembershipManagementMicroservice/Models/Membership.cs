namespace MembershipManagementMicroservice.Models;

public class Membership
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string MembershipType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}