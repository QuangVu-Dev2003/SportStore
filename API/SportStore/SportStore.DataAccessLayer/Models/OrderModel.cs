using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportStore.DataAccessLayer.Models
{
    public class OrderModel
    {
        [Key]
        public Guid OrderId { get; set; }

        public Guid OrderCode { get; set; }

        public string Username { get; set; }

        public DateTime CreateDate { get; set; }

        public int Status { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        public ICollection<OrderDetailModel> OrderDetails { get; set; }
    }
}