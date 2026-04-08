using CleanMonolith.Domain.Common;

namespace CleanMonolith.Domain.Entities;

public class UserMaster : BaseEntity
{
    public long UserId { get; set; }
    public string LoginId { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? LoginName { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string ActiveStatus { get; set; } = "Active";
    public int? RoleId { get; set; }

    public string? Email { get; set; }
    public string? PhoneNo { get; set; }

    public int? LoginFailedCount { get; set; }

    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }

    public bool IsDeleted { get; set; }
}