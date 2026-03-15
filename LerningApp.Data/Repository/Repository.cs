using System.Linq.Expressions;
using LerningApp.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Repository;

public class Repository<TType, TId>
    : IRepository<TType, TId>  where TType : class
{
    private readonly LerningAppContext dbContext;
    private readonly DbSet<TType> dbSet;
    
    public Repository(LerningAppContext dbContext)
    {
        this.dbContext = dbContext;
        this.dbSet = this.dbContext.Set<TType>();

    }
    public TType? GetById(TId id)
    {
        TType? entity = this.dbSet
            .Find(id);

        return entity;
    }

    public async Task<TType?> GetByIdAsync(TId id)
    {
        TType? entity = await this.dbSet
            .FindAsync(id);

        return entity;
    }

    public IEnumerable<TType> GetAll()
    {
        return dbSet.ToArray();
    }

    public async Task<IEnumerable<TType>> GetAllAsync()
    {
        return await dbSet.ToArrayAsync();
    }

    public IQueryable<TType> GetAllAttached()
    {
        return dbSet.AsQueryable();
    }
    
    public TType? FirstorDefault(Func<TType, bool> predicate)
    {
        TType? entity = this.dbSet
            .FirstOrDefault(predicate)!;

        return entity;
    }
    public async Task<TType?> FirstorDefaultAsync(Expression<Func<TType, bool>> predicate)
    {
        TType? entity = await this.dbSet
            .FirstOrDefaultAsync(predicate);

        return entity;
    }

    public void Add(TType item)
    {
        dbSet.Add(item);
    }
    
    public void AddRange(IEnumerable<TType> items)
    {
        dbSet.AddRange(items);
    }

    public bool DeleteById(TId id)
    {
        TType? entity = GetById(id);
        if (entity == null)
        {
            return false;
        }
        dbSet.Remove(entity);

        return true;
    }
   
    public void DeleteByEntity(TType entity)
    {
        dbSet.Remove(entity);
    }

    public bool Update(TType item)
    {
        dbSet.Update(item);
        return true;
    }
    
    public async Task SaveChangesAsync()
    {
        await this.dbContext.SaveChangesAsync();
    }
  
}