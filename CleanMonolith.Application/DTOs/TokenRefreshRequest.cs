namespace CleanMonolith.Application.DTOs;

public class TokenRefreshRequest
{
    public int UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}
