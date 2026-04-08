using System.Threading.Tasks;
using CleanMonolith.Application.DTOs;
using CleanMonolith.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanMonolith.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto request)
    {
        var user = await _userService.CreateAsync(request);
        return CreatedAtAction("GetById", "Users", new { id = user.UserId }, user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.status || result.data == null)
        {
            return Unauthorized(new { status = result.status, message = result.message });
        }

        SetAccessTokenCookie(result.data.AccessToken);
        SetRefreshTokenCookie(result.data.RefreshToken);

        return Ok(new
        {
            status = result.status,
            message = result.message,
            result.data.UserId,
            result.data.Username
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenRefreshRequest request)
    {
        var cookieToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(cookieToken))
        {
            request.RefreshToken = cookieToken; // use http-only cookie token if present
        }

        var response = await _authService.RefreshTokenAsync(request);
        SetRefreshTokenCookie(response.RefreshToken);
        
        return Ok(new { response.AccessToken, response.UserId, response.Username });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            var userId = int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0"
            );

            // Revoke this specific refresh token
            await _authService.LogoutAsync(userId, refreshToken);
        }

        // ✅ Delete BOTH cookies properly
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        };

        Response.Cookies.Delete("refreshToken", cookieOptions);
        Response.Cookies.Delete("accessToken", cookieOptions);

        return Ok(new { message = "Logged out successfully" });
    }

    private void SetRefreshTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7),
            Secure = true, // required for SameSite=None
            SameSite = SameSiteMode.None // 🔥 FIX
        };

        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    private void SetAccessTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None, // 🔥 FIX
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        Response.Cookies.Append("accessToken", token, cookieOptions);
    }
    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
    {
        await _userService.UpdatePasswordAsync(dto);
        return Ok(new { message = "Password updated successfully" });
    }
}
