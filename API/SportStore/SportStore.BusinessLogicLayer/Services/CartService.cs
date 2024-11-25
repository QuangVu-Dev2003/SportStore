using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.DataAccessLayer.Repositories.IRepository;
using SportStore.DataAccessLayer.Models;

namespace SportStore.BusinessLogicLayer.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CartItemVm>> GetCartItemsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("ID người dùng không được để trống hoặc rỗng.");

            var cart = await _unitOfWork.CartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new InvalidOperationException("Không tìm thấy giỏ hàng của người dùng đã cho.");

            return cart.Items.Select(item => new CartItemVm
            {
                ProductId = item.ProductId,
                ProductName = item.Product.ProductName,
                Quantity = item.Quantity,
                Price = item.Product.Price,
                Total = item.Quantity * item.Product.Price
            }).ToList();
        }

        public async Task AddToCartAsync(string userId, CartItemVm cartItem)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("ID người dùng không được để trống hoặc rỗng.");

            if (cartItem == null)
                throw new ArgumentNullException(nameof(cartItem));

            var cart = await _unitOfWork.CartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new CartModel
                {
                    UserId = userId,
                    Items = new List<CartItemModel>
            {
                new CartItemModel
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity
                }
            }
                };
                await _unitOfWork.CartRepository.AddAsync(cart);
            }
            else
            {
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == cartItem.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity += cartItem.Quantity;
                }
                else
                {
                    cart.Items.Add(new CartItemModel
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string userId, Guid productId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("ID người dùng không được để trống hoặc rỗng.");

            var cart = await _unitOfWork.CartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
                throw new InvalidOperationException("Không tìm thấy giỏ hàng của người dùng đã cho.");

            var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (itemToRemove != null)
            {
                cart.Items.Remove(itemToRemove);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("ID người dùng không được để trống hoặc rỗng.");

            var cart = await _unitOfWork.CartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
                throw new InvalidOperationException("Không tìm thấy giỏ hàng của người dùng đã cho.");

            cart.Items.Clear();
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
