using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using CleanMonolith.Application.DTOs;
using CleanMonolith.Application.Interfaces;
using CleanMonolith.Domain.Entities;

namespace CleanMonolith.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(IUnitOfWork unitOfWork, IJwtProvider jwtProvider)
    {
        _unitOfWork = unitOfWork;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var users = await _unitOfWork.Users.FindAsync(u => u.Username == request.Username);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var token = _jwtProvider.GenerateToken(user);
        var refreshTokenValue = _jwtProvider.GenerateRefreshToken();
        
        var refreshToken = new RefreshToken
        {
            TokenHash = BCrypt.Net.BCrypt.HashPassword(refreshTokenValue),
            JwtId = Guid.NewGuid().ToString(), // Should map to exact Jwt Id if needed
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };
        
        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = token,
            RefreshToken = refreshTokenValue,
            UserId = user.Id,
            Username = user.Username
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(TokenRefreshRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) throw new UnauthorizedAccessException("Invalid client.");

        var activeTokens = await _unitOfWork.RefreshTokens.FindAsync(rt => rt.UserId == request.UserId && !rt.IsUsed && !rt.IsRevoked);
        
        var refreshToken = activeTokens.FirstOrDefault(rt => BCrypt.Net.BCrypt.Verify(request.RefreshToken, rt.TokenHash));
        if (refreshToken == null || refreshToken.ExpiryDate <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        // Mark as used
        refreshToken.IsUsed = true;
        _unitOfWork.RefreshTokens.Update(refreshToken);
        
        // Generate new tokens
        var newAccessToken = _jwtProvider.GenerateToken(user);
        var newRefreshTokenValue = _jwtProvider.GenerateRefreshToken();
        
        var newRefreshToken = new RefreshToken
        {
            TokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshTokenValue),
            JwtId = Guid.NewGuid().ToString(),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };
        
        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            UserId = user.Id,
            Username = user.Username
        };
    }

    public async Task LogoutAsync(int userId, string refreshTokenValue)
    {
        var activeTokens = await _unitOfWork.RefreshTokens.FindAsync(rt => rt.UserId == userId && !rt.IsUsed && !rt.IsRevoked);
        var refreshToken = activeTokens.FirstOrDefault(rt => BCrypt.Net.BCrypt.Verify(refreshTokenValue, rt.TokenHash));
        
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            _unitOfWork.RefreshTokens.Update(refreshToken);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
