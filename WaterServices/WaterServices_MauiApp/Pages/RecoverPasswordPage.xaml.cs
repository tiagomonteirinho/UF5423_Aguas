using WaterServices_MauiApp.Services;
using WaterServices_MauiApp.Validations;

namespace WaterServices_MauiApp.Pages;

public partial class RecoverPasswordPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public RecoverPasswordPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void sendEmail_button_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(email_entry.Text))
        {
            await DisplayAlert("Error", "Email is required.", "OK");
            return;
        }

        var response = await _apiService.RecoverPassword(email_entry.Text);
        if (!response.HasError)
        {
            await DisplayAlert("Success", "Password reset email successfully sent.", "OK");
            Application.Current!.MainPage = new LoginPage(_apiService, _validator);
        }
        else
        {
            await DisplayAlert("Error", "Could not sign in.", "OK");
        }
    }
}