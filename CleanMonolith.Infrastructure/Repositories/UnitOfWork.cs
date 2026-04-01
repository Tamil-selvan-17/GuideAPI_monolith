using System.Threading;
using System.Threading.Tasks;
using CleanMonolith.Application.Interfaces;
using CleanMonolith.Domain.Entities;
using CleanMonolith.Infrastructure.Persistence;

namespace CleanMonolith.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IRepository<User>? _users;
    private IRepository<RefreshToken>? _refreshTokens;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => _users ??= new GenericRepository<User>(_context);
    public IRepository<RefreshToken> RefreshTokens => _refreshTokens ??= new GenericRepository<RefreshToken>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
