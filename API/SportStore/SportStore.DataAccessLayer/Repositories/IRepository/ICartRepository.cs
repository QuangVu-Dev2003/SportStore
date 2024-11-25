using SportStore.DataAccessLayer.Models;

namespace SportStore.DataAccessLayer.Repositories.IRepository
{
    public interface ICartRepository : IGenericRepository<CartModel>
    {
        Task<CartModel> GetCartByUserIdAsync(string userId);
    }
}