﻿using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using SportStore.BusinessLogicLayer.Services;
using SportStore.DataAccessLayer.Data;
using SportStore.DataAccessLayer.Models;
using SportStore.WebApi.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SportStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SportStoreDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailSender _emailSender;

        public AuthenticationController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SportStoreDbContext context,
            IConfiguration configuration,
            EmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVm registerVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser
            {
                FirstName = registerVm.FirstName,
                LastName = registerVm.LastName,
                Email = registerVm.Email,
                UserName = registerVm.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerVm.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest($"Không thể tạo người dùng: {errors}");
            }

            if (await _roleManager.RoleExistsAsync("User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Authentication",
                new { token, email = user.Email }, Request.Scheme);

            var emailMessage = $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.";
            await _emailSender.SendEmailAsync(user.Email, "Xác nhận tài khoản", emailMessage);

            return Created(nameof(Register), $"Tài khoản {registerVm.Email} đã được đăng ký! Vui lòng xác nhận tài khoản.");
        }

        [HttpGet("confirm-email")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Yêu cầu xác nhận email không hợp lệ.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email được xác nhận thành công!");
            }
            else
            {
                return BadRequest("Xác nhận email không thành công.");
            }
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody] LoginVm loginVm)
        {
            var user = await _userManager.FindByEmailAsync(loginVm.Email);

            if (user == null)
            {
                return BadRequest("Email không tồn tại.");
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                return BadRequest($"Tài khoản của bạn đã bị khóa. Vui lòng thử lại sau {user.LockoutEnd - DateTime.UtcNow}.");
            }

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginVm.Password);
            if (!isPasswordValid)
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(5);
                    await _userManager.UpdateAsync(user);

                    return BadRequest($"Tài khoản của bạn đã bị khóa. Vui lòng thử lại sau {user.LockoutEnd - DateTime.UtcNow}.");
                }

                await _userManager.UpdateAsync(user);
                return BadRequest($"Bạn đã nhập sai mật khẩu {user.FailedLoginAttempts}/5 lần. Sau 5 lần sai, tài khoản sẽ bị khóa 5 phút.");
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            var tokenValue = await GenerateJWTToken(user);
            return Ok(tokenValue);
        }

        [HttpPost("forgot-password")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordVm forgotPasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Email là bắt buộc");
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordVm.Email);
            if (user == null)
            {
                return Ok("Đã gửi yêu cầu.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Authentication",
                new { token, email = forgotPasswordVm.Email }, Request.Scheme);

            var emailMessage = $"Please reset your password by clicking <a href='{resetLink}'>here</a>.";
            await _emailSender.SendEmailAsync(forgotPasswordVm.Email, "Đặt lại mật khẩu", emailMessage);

            return Ok("Đã gửi yêu cầu.");
        }

        [HttpPost("send-email")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SendEmailAsync([FromBody] SendEmailVm sendEmailVm)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress(sendEmailVm.ToEmail, sendEmailVm.ToEmail));
            emailMessage.Subject = sendEmailVm.Subject;
            emailMessage.Body = new TextPart("html") { Text = sendEmailVm.Message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

            return Ok("Email đã được gửi thành công.");
        }

        [HttpPost("reset-password")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordVm resetPasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordVm.Email);
            if (user == null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }
            Console.WriteLine($"Token received: {resetPasswordVm.Token}");
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordVm.Token, resetPasswordVm.NewPassword);

            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            return Ok("Mật khẩu đã được đặt lại thành công.");
        }

        private async Task<AuthResultVm> GenerateJWTToken(AppUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            var response = new AuthResultVm
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = token.ValidTo
            };

            return response;
        }
    }
}