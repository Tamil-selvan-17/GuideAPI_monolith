using System;
using CleanMonolith.Domain.Common;

namespace CleanMonolith.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string TokenHash { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty;
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime ExpiryDate { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
