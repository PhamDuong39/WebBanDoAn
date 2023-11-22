namespace WebBanDoAn.ViewModels.ResponseModel
{
    public class LoginResponseModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
