using System.ComponentModel.DataAnnotations;

namespace SportStore.WebApi.ViewModels
{
    public class LoginVm
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }
    }
}