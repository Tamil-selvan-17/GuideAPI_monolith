using CleanMonolith.Domain.Entities;

namespace CleanMonolith.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(UserMaster user);
    string GenerateRefreshToken();
}
