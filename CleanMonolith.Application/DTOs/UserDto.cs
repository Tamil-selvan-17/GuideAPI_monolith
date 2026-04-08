namespace CleanMonolith.Application.DTOs;

public class UserDto
{
    public int UserId { get; set; }

    public string LoginId { get; set; } = string.Empty;
    public string LoginName { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
