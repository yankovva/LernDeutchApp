using System.Linq.Expressions;

namespace LerningApp.Data.Repository.Interfaces;

public interface IRepository<TType, TId> 
{
    TType GetById(TId id);
    Task<TType> GetByIdAsync(TId id);

    IEnumerable<TType> GetAll();
    Task<IEnumerable<TType>> GetAllAsync();
    IQueryable<TType> GetAllAttached();

    TType FirstorDefault(Func<TType, bool> predicate);
    Task<TType> FirstorDefaultAsync(Expression<Func<TType, bool>> predicate);

    void Add(TType item);
    Task AddAsync(TType item);

    Task SaveChangesAsync();

    bool Delete(TId id);
    Task<bool> DeleteAsync(TId id);
    Task DeleteAsync(TType entity);

    bool Update(TType item);
    Task<bool> UpdateAsync(TType item);
}