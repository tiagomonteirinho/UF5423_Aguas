namespace WaterServices_MauiApp.Models;

public class UserImage
{
    public string? ImageUrl { get; set; }

    public string? ImagePath => AppConfig.BaseUrl + ImageUrl;
}
