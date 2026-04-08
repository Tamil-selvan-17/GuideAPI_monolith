using System;
using System.Linq;
using System.Threading.Tasks;
using CleanMonolith.Application.DTOs;
using CleanMonolith.Application.Interfaces;
using CleanMonolith.Domain.Entities;

namespace CleanMonolith.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<(bool status, string message, AuthResponse? data)> LoginAsync(LoginRequest request)
    {
        // 1. Get user
        var user = await _userRepository.GetByLoginIdAsync(request.LoginName);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return (false, "Invalid username or password", null);
        }

        // 2. Generate tokens
        var accessToken = _jwtProvider.GenerateToken(user);
        var refreshTokenValue = _jwtProvider.GenerateRefreshToken();

        // 3. Store refresh token (hashed)
        var refreshToken = new RefreshToken
        {
            TokenHash = BCrypt.Net.BCrypt.HashPassword(refreshTokenValue),
            JwtId = Guid.NewGuid().ToString(),
            UserId = user.UserId, // ✅ no cast
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        // 4. Return response
        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            UserId = (int)user.UserId, // only here for DTO
            Username = user.LoginId
        };

        return (true, "Login successful", response);
    }

    public async Task<AuthResponse> RefreshTokenAsync(TokenRefreshRequest request)
    {
        // 1. Get user
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid user.");
        }

        // 2. Get active tokens
        var activeTokens = await _refreshTokenRepository
            .GetActiveTokensByUserIdAsync(request.UserId);

        // 3. Validate refresh token
        var existingToken = activeTokens
            .FirstOrDefault(rt => BCrypt.Net.BCrypt.Verify(request.RefreshToken, rt.TokenHash));

        if (existingToken == null || existingToken.ExpiryDate <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        // 4. Mark old token as used
        existingToken.IsUsed = true;
        await _refreshTokenRepository.UpdateAsync(existingToken);

        // 5. Generate new tokens
        var newAccessToken = _jwtProvider.GenerateToken(user);
        var newRefreshTokenValue = _jwtProvider.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            TokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshTokenValue),
            JwtId = Guid.NewGuid().ToString(),
            UserId = (int)user.UserId,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            UserId = (int)user.UserId,
            Username = user.LoginId
        };
    }

    public async Task LogoutAsync(int userId, string refreshTokenValue)
    {
        var activeTokens = await _refreshTokenRepository
            .GetActiveTokensByUserIdAsync(userId);

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(token);
        }

        await _refreshTokenRepository.SaveChangesAsync();
    }
}