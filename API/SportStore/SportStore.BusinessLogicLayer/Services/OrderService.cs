using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;

namespace SportStore.BusinessLogicLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<OrderService> _logger;
        private readonly ICartService _cartService;

        public OrderService(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<AppUser> userManager, ILogger<OrderService> logger, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
            _logger = logger;
            _cartService = cartService;
        }

        public async Task<OrderModel> CheckoutAsync(OrderVm orderVm, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy người dùng");
                }

                var orderDetails = orderVm.OrderDetails.Select(od => new OrderDetailModel
                {
                    ProductId = od.ProductId,
                    Price = od.UnitPrice,
                    Quantity = od.Quantity
                }).ToList();

                var order = new OrderModel
                {
                    UserId = userId,
                    OrderDetails = orderDetails, 
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending, 
                    ShippingAddress = orderVm.ShippingAddress,
                    TotalAmount = orderDetails.Sum(od => od.Price * od.Quantity)
                };

                await _unitOfWork.OrderRepository.AddAsync(order);

                var subject = "Xác nhận đơn hàng";
                var body = $"Đơn hàng của bạn đã được đặt thành công và đang chờ xem xét. <a href='http://localhost:4200/orders/{order.OrderId}'>Xem đơn hàng của bạn tại đây</a>";
                await _emailSender.SendEmailAsync(user.Email, subject, body);
               
                await _cartService.ClearCartAsync(userId);
                await _unitOfWork.SaveChangesAsync();

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi đặt hàng.");
                throw new Exception("Đã xảy ra lỗi khi đặt hàng.");
            }
        }

        public async Task<OrderModel> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            return order;
        }
        public async Task<OrderModel> GetOrderByIdAsync(Guid orderId, string userId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId, userId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            return order;
        }

        public async Task<IEnumerable<OrderModel>> GetAllOrdersAsync()
        {
            return await _unitOfWork.OrderRepository.GetAllOrdersAsync();
        }

        public async Task<bool> CancelOrderAsync(Guid orderId, string userId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId, userId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            if (order.Status == OrderStatus.Pending)
            {
                var timeElapsed = DateTime.UtcNow - order.OrderDate;
                if (timeElapsed.TotalDays <= 1) // Cho phép hủy trong vòng 1 ngày
                {
                    order.Status = OrderStatus.Cancelled;
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
                else
                {
                    throw new Exception("Đơn hàng không thể hủy sau 1 ngày.");
                }
            }

            return false;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, int status)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            order.Status = (OrderStatus)status;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // Kiểm tra xem đơn hàng có thể hủy được không
        public async Task<bool> CanCancelOrderAsync(Guid orderId, string userId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId, userId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            var timeElapsed = DateTime.UtcNow - order.OrderDate;
            return order.Status == OrderStatus.Pending && timeElapsed.TotalDays <= 1;
        }
    }
}