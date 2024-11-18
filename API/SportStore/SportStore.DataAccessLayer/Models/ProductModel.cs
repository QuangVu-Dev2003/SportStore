using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportStore.DataAccessLayer.Models
{
    public enum ProductStatus
    {
        [Display(Name = "Còn Hàng")]
        InStock,

        [Display(Name = "Đang Nhập Hàng")]
        Restocking,

        [Display(Name = "Hết Hàng")]
        OutStock,

        [Display(Name = "Ngừng Bán")]
        Discontinued
    }

    public class ProductModel
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả sản phẩm.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả chi tiết sản phẩm.")]
        public string Detail { get; set; }

        public List<string>? Images { get; set; } = new List<string>();

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm cần lớn hơn 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng sản phẩm trong kho.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải lớn hơn 0.")]
        public int Instock { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Restocking;

        [Required(ErrorMessage = "Vui lòng nhập ngày thêm sản phẩm.")]
        public DateTime AddDate { get; set; } = DateTime.Now;

        public ICollection<ProductCategoryModel> ProductCategoryModels { get; set; }

        public Guid BrandId { get; set; }

        [ForeignKey("BrandId")]
        public BrandModel Brand { get; set; }
    }
}