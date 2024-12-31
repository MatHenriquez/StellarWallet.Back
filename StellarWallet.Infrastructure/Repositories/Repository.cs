using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StellarWallet.Domain.Interfaces.Persistence;

namespace StellarWallet.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DatabaseContext _context;
    internal DbSet<T> dbSet;

    public Repository(DatabaseContext context)
    {
        _context = context;
        dbSet = _context.Set<T>();
    }

    public async Task Add(T entity)
    {
        await dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public void Delete(T entity)
    {
        dbSet.Remove(entity);
    }

    public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null, bool istracking = true)
    {
        IQueryable<T> query = dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (!istracking)
        {
            query = query.AsNoTracking();
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetById(int id)
    {
        return await dbSet.FindAsync(id);
    }

    public void Update(T entity)
    {
        dbSet.Update(entity);
        _context.SaveChangesAsync();
    }
}
