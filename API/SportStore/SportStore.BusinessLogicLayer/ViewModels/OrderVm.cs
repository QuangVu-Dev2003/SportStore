namespace SportStore.BusinessLogicLayer.ViewModels
{
    public class OrderVm
    {
        public Guid OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } 
        public List<OrderDetailVm> OrderDetails { get; set; }
    }

    public class OrderDetailVm
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}