using CleanMonolith.Domain.Entities;

namespace CleanMonolith.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
