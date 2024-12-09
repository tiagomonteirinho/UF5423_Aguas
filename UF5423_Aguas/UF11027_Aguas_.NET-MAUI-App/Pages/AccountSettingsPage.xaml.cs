using UF11027_Aguas_.NET_MAUI_App.Services;
using UF11027_Aguas_.NET_MAUI_App.Validations;

namespace UF11027_Aguas_.NET_MAUI_App.Pages;

public partial class AccountSettingsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    private const string UserNameKey = "username";

    public AccountSettingsPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoadAccountData();
        image_imgBtn.Source = await GetImage();
    }

    private void LoadAccountData()
    {
        name_lbl.Text = Preferences.Get(UserNameKey, string.Empty);
        name_entry.Text = Preferences.Get(UserNameKey, string.Empty);
    }

    private async Task<string?> GetImage()
    {
        string defaultUserImageUrl = AppConfig.DefaultUserImageUrl;

        var (response, errorMessage) = await _apiService.GetImage();
        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        if (response == null || string.IsNullOrEmpty(response?.ImageUrl))
        {
            return defaultUserImageUrl;
        }

        return response.ImageUrl;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void save_btn_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(name_entry.Text))
        {
            await DisplayAlert("Error", "Name cannot be empty.", "OK");
            return;
        }

        var response = await _apiService.ChangeInfo(name_entry.Text);
        if (!response.HasError)
        {
            Preferences.Set(UserNameKey, name_entry.Text);
            await DisplayAlert("Success", "Changes successfully saved.", "OK");
            name_lbl.Text = name_entry.Text;
        }
        else
        {
            await DisplayAlert("Error", "Could not change info.", "OK");
        }
    }

    private void changePassword_tap_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new ChangePasswordPage(_apiService, _validator));
    }
}