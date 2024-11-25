using SportStore.DataAccessLayer.Models;

namespace SportStore.DataAccessLayer.Repositories.IRepository
{
    public interface IOrderRepository : IGenericRepository<OrderModel>
    {
        Task<IEnumerable<OrderModel>> GetAllOrdersAsync();
        Task<OrderModel> GetOrderByIdAsync(Guid orderId);//ADmin
        Task<OrderModel> GetOrderByIdAsync(Guid orderId, string userId);
        Task<OrderModel> AddOrderAsync(OrderModel order);
    }
}