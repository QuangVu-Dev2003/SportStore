using Microsoft.AspNetCore.Http;
using SportStore.DataAccessLayer.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportStore.BusinessLogicLayer.ViewModels
{
    public enum CategoryStatus
    {
        [Display(Name = "Đợi xác nhận")]
        Pending = 0,
        [Display(Name = "Mở")]
        Open = 1,
        [Display(Name = "Đóng")]
        Closed = 2
    }

    public class CategoryVm
    {
        [Key]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        public string CategoryName { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public string Status { get; set; } = "Đợi xác nhận";

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageFile { get; set; }
    }
}