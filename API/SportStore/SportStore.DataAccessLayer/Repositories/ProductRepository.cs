using SportStore.DataAccessLayer.Data;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;

namespace SportStore.DataAccessLayer.Repositories
{
    public class ProductRepository :GenericRepository<ProductModel>,IProductRepository
    {
        public ProductRepository(SportStoreDbContext context):base(context) { }
    }
}