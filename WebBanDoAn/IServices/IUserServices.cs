using WebBanDoAn.ViewModels.CommonModels;
using WebBanDoAn.ViewModels.Users;

namespace WebBanDoAn.IServices
{
    public interface IUserServices
    {
        Task<ResponseModel<string>> UpdateUser(UpdateUserModel updateUserModel);
        Task<ResponseModel<bool>> LockUser(string userName);
        Task<ResponseModel<bool>> UnlockUser(string userName);
    }
}
