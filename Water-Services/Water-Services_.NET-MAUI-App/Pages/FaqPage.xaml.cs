using Water_Services_.NET_MAUI_App.Services;
using Water_Services_.NET_MAUI_App.Validations;

namespace Water_Services_.NET_MAUI_App.Pages;

public partial class FaqPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public FaqPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }
}