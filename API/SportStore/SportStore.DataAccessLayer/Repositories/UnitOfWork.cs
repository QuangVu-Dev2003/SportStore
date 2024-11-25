using SportStore.DataAccessLayer.Data;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;

namespace SportStore.DataAccessLayer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SportStoreDbContext _context;
        private bool _disposed;

        private IGenericRepository<ProductModel> _productRepository;
        private IGenericRepository<CategoryModel> _categoryRepository;
        private IGenericRepository<BrandModel> _brandRepository;
        private IGenericRepository<AppUser> _userRepository;
        private ICartRepository _cartRepository;
        private IOrderRepository _orderRepository;

        public UnitOfWork(SportStoreDbContext context)
        {
            _context = context;
        }

        public SportStoreDbContext Context => _context;

        public IGenericRepository<ProductModel> ProductRepository
        {
            get => _productRepository ??= new GenericRepository<ProductModel>(_context);
        }

        public IGenericRepository<CategoryModel> CategoryRepository
        {
            get => _categoryRepository ??= new GenericRepository<CategoryModel>(_context);
        }

        public IGenericRepository<BrandModel> BrandRepository
        {
            get => _brandRepository ??= new GenericRepository<BrandModel>(_context);
        }

        public IGenericRepository<AppUser> UserRepository
        {
            get => _userRepository ??= new GenericRepository<AppUser>(_context);
        }

        public IOrderRepository OrderRepository
        {
            get => _orderRepository ??= new OrderRepository(_context);
        }

        public ICartRepository CartRepository => new CartRepository(_context);

        public IGenericRepository<T> GenericRepository<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}