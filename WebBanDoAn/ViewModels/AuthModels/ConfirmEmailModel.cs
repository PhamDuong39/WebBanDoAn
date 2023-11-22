using System.ComponentModel.DataAnnotations;

namespace WebBanDoAn.ViewModels.AuthModels
{
    public class ConfirmEmailModel
    {
        [Required] public string UserNameOrEmail { get; set; }

        [Required] public string Token { get; set; }
    }
}
