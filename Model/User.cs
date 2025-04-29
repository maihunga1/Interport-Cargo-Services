using System.ComponentModel.DataAnnotations;

namespace Model;


// First Name, Family Name, Email address, Phone number, Company name (if applicable), Address, and password
public class User
{
    [MaxLength(50)]
    [MinLength(3)]
    public required string UserName { get; set; }
    [Key]
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string FamilyName { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string CompanyName { get; set; }
    public required string Address { get; set; }
    public required string Password { get; set; }
}