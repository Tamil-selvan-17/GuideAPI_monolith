using CleanMonolith.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanMonolith.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(int userId);
        Task AddAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token);
        Task SaveChangesAsync();
    }
}
