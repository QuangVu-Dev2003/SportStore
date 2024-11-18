using SportStore.DataAccessLayer.Data;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;

namespace SportStore.DataAccessLayer.Repositories
{
    public class CategoryRepository : GenericRepository<CategoryModel>, ICategoryRepository
    {
        public CategoryRepository(SportStoreDbContext context) : base(context) { }
    }
}