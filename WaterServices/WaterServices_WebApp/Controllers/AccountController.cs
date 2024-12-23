using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WaterServices_WebApp.Data.Entities;
using WaterServices_WebApp.Helpers;
using WaterServices_WebApp.Models;

namespace WaterServices_WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;

        public AccountController(IUserHelper userHelper, IConfiguration configuration, IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _mailHelper = mailHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid login attempt.";
                return View(model);
            }

            var result = await _userHelper.LoginAsync(model);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = "Could not log in.";
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateApiToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
                }

                var result = await _userHelper.ValidatePasswordAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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

                    return this.Created(string.Empty, results);
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> ChangeInfo()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = ConvertToChangeInfoViewModel(user);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeInfo(ChangeInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Could not update user info.";
                return View(model);
            }

            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            if (model.FullName == user.FullName && model.ImageFile == null) // Prevent image file duplication.
            {
                ViewBag.ErrorMessage = "No changes detected. No updates were made.";
                return View(ConvertToChangeInfoViewModel(user)); // Keep view user info.
            }

            user.FullName = model.FullName;

            var path = model.ImageUrl;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var guid = Guid.NewGuid().ToString();
                string fileName = $"{guid}.jpg";
                path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot\\images\\users",
                    fileName
                );
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                user.ImageUrl = $"~/images/users/{fileName}";
            }

            var response = await _userHelper.ChangeInfoAsync(user);
            if (response.Succeeded)
            {
                ViewBag.SuccessMessage = "User info updated successfully!";
                return View(ConvertToChangeInfoViewModel(user)); // Update view user info.
            }

            ViewBag.ErrorMessage = "Could not update user info.";
            return View(model);
        }

        private ChangeInfoViewModel ConvertToChangeInfoViewModel(User user)
        {
            return new ChangeInfoViewModel
            {
                FullName = user.FullName,
                ImageUrl = user.ImageUrl,
                ImageFullPath = user.ImageFullPath
            };
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
                }

                var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    ViewBag.SuccessMessage = "Password updated successfully!";
                    return View();
                }

                ViewBag.ErrorMessage = result.Errors.FirstOrDefault().Description;
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "Could not find that email address.";
                    return View(model);
                }

                var passwordToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var actionUrl = Url.Action
                (
                    "SetPassword",
                    "Account",
                    new { email = user.Email, passwordToken },
                    protocol: HttpContext.Request.Scheme,
                    host: AppConfig.Host
                );

                bool emailSent = _mailHelper.SendEmail(user.Email, "Password recovery", $"<h2>Password recovery</h2>"
                    + $"To recover your password, please reset it <a href=\"{actionUrl}\" style=\"color: blue;\">here</a>.");

                if (!emailSent)
                {
                    ViewBag.ErrorMessage = "Could not send password recovery email.";
                    return View(model);
                }

                ViewBag.SuccessMessage = "A password recovery email has been sent to your email address. Please find it and follow the instructions.";
                return View();
            }

            return View(model);
        }

        public IActionResult SetPassword(string email, string passwordToken, string confirmationToken)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                _userHelper.LogoutAsync(); // Sign out from current session.
            }

            return View(new SetPasswordViewModel
            {
                Email = email,
                PasswordToken = passwordToken,
                ConfirmationToken = confirmationToken
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Could not find that email address.";
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.ConfirmationToken))
            {
                var confirmEmail = await _userHelper.ConfirmAccountAsync(user, model.ConfirmationToken);
                if (!confirmEmail.Succeeded)
                {
                    return RedirectToAction("NotFound404", "Errors");
                }
            }

            var result = await _userHelper.SetPasswordAsync(user, model.PasswordToken, model.Password);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = "Could not reset password.";
                return View(model);
            }

            ViewBag.SuccessMessage = "Password updated successfully! You may now sign in to your account.";
            return View();
        }
    }
}