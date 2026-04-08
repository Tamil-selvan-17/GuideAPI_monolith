using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanMonolith.Application.DTOs;
using CleanMonolith.Application.Interfaces;
using CleanMonolith.Domain.Entities;

namespace CleanMonolith.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        return MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(MapToDto);
    }

    public async Task<(IEnumerable<UserDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var (users, totalCount) = await _userRepository.GetPagedAsync(page, pageSize);

        return (users.Select(MapToDto), totalCount);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        // Check existing user
        var existing = await _userRepository.GetByLoginIdAsync(dto.LoginId);
        if (existing != null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        var user = new UserMaster
        {
            LoginId = dto.LoginId,
            LoginName = dto.LoginName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        user.LoginName = dto.LoginName;
        user.Email = dto.Email;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    // 🔥 Manual Mapper (Best for DB-first)
    private static UserDto MapToDto(UserMaster user)
    {
        return new UserDto
        {
            UserId = (int)user.UserId,
            LoginId = user.LoginId,
            LoginName = user.LoginName,
            Email = user.Email
        };
    }
    public async Task UpdatePasswordAsync(UpdatePasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.LoginId))
            throw new ArgumentException("LoginId is required");

        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters");

        var hash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        await _userRepository.UpdatePasswordAsync(dto.LoginId, hash);
        await _userRepository.SaveChangesAsync();
    }
}