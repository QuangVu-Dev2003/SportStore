using System.ComponentModel.DataAnnotations;

namespace SportStore.WebApi.ViewModels
{
    public class ForgotPasswordVm
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public string Email { get; set; }
    }
}