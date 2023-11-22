using System.ComponentModel.DataAnnotations;

namespace WebBanDoAn.ViewModels.Users
{
    public class UpdateUserModel
    {
        [Required]
        public string Username { get; set; }
        public string? Fullname { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
    }
}
