using Microsoft.EntityFrameworkCore;
using ProductCrud.Contexts;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ProductCrud.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _entities;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public void Add(T entity)
        {
            _entities.Add(entity);
        }

        public void Delete(T entity)
        {
            _entities.Remove(entity);
        }

        public async Task<T> FindByConditionAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }
        //O(n)
        public async Task<IEnumerable<T>> GetAllByConditionAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities
                .Where(predicate)
                .ToListAsync();
        }
        // O(1)
        public async Task<T> GetByIdGuidAsync(Guid id)
        {
            //try entities[id]
            return await _entities.FindAsync(id);
        }


        // O(1)
        public async Task<T> GetByIdIntAsync(int id)
        {
            return await _entities.FindAsync(id);
        }
        // O(1)
        public async Task<T> GetByIdStringAsync(string username)
        {
            return await _entities.FindAsync(username);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        public void Update(T entity)
        {
            _entities.Update(entity);
        }
    }
}
