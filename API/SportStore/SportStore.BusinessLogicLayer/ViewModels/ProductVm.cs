using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SportStore.BusinessLogicLayer.ViewModels
{
    public enum ProductViewStatus
    {
        InStock = 0, // "Còn Hàng"
        Restocking = 1, // "Đang Nhập Hàng"
        OutStock = 2, // "Hết Hàng"
        Discontinued = 3 // "Ngừng Bán"
    }

    public class ProductVm
    {
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả sản phẩm.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chi tiết sản phẩm.")]
        public string Detail { get; set; }

        public List<string>? Images { get; set; } = new List<string>();

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm cần lớn hơn 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng sản phẩm.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải lớn hơn 0.")]
        public int Instock { get; set; }

        public ProductViewStatus Status { get; set; } = ProductViewStatus.Restocking;

        [Required(ErrorMessage = "Vui lòng nhập ngày thêm sản phẩm.")]
        public DateTime AddDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng chọn thương hiệu.")]
        public string BrandName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public List<string> CategoryNames { get; set; } = new List<string>(); 

        public IEnumerable<IFormFile>? ImageFiles { get; set; } 
    }
}