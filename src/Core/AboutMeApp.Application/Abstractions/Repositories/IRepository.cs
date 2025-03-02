using AboutMeApp.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AboutMeApp.Application.Abstractions.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    DbSet<T> Table { get; }
    Task<T?> GetByIdAsync(Guid id);
    IQueryable<T> GetAll();
    IQueryable<T> GetAll(Expression<Func<T, bool>> expression, Expression<Func<T, object>>? orderBy = null, bool isAscending = true, bool isTracking = false, params string[] includes);
    Task<T?> GetByFilter(Expression<Func<T, bool>> expression, bool isTracking = false, params string[] includes);
    Task<T> AddAsync(T entity);
    T? Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}
