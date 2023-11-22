using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using WebBanDoAn.Entities;
using Microsoft.AspNetCore.Routing;
using WebBanDoAn.IServices;
using WebBanDoAn.ViewModels.AuthModels;
using WebBanDoAn.ViewModels.CommonModels;
using WebBanDoAn.ViewModels.ResponseModel;
using Microsoft.AspNetCore.Http.Extensions;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Crypto.Operators;

namespace WebBanDoAn.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IEmailServices _emailService;
        
        public AuthServices(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager,
                            IConfiguration configuration, IWebHostEnvironment environment, IEmailServices emailServices   
                          )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _environment = environment;
            _emailService = emailServices;
         
        }

        #region Login + Logout

        public async Task<ResponseModel<LoginResponseModel>> Login(LoginModel loginModel)
        {
            //check input xem có phải email hay không
            var isEmail = Regex.IsMatch(loginModel.UsernameOrEmail, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

            // Attempt to find the user by username or email
            var user = isEmail
                ? await _userManager.FindByEmailAsync(loginModel.UsernameOrEmail)
                : await _userManager.FindByNameAsync(loginModel.UsernameOrEmail);

            // checking password
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password) )
            {
                if(user is { EmailConfirmed: false})
                {
                    return new ResponseModel<LoginResponseModel>
                    {
                        Success = false,
                        Message = "Email chưa được xác nhận!",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var userRoles = await _userManager.GetRolesAsync(user);

                // claims list creation
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                // add role to the claim list
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                // generate token with all claim
                var token = GenerateAccessToken(authClaims);
                var refreshToken = GenerateRefreshToken();
                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                await _userManager.UpdateAsync(user);
                return new ResponseModel<LoginResponseModel>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = new LoginResponseModel()
                    {
                        AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                        RefreshToken = refreshToken,
                        Expiration = token.ValidTo,
                    }
                };
            }
            return new ResponseModel<LoginResponseModel>
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Thất bại!"
            };
        }

        public async Task<ResponseModel<string>> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return new ResponseModel<string>()
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Đăng xuất thành công"
                };
            }
            catch (Exception)
            {
                return new ResponseModel<string>()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Đăng xuất thất bại"
                };
            }
        }

        #endregion

        #region Register + Confirm Email

        public async Task<ResponseModel<ApplicationUser>> Register(RegisterModel registerModel)
        {
            // check username tồn tại
            var userExists = await _userManager.FindByNameAsync(registerModel.Username);
            if (userExists != null)
            {
                return new ResponseModel<ApplicationUser>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Username đã tồn tại!"
                };
            }

            // check email tồn tại
            var checkEmail = await _userManager.FindByEmailAsync(registerModel.Email);
            if (checkEmail != null)
            {
                return new ResponseModel<ApplicationUser>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Email đã được đăng ký!"
                };
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerModel.Username,
                Address = registerModel.Address,
                FullName = registerModel.Fullname,
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now
            };
            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                return new ResponseModel<ApplicationUser>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Đăng ký thất bại do Server!"
                };
            }
            // Thêm role cho user (AspNetUserRoles)
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var adminRole = new ApplicationRole { Name = "Admin", AuthorityName = "Admin", CreatedAt = DateTime.Now };
                await _roleManager.CreateAsync(adminRole);
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var userRole = new ApplicationRole { Name = "User", AuthorityName = "User", CreatedAt = DateTime.Now };
                await _roleManager.CreateAsync(userRole);
            }

            if (!await _roleManager.RoleExistsAsync("Employee"))
            {
                var employeeRole = new ApplicationRole { Name = "Employee", AuthorityName = "Employee", CreatedAt = DateTime.Now };
                await _roleManager.CreateAsync(employeeRole);
            }

            if (await _roleManager.RoleExistsAsync("User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            // Thêm token cho Xác thực Email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenEncoded = Uri.EscapeDataString(token);
            var confirmationLink = $"https://localhost:7046/api/auth/confirm-email?UserNameOrEmail={user.Email}&Token={tokenEncoded}";
            var htmlContent = $@"
                <html>
                <body>
                    <p>Xin chào,</p>
                    <p>Vui lòng bấm vào nút bên dưới để xác nhận Email!</p>
                    <a href='{confirmationLink}'>Xác nhận email</a>
                </body>
                </html>";

            var message = new EmailMessageSetUpModel(new string[] { user.Email! }, "Xác nhận email", htmlContent);
            _emailService.SendEmail(message);

            return new ResponseModel<ApplicationUser>
            {
                Success = true,
                StatusCode = StatusCodes.Status201Created,
                Message = $"Đăng ký thành công và Email xác nhận đã được gửi đến {user.Email}!",
                Data = user
            };
        }

       

        // Sau khi email dược gửi người dùng click vào link thì sẽ chạy hàm này
        public async Task<ResponseModel<ConfirmEmailModel>> ConfirmEmail(ConfirmEmailModel confirmEmailModel)
        {
            //check input xem có phải email hay không
            var isEmail = Regex.IsMatch(confirmEmailModel.UserNameOrEmail, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

            // Attempt to find the user by username or email
            var user = isEmail
                ? await _userManager.FindByEmailAsync(confirmEmailModel.UserNameOrEmail)
                : await _userManager.FindByNameAsync(confirmEmailModel.UserNameOrEmail);

            if(user == null)
            {
                return new ResponseModel<ConfirmEmailModel>()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Email chưa được đăng ký trên hệ thống"
                };
            }

            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailModel.Token);
            user.UpdateAt = DateTime.Now;
            await _userManager.UpdateAsync(user);
            
            // Thêm hàm cập nhật updatedAt vào đây

            if(result.Succeeded)
            {
                return new ResponseModel<ConfirmEmailModel>()
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Email đã được xác nhận thành công"
                };
            }
            return new ResponseModel<ConfirmEmailModel>()
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Xác nhận Email thất bại"
            };
        }

        

        #endregion

        #region ForgotPassword + Change Password

        public async Task<ResponseModel<ResetPasswordModel>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResponseModel<ResetPasswordModel>()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Email chưa được đăng ký!"
                };
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenEncoded = Uri.EscapeDataString(token);

            var confirmationLink = $" http://localhost:5173/ChangePasswordView?UserNameOrEmail={user.Email}&Token={tokenEncoded}";
            var htmlContent = $@"
                <html>
                <body>
                    <p>Xin chào,</p>
                    <p>Bạn có muốn thay đổi mật khẩu? Nhấp vào liên kết dưới đây để tiếp tục:</p>
                    <a href='{confirmationLink}'>Thay đổi mật khẩu</a>
                </body>
                </html>";

            var message = new EmailMessageSetUpModel(new string[] { user.Email! }, "Thay đổi mật khẩu", htmlContent);
            _emailService.SendEmail(message);

            return new ResponseModel<ResetPasswordModel>()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Đường dẫn thay đổi mật khẩu mới đã được gửi đến mail đăng ký trên hệ thống"
            };
        }

        public async Task<ResponseModel<ResetPasswordModel>> ChangePassword(ResetPasswordModel resetPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if(user == null)
            {
                return new ResponseModel<ResetPasswordModel>()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Email này chưa được đăng ký trên hệ thống"
                };
            }
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            user.UpdateAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            if(resetPassResult.Succeeded)
            {
                return new ResponseModel<ResetPasswordModel>()
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Đổi mật khẩu thành công!"
                };
            }
            return new ResponseModel<ResetPasswordModel>()
            {
                Success = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Đổi mật khẩu thất bại do Server!"
            };
        }

        #endregion

        #region Generate + Get Principle Token

        private JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> authClaims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: signinCredentials
                );
            return token;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principle = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if(jwtSecurityToken != null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principle;
        }

        #endregion
    }
}
