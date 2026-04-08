using CleanMonolith.Application.Interfaces;
using CleanMonolith.Domain.Entities;
using CleanMonolith.Infrastructure.Entity;
using CleanMonolith.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleanMonolith.Infrastructure.Repositories;

public class UserRepository : IUserRepository,IDisposable
{
    private readonly nEdit_DEVContext _context;

    public UserRepository(nEdit_DEVContext context)
    {
        _context = context;
    }

    public async Task<UserMaster?> GetByLoginIdAsync(string loginId)
    {
        var entity = await _context.Tbl_UserMasters
            .FirstOrDefaultAsync(x => x.LoginID == loginId && x.IsDeleted == 0);

        return entity == null ? null : UserMapper.ToDomain(entity);
    }

    public async Task<UserMaster?> GetByIdAsync(long id)
    {
        var entity = await _context.Tbl_UserMasters
            .FirstOrDefaultAsync(x => x.UserID == id && x.IsDeleted == 0);

        return entity == null ? null : UserMapper.ToDomain(entity);
    }

    public async Task<IEnumerable<UserMaster>> GetAllAsync()
    {
        var entities = await _context.Tbl_UserMasters
            .Where(x => x.IsDeleted == 0)
            .ToListAsync();

        return entities.Select(UserMapper.ToDomain);
    }

    public async Task<(IEnumerable<UserMaster>, int)> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Tbl_UserMasters
            .Where(x => x.IsDeleted == 0);

        var totalCount = await query.CountAsync();

        var entities = await query
            .OrderBy(x => x.UserID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (entities.Select(UserMapper.ToDomain), totalCount);
    }

    public async Task AddAsync(UserMaster user)
    {
        var entity = new Tbl_UserMaster
        {
            LoginID = user.LoginId,
            LoginName = user.LoginName,
            EmailID = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Password = user.PasswordHash,

            ActiveStatus = "Active",
            IsDeleted = 0,
            CreatedBy = 1,
            CreatedTime = DateTime.UtcNow,
            UpdatedBy = 1,
            UpdatedTime = DateTime.UtcNow
        };

        await _context.Tbl_UserMasters.AddAsync(entity);

        // Update domain ID after insert (optional)
        user.UserId = entity.UserID;
    }

    public async Task UpdateAsync(UserMaster user)
    {
        var entity = await _context.Tbl_UserMasters
            .FirstOrDefaultAsync(x => x.UserID == user.UserId);

        if (entity == null)
            throw new KeyNotFoundException("User not found");

        entity.LoginName = user.LoginName;
        entity.EmailID = user.Email;
        entity.FirstName = user.FirstName;
        entity.LastName = user.LastName;

        entity.UpdatedBy = 1;
        entity.UpdatedTime = DateTime.UtcNow;

        _context.Tbl_UserMasters.Update(entity);
    }

    public async Task DeleteAsync(UserMaster user)
    {
        var entity = await _context.Tbl_UserMasters
            .FirstOrDefaultAsync(x => x.UserID == user.UserId);

        if (entity == null)
            throw new KeyNotFoundException("User not found");

        // Soft delete (recommended ✅)
        entity.IsDeleted = 1;
        entity.UpdatedTime = DateTime.UtcNow;

        _context.Tbl_UserMasters.Update(entity);
    }
    public async Task UpdatePasswordAsync(string loginId, string passwordHash)
    {
        var entity = await _context.Tbl_UserMasters
            .FirstOrDefaultAsync(x => x.LoginID == loginId && x.IsDeleted == 0);

        if (entity == null)
            throw new KeyNotFoundException("User not found");

        entity.Password = passwordHash;
        entity.UpdatedTime = DateTime.UtcNow;

        _context.Tbl_UserMasters.Update(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}