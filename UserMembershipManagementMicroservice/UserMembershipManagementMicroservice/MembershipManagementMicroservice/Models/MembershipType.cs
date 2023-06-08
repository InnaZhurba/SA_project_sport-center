namespace MembershipManagementMicroservice.Models;

public class MembershipType
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}