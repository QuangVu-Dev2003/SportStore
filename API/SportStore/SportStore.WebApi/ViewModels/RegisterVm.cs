using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportStore.WebApi.ViewModels
{
    public class RegisterVm
    {
        [Required(ErrorMessage = "FirstName là bắt buộc!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName là bắt buộc!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc!")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password là bắt buộc!")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [NotMapped]
        public string ConfirmPassword { get; set; }
    }
}