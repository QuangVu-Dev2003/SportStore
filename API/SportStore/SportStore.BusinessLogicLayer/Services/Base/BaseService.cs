using SportStore.DataAccessLayer.Repositories.IRepository;

namespace SportStore.BusinessLogicLayer.Services.Base
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _unitOfWork.GenericRepository<T>().GetAllAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.GenericRepository<T>().GetByIdAsync(id);
        }

        public async Task<T> CreateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity cannot be null");

            var result = await _unitOfWork.GenericRepository<T>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task UpdateAsync(T entity)
        {
            _unitOfWork.GenericRepository<T>().Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _unitOfWork.GenericRepository<T>().RemoveAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}