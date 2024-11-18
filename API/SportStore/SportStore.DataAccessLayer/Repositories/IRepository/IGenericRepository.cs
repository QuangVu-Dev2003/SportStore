using System.Linq.Expressions;

namespace SportStore.DataAccessLayer.Repositories.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetDbSet();
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        Task RemoveAsync(Guid id);
    }
}