using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CleanMonolith.Application.DTOs;
using CleanMonolith.Application.Interfaces;
using CleanMonolith.Domain.Entities;

namespace CleanMonolith.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException($"User with id {id} not found.");
        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<(IEnumerable<UserDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var result = await _unitOfWork.Users.GetPagedAsync(page, pageSize);
        return (_mapper.Map<IEnumerable<UserDto>>(result.Items), result.TotalCount);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
    {
        var existingUsers = await _unitOfWork.Users.FindAsync(u => u.Email == createUserDto.Email || u.Username == createUserDto.Username);
        if (existingUsers.GetEnumerator().MoveNext())
        {
            throw new InvalidOperationException("User with this email or username already exists.");
        }

        var user = _mapper.Map<User>(createUserDto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        user.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task UpdateAsync(int id, UpdateUserDto updateUserDto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException($"User with id {id} not found.");

        _mapper.Map(updateUserDto, user);
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException($"User with id {id} not found.");

        _unitOfWork.Users.Remove(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
