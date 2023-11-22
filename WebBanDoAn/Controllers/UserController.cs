using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using WebBanDoAn.IServices;
using WebBanDoAn.ViewModels.Users;

namespace WebBanDoAn.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateInforUser(UpdateUserModel updateUserModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _userServices.UpdateUser(updateUserModel);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("lock-user")]
        public async Task<IActionResult> LockAccountUser(string userName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _userServices.LockUser(userName);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("unlock-user")]
        public async Task<IActionResult> UnlockAccountUser(string userName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _userServices.UnlockUser(userName);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
