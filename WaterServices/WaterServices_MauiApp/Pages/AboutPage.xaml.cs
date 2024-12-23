using WaterServices_MauiApp.Services;
using WaterServices_MauiApp.Validations;

namespace WaterServices_MauiApp.Pages;

public partial class AboutPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public AboutPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }
}