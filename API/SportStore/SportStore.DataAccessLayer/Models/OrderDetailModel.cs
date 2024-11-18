using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportStore.DataAccessLayer.Models
{
    public class OrderDetailModel
    {
        [Key]
        public Guid ODId { get; set; }

        public Guid OrderId { get; set; }

        public string Username { get; set; }

        public Guid OrderCode { get; set; }

        public Guid ProductId { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("OrderId")]
        public OrderModel Order { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}