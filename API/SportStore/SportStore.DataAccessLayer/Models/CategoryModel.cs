using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SportStore.DataAccessLayer.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportStore.DataAccessLayer.Models
{
    public enum CategoryStatus
    {
        [Display(Name = "Đợi xác nhận")]
        Pending,

        [Display(Name = "Mở")]
        Open,

        [Display(Name = "Đóng")]
        Closed
    }

    public class CategoryModel
    {
        [Key]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        public string CategoryName { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public CategoryStatus Status { get; set; } = CategoryStatus.Pending;

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageFile { get; set; }

        [ValidateNever]
        public ICollection<ProductCategoryModel> ProductCategoryModels { get; set; }
    }
}