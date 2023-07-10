namespace LoggingApp.Shared.Models;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Nationality { get; set; }
    public String RoleName { get; set; }
}