namespace UF11027_Aguas_.NET_MAUI_App.Models;

public class UserImage
{
    public string? ImageUrl { get; set; }

    public string? ImagePath => AppConfig.BaseUrl + ImageUrl;
}
