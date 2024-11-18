using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SportStore.DataAccessLayer.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName)?.ToLower();
                string[] extensions = { ".png", ".jpg", ".jpeg", ".webp" };

                if (string.IsNullOrEmpty(extension) || !extensions.Contains(extension))
                {
                    return new ValidationResult("Các file được phép là \"png\", \"jpg\", \"jpeg\".");
                }
            }
            return ValidationResult.Success;
        }
    }
}