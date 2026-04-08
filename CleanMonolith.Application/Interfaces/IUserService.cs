using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMonolith.Application.DTOs;

namespace CleanMonolith.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(int id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<(IEnumerable<UserDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task<UserDto> CreateAsync(CreateUserDto createUserDto);
    Task UpdateAsync(int id, UpdateUserDto updateUserDto);
    Task DeleteAsync(int id);
    Task UpdatePasswordAsync(UpdatePasswordDto dto);
}
