using System.ComponentModel.DataAnnotations;

namespace SportStore.WebApi.ViewModels
{
    public class ResetPasswordVm
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Token là bắt buộc")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password là bắt buộc")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}