using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SportStore.DataAccessLayer.Models
{
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }

    public class OrderModel
    {
        [Key]
        public Guid OrderId { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public AppUser User { get; set; }

        public DateTime OrderDate { get; set; }

        public string ShippingAddress { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public ICollection<OrderDetailModel> OrderDetails { get; set; } = new List<OrderDetailModel>();
    }
}