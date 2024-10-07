using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;
using UF5423_Aguas.Models;

public class AccountController : Controller
{
    private readonly IUserHelper _userHelper;

    public AccountController(IUserHelper userHelper)
    {
        _userHelper = userHelper;
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
                return this.RedirectToAction("Index", "Home");
            }
        }

        ViewBag.ErrorMessage = "Could not login.";
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _userHelper.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> ChangeInfo()
    {
        var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
        var model = new ChangeInfoViewModel
        {
            FullName = user.FullName,
            ImageUrl = user.ImageUrl
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeInfo(ChangeInfoViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            if (user != null)
            {
                if (model.FullName == user.FullName && model.ImageFile == null)
                {
                    ViewBag.ErrorMessage = "No changes detected. No updates were made.";
                    return View(new ChangeInfoViewModel
                    {
                        ImageUrl = user.ImageUrl, // Keep image.
                    });
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
                    return View(new ChangeInfoViewModel
                    {
                        ImageUrl = user.ImageUrl, // Update image.
                    });
                }

                ViewBag.ErrorMessage = "Could not update user info.";
                return View(model);
            }
        }

        ViewBag.ErrorMessage = "Could not update user info.";
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
}
