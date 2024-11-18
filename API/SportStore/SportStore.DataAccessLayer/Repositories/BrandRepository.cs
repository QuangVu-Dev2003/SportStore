using SportStore.DataAccessLayer.Data;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;

namespace SportStore.DataAccessLayer.Repositories
{
    public class BrandRepository : GenericRepository<BrandModel>, IBrandRepository
    {
        public BrandRepository(SportStoreDbContext context) : base(context) { }
    }
}