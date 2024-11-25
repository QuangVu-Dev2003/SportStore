using Microsoft.EntityFrameworkCore;
using SportStore.DataAccessLayer.Data;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;

namespace SportStore.DataAccessLayer.Repositories
{
    public class CartRepository : GenericRepository<CartModel>, ICartRepository
    {
        private readonly SportStoreDbContext _context;

        public CartRepository(SportStoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CartModel> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}