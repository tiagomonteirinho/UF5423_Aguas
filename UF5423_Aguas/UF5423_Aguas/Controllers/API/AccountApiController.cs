﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UF5423_Aguas.Data.API;
using UF5423_Aguas.Helpers;

namespace UF5423_Aguas.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountApiController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;

        public AccountApiController(IUserHelper userHelper, IConfiguration configuration, IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _mailHelper = mailHelper;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Could not process request.");
            }

            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("Invalid email or password.");
            }

            var result = await _userHelper.ValidatePasswordAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return NotFound("Invalid email or password.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
            (
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(15),
                signingCredentials: credentials
            );

            var results = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
            };

            return Ok(new
            {
                results
            });
        }

        [HttpPost("changeinfo")]
        public async Task<IActionResult> ChangeInfo(ChangeInfoDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Could not process request.");
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("Invalid email or password.");
            }

            user.FullName = model.FullName;

            var response = await _userHelper.ChangeInfoAsync(user);
            if (!response.Succeeded)
            {
                ViewBag.ErrorMessage = "Could not update user info.";
                return View(model);
            }

            return Ok(new { Message = "User info successfully updated." });
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return BadRequest(new { Message = "Could not update password." }); //TODO: Fix error when logged in somewhere else.
            }

            return Ok(new { Message = "Password successfully updated." });
        }

        [HttpPost("recoverpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var passwordToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            var actionUrl = Url.Action
            (
                "SetPassword",
                "Account",
                new { email = user.Email, passwordToken },
                protocol: HttpContext.Request.Scheme
            );

            bool emailSent = _mailHelper.SendEmail(user.Email, "Password recovery", $"<h2>Password recovery</h2>"
                + $"To recover your password, please reset it <a href=\"{actionUrl}\" style=\"color: blue;\">here</a>.");

            if (!emailSent)
            {
                return BadRequest(new { Message = "Could not send password recovery email." });
            }

            return Ok(new { Message = "A password recovery email has been sent to your email address. Please find it and follow the instructions." });
        }
    }
}