using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;

namespace SportStore.BusinessLogicLayer.Services
{
    public class UserOrderService : IUserOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CancelOrderAsync(Guid orderId, string userId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId, userId);

            if (order == null)
            {
                throw new KeyNotFoundException("Không tìm thấy đơn hàng hoặc đơn hàng không thuộc về người dùng.");
            }

            if (order.Status == OrderStatus.Delivered)
            {
                throw new InvalidOperationException("Không thể hủy đơn hàng đã được giao.");
            }
            if (order.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Đơn hàng đã bị hủy.");
            }

            order.Status = OrderStatus.Cancelled;

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<OrderVm> GetOrderByIdAsync(Guid orderId, string userId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId, userId);
            if (order == null) return null;

            return new OrderVm
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.OrderDetails.Sum(od => od.Price * od.Quantity),
                Status = order.Status.ToString(),
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailVm
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.Price
                }).ToList()
            };
        }
    }
}