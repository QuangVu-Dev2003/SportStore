using SportStore.BusinessLogicLayer.ViewModels;

namespace SportStore.BusinessLogicLayer.Services.IService
{
    public interface IAdminOrderService
    {
        Task<IEnumerable<OrderVm>> GetAllOrdersAsync();
        Task<OrderVm> GetOrderByIdAsync(Guid orderId);
        Task UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusVm updateStatusVm);
    }
}