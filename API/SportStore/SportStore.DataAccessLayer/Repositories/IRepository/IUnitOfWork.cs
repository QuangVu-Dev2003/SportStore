using SportStore.DataAccessLayer.Data;
using SportStore.DataAccessLayer.Models;

namespace SportStore.DataAccessLayer.Repositories.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        SportStoreDbContext Context { get; }
        IGenericRepository<ProductModel> ProductRepository { get; }
        IGenericRepository<CategoryModel> CategoryRepository { get; }
        IGenericRepository<BrandModel> BrandRepository { get; }
        IGenericRepository<AppUser> UserRepository { get; }
        IGenericRepository<T> GenericRepository<T>() where T : class;

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}