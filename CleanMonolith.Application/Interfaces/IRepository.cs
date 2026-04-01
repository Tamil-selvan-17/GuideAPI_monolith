using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CleanMonolith.Domain.Common;

namespace CleanMonolith.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
    // Pagination projection
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
