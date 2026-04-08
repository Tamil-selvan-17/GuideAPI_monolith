using CleanMonolith.Application.Interfaces;
using CleanMonolith.Domain.Entities;
using CleanMonolith.Infrastructure.Entity;
using CleanMonolith.Infrastructure.Mappers;
using CleanMonolith.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly nEdit_DEVContext _context;

    public RefreshTokenRepository(nEdit_DEVContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(int userId)
    {
        var entities = await _context.Tbl_RefreshTokens
            .Where(x => x.UserId == userId && !x.IsUsed && !x.IsRevoked)
            .ToListAsync();

        return entities.Select(RefreshTokenMapper.ToDomain);
    }

    public async Task AddAsync(RefreshToken token)
    {
        var entity = RefreshTokenMapper.ToEntity(token);
        await _context.Tbl_RefreshTokens.AddAsync(entity);
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        var entity = await _context.Tbl_RefreshTokens
            .FirstOrDefaultAsync(x => x.Id == token.Id);

        if (entity == null)
            throw new KeyNotFoundException("Refresh token not found");

        entity.IsUsed = token.IsUsed;
        entity.IsRevoked = token.IsRevoked;
        entity.ExpiryDate = token.ExpiryDate;

        _context.Tbl_RefreshTokens.Update(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}