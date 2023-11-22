using System.ComponentModel.DataAnnotations;

namespace WebBanDoAn.ViewModels.AuthModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
