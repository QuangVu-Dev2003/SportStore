using Microsoft.AspNetCore.Http;
using SportStore.DataAccessLayer.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportStore.BusinessLogicLayer.ViewModels
{
    public enum BrandStatus
    {
        [Display(Name = "Đợi xác nhận")]
        Pending = 0,
        [Display(Name = "Mở")]
        Open = 1,
        [Display(Name = "Đóng")]
        Closed = 2
    }

    public class BrandVm
    {
        [Key]
        public Guid BrandId { get; set; }

        [Required(ErrorMessage = "Tên thương hiệu là bắt buộc.")]
        public string BrandName { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public string Status { get; set; } = "Đợi xác nhận";

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageFile { get; set; }
    }
}