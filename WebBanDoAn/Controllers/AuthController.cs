using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Operators;
using WebBanDoAn.IServices;
using WebBanDoAn.ViewModels.AuthModels;
using WebBanDoAn.ViewModels.CommonModels;
using WebBanDoAn.ViewModels.ResponseModel;

namespace WebBanDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly IEmailServices _emailServices;

        public AuthController(IAuthServices authServices, IEmailServices emailServices)
        {
            _authServices = authServices;
            _emailServices = emailServices;
        }

        #region Login + Logout

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.Login(loginModel);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await HttpContext.SignOutAsync();

            var result = await _authServices.Logout();

            return result.Success ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region Register + ConfirmEmail

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.Register(registerModel);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]ConfirmEmailModel confirmEmailModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authServices.ConfirmEmail(confirmEmailModel);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region ForgotPassword + changePassword

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPass(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.ForgotPassword(email);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePass([FromBody]ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.ChangePassword(resetPasswordModel);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        #endregion
    }
}
