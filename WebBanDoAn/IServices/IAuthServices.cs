using System.Security.Claims;
using WebBanDoAn.Entities;
using WebBanDoAn.ViewModels.AuthModels;
using WebBanDoAn.ViewModels.CommonModels;
using WebBanDoAn.ViewModels.ResponseModel;

namespace WebBanDoAn.IServices
{
    public interface IAuthServices
    {
        Task<ResponseModel<LoginResponseModel>> Login(LoginModel loginModel);
        Task<ResponseModel<string>> Logout();
      


        Task<ResponseModel<ResetPasswordModel>> ForgotPassword(string email);
        Task<ResponseModel<ResetPasswordModel>> ChangePassword(ResetPasswordModel resetPasswordModel);

        

        Task<ResponseModel<ApplicationUser>> Register(RegisterModel registerModel);
        Task<ResponseModel<ConfirmEmailModel>> ConfirmEmail(ConfirmEmailModel confirmEmailModel);
        
    }
}
