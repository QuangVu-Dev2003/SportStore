using Microsoft.AspNetCore.Http;
using SportStore.DataAccessLayer.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportStore.DataAccessLayer.Models
{
    public enum BrandStatus
    {
        [Display(Name = "Đợi xác nhận")]
        Pending,

        [Display(Name = "Mở")]
        Open,

        [Display(Name = "Đóng")]
        Closed
    }

    public class BrandModel
    {
        [Key]
        public Guid BrandId { get; set; }

        [Required(ErrorMessage = "Tên thương hiệu là bắt buộc.")]
        public string BrandName { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public BrandStatus Status { get; set; } = BrandStatus.Pending;

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageFile { get; set; }

        public ICollection<ProductModel> Products { get; set; }
    }
}