namespace SportStore.DataAccessLayer.Models
{
    public class CartModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
        public virtual ICollection<CartItemModel> Items { get; set; } = new List<CartItemModel>();
    }
}