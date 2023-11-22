using Microsoft.AspNetCore.Identity;
using WebBanDoAn.Context;
using WebBanDoAn.Entities;
using WebBanDoAn.Enums.User;
using WebBanDoAn.IServices;
using WebBanDoAn.ViewModels.CommonModels;
using WebBanDoAn.ViewModels.Users;

namespace WebBanDoAn.Services
{
    public class UserServices : IUserServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserServices(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseModel<bool>> LockUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new ResponseModel<bool>()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Không tìm thấy thông tin người dùng"
                };
            }
            user.Status = Convert.ToInt32(UserStatusEnum.INACTIVE);
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ResponseModel<bool>()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Cập nhật thông tin thất bại"
                };
            }
            return new ResponseModel<bool>()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = $"Thông tin người dùng {user.UserName} được cập nhật thành công"
            };
        }

        public async Task<ResponseModel<bool>> UnlockUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new ResponseModel<bool>()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Không tìm thấy thông tin người dùng"
                };
            }
            user.Status = Convert.ToInt32(UserStatusEnum.ACTIVE);
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ResponseModel<bool>()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Cập nhật thông tin thất bại"
                };
            }
            return new ResponseModel<bool>()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = $"Thông tin người dùng {user.UserName} được cập nhật thành công"
            };
        }

        public async Task<ResponseModel<string>> UpdateUser(UpdateUserModel updateUserModel)
        {
            var user = await _userManager.FindByNameAsync(updateUserModel.Username);
            if(user == null)
            {
                return new ResponseModel<string>()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Không tìm thấy thông tin người dùng"
                };
            }

            user.FullName = updateUserModel.Fullname;
            user.Address = updateUserModel.Address;
            user.PhoneNumber = updateUserModel.PhoneNumber;
            user.Avatar = updateUserModel.Avatar;

            var result = await _userManager.UpdateAsync(user);

            if(!result.Succeeded)
            {
                return new ResponseModel<string>()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Cập nhật thông tin thất bại"
                };
            }
            return new ResponseModel<string>()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = $"Thông tin người dùng {user.UserName} được cập nhật thành công"
            };
        }
    }
}
