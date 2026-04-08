namespace CleanMonolith.Application.DTOs;

public class CreateUserDto
{
    public string LoginId { get; set; } = string.Empty;   // Username
    public string LoginName { get; set; } = string.Empty; // Display name
    public string Password { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
