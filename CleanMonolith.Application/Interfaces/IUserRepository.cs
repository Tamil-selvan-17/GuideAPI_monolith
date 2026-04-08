using CleanMonolith.Domain.Common;
using CleanMonolith.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CleanMonolith.Application.Interfaces;

public interface IUserRepository
{
    Task<UserMaster?> GetByIdAsync(long id);
    Task<IEnumerable<UserMaster>> GetAllAsync();
    Task<(IEnumerable<UserMaster>, int)> GetPagedAsync(int page, int pageSize);
    Task<UserMaster?> GetByLoginIdAsync(string loginId);
    Task UpdatePasswordAsync(string loginId, string passwordHash);

    Task AddAsync(UserMaster user);
    Task UpdateAsync(UserMaster user);
    Task DeleteAsync(UserMaster user);
    Task SaveChangesAsync();
}
