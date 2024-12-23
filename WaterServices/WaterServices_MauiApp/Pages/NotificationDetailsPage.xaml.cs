using WaterServices_MauiApp.Services;
using WaterServices_MauiApp.Validations;

namespace WaterServices_MauiApp.Pages;

public partial class NotificationDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public NotificationDetailsPage(int id, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;

        GetNotificationDetails(id);
    }

    private async void GetNotificationDetails(int id)
    {
        try
        {
            notificationDetailsLoaded_ai.IsRunning = true;
            notificationDetailsLoaded_ai.IsVisible = true;

            var (notificationDetails, errorMessage) = await _apiService.GetNotificationDetails(id);
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (notificationDetails == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not find notification details.", "OK");
                return;
            }

            BindingContext = notificationDetails;
            await _apiService.MarkNotificationAsRead(id);
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not process request.", "OK");
        }
        finally
        {
            notificationDetailsLoaded_ai.IsRunning = false;
            notificationDetailsLoaded_ai.IsVisible = false;
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}