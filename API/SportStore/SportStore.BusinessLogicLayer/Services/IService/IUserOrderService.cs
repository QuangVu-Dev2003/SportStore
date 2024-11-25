using SportStore.BusinessLogicLayer.ViewModels;

namespace SportStore.BusinessLogicLayer.Services.IService
{
    public interface IUserOrderService
    {
        Task<OrderVm> GetOrderByIdAsync(Guid orderId, string userId);
        Task CancelOrderAsync(Guid orderId, string userId);
    }
}