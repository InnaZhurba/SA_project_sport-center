namespace MembershipManagementMicroservice.Models;

/// <summary>
///  The User class represents a user in the MembershipManagementMicroservice
///  It contains properties representing the user's Id, Username, Password, and Email
///  It is used by the MembershipController class
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}