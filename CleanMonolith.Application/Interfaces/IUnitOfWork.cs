using System;
using System.Threading;
using System.Threading.Tasks;
using CleanMonolith.Domain.Entities;

namespace CleanMonolith.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
