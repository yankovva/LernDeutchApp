using System.Linq.Expressions;

namespace LerningApp.Data.Repository.Interfaces;

public interface IRepository<TType, TId> 
{
    //TODO add soft delete
    TType? GetById(TId id);
    Task<TType?> GetByIdAsync(TId id);

    IEnumerable<TType> GetAll();
    Task<IEnumerable<TType>> GetAllAsync();
    IQueryable<TType> GetAllAttached();

    TType? FirstorDefault(Func<TType, bool> predicate);
    Task<TType?> FirstorDefaultAsync(Expression<Func<TType, bool>> predicate);

    void Add(TType item);
   void AddRange(IEnumerable<TType> items);

    Task SaveChangesAsync();

    bool DeleteById(TId id);
    void DeleteByEntity(TType entity);

    bool Update(TType item);
   
}