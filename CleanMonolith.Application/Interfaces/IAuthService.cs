using System.Threading.Tasks;
using CleanMonolith.Application.DTOs;

namespace CleanMonolith.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(TokenRefreshRequest request);
    Task LogoutAsync(int userId, string refreshToken);
}
