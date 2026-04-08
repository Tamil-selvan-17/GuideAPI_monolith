using System;
using CleanMonolith.Domain.Common;

namespace CleanMonolith.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public long UserId { get; set; } 
    public string TokenHash { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiryDate { get; set; }

    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
}
