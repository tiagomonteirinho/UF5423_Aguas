using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;
using UF5423_Aguas.Models;

namespace UF5423_Aguas.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IUserRepository _userRepository;

        public UsersController(IUserHelper userHelper, IUserRepository userRepository)
        {
            _userHelper = userHelper;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return View(users);
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
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Could not login.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;
                if(model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\users",
                        model.ImageFile.FileName
                        );

                    using(var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/users/{model.ImageFile.FileName}";
                }
                else
                {
                    // Set a default image if no image is uploaded
                    path = "~/images/users/default.png"; // Ensure this default image exists
                }

                var user = this.ConvertToUser(model, path);

                user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    user = new User
                    {
                        FullName = model.FullName,
                        Email = model.Email,
                        UserName = model.Email,
                        ImageUrl = path
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "Could not register user account.");
                        return View(model);
                    }

                    var loginViewModel = new LoginViewModel
                    {
                        Email = model.Email,
                        Password = model.Password,
                        StaySignedIn = false,
                    };

                    var result2 = await _userHelper.LoginAsync(loginViewModel);
                    if (result2.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "Could not log in.");
                }

                ModelState.AddModelError(string.Empty, "A user already exists with that email address.");
            }

            return View(model);
        }

        private User ConvertToUser(RegisterViewModel model, string path)
        {
            return new User
            {
                Id = model.Id,
                Email = model.Email,
                UserName = model.UserName,
                FullName = model.FullName,
                ImageUrl = model.ImageUrl,
            };
        }

        public async Task<IActionResult> ChangeUserInfo()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangeUserInfoViewModel
            {
                FullName = user.FullName,
                ImageUrl = user.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserInfo(ChangeUserInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    user.FullName = model.FullName;

                    // If a new image is uploaded
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\users",
                            model.ImageFile.FileName
                        );

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }

                        user.ImageUrl = $"~/images/users/{model.ImageFile.FileName}";
                    }

                    var response = await _userHelper.ChangeUserInfoAsync(user);
                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User info updated successfully";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }

            return View(model);
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
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUserInfo");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }

            return View(model);
        }
    }
}
