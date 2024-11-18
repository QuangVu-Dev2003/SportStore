using Microsoft.EntityFrameworkCore;
using SportStore.DataAccessLayer.Data;
using SportStore.DataAccessLayer.Repositories.IRepository;
using System.Linq.Expressions;

namespace SportStore.DataAccessLayer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SportStoreDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(SportStoreDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetDbSet()
        {
            return _dbSet;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            var result = await _dbSet.AddAsync(entity);
            return result.Entity;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
    }
}