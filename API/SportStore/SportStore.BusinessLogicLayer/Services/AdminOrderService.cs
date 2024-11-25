using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;

namespace SportStore.BusinessLogicLayer.Services
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderVm>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();
            return orders.Select(order => new OrderVm
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailVm
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.Price
                }).ToList()
            });
        }

        public async Task<OrderVm> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return null;

            return new OrderVm
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
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

        public async Task UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusVm updateStatusVm)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order với ID: {orderId} không tìm thấy.");

            if (!Enum.TryParse(updateStatusVm.Status, out OrderStatus newStatus))
                throw new InvalidOperationException($"Trạng thái không hợp lệ: {updateStatusVm.Status}");

            order.Status = newStatus;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}