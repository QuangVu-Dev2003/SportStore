using SportStore.BusinessLogicLayer.ViewModels;

namespace SportStore.BusinessLogicLayer.Services.IService
{
    public interface ICartService
    {
        Task<List<CartItemVm>> GetCartItemsAsync(string userId);
        Task AddToCartAsync(string userId, CartItemVm cartItem);
        Task RemoveFromCartAsync(string userId, Guid productId);
        Task ClearCartAsync(string userId);
    }
}