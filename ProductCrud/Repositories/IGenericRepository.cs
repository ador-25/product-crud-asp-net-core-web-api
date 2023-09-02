using System.Linq.Expressions;

namespace ProductCrud.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // Get full list
        Task<IEnumerable<T>> GetAllAsync();

        //Get filtered list
        Task<IEnumerable<T>> GetAllByConditionAsync(Expression<Func<T, bool>> predicate);

        //Search for an object with PK either int,string or Guid
        Task<T> GetByIdIntAsync(int id);
        Task<T> GetByIdStringAsync(string username);
        Task<T> GetByIdGuidAsync(Guid id);

        // Find one with condition
        Task<T> FindByConditionAsync(Expression<Func<T, bool>> predicate);

        // Create new instance
        void Add(T entity);
        //Update existing
        void Update(T entity);

        //Delete
        void Delete(T entity);

        // return true on save
        Task<bool> SaveChangesAsync();


    }
}
