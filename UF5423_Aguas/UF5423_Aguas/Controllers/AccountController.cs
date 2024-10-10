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
    private readonly IBlobHelper _blobHelper;

    public AccountController(IUserHelper userHelper, IBlobHelper blobHelper)
    {
        _userHelper = userHelper;
        _blobHelper = blobHelper;
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
        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            user.ImageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
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
            ImageId = user.ImageId,
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
}
