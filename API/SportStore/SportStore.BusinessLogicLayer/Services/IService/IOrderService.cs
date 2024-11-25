using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;

namespace SportStore.BusinessLogicLayer.Services.IService
{
    public interface IOrderService
    {
        Task<OrderModel> CheckoutAsync(OrderVm orderVm, string userId);

        // admin
        Task<OrderModel> GetOrderByIdAsync(Guid orderId);

        // user
        Task<OrderModel> GetOrderByIdAsync(Guid orderId, string userId);

        Task<IEnumerable<OrderModel>> GetAllOrdersAsync();

        // user | admin
        Task<bool> CancelOrderAsync(Guid orderId, string userId);

        // Cập nhật trạng thái của đơn hàng (admin)
        Task<bool> UpdateOrderStatusAsync(Guid orderId, int status);

        // Kiểm tra xem đơn hàng có thể hủy được không
        Task<bool> CanCancelOrderAsync(Guid orderId, string userId);
    }
}