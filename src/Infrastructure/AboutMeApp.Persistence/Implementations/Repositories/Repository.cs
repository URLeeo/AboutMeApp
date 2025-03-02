using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Domain.Entities.Common;
using AboutMeApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AboutMeApp.Persistence.Implementations.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AboutMeAppDbContext _context;
    public Repository(AboutMeAppDbContext context)
    {
        _context = context;
    }

    public DbSet<T> Table => _context.Set<T>();

    public async Task<T> AddAsync(T entity)
    {
        await Table.AddAsync(entity);
        return entity;
    }

    public void Delete(T entity)
    {
        Table.Remove(entity);
    }

    public IQueryable<T> GetAll()
    {
        return Table.AsNoTracking();
    }

    public IQueryable<T> GetAll(
        Expression<Func<T, bool>> expression,
        Expression<Func<T, object>>? orderBy = null,
        bool isAscending = true,
        bool isTracking = false,
        params string[] includes)
    {
        IQueryable<T> query = Table.Where(expression);
        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        if (!isTracking)
        {
            query = query.AsNoTracking();
        }
        if (orderBy is not null)
        {
            query = isAscending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }
        return query;
    }

    public async Task<T?> GetByFilter(
        Expression<Func<T, bool>> expression,
        bool isTracking = false,
        params string[] includes)
    {
        IQueryable<T> query = Table.Where(expression);
        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        if (!isTracking)
        {
            query = query.AsNoTracking();
        }
        return await query.FirstOrDefaultAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Table.FindAsync(id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public T Update(T entity)
    {
        Table.Update(entity);
        return entity;
    }
}
