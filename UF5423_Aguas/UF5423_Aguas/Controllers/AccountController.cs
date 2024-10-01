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

                var response = await _userHelper.EditUserAsync(user);
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
                    return RedirectToAction("ChangeInfo");
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
