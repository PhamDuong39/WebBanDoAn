using System.ComponentModel.DataAnnotations;

namespace WebBanDoAn.ViewModels.AuthModels
{
    public class ResetPasswordModel
    {
        [Required]
        [EmailAddress] 
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ConfirmPassword { get; set; } = null!;

        public string Token { get; set; } = null!;
    }
}
