namespace SportStore.DataAccessLayer.Models
{
    public class CartItemModel
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public virtual CartModel Cart { get; set; }
        public Guid ProductId { get; set; }
        public virtual ProductModel Product { get; set; }
        public int Quantity { get; set; }
    }
}