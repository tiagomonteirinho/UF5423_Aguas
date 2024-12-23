namespace WaterServices_MauiApp.Models;

class ChangePassword
{
    public string? OldPassword { get; set; }

    public string? NewPassword { get; set; }

    public string? ConfirmNewPassword { get; set; }
}
