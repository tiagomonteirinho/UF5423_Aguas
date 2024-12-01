using UF11027_Aguas_.NET_MAUI_App.Services;
using UF11027_Aguas_.NET_MAUI_App.Validations;

namespace UF11027_Aguas_.NET_MAUI_App.Pages
{
    public partial class NotificationsPage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private bool _loginPageDisplayed = false;

        public NotificationsPage(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await GetNotifications();
        }

        private async Task GetNotifications()
        {
            try
            {
                notificationsLoaded_ai.IsRunning = true;
                notificationsLoaded_ai.IsVisible = true;

                var (notifications, errorMessage) = await _apiService.GetNotifications();
                if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
                {
                    await DisplayLoginPage();
                    return;
                }

                if (errorMessage == "NotFound")
                {
                    await DisplayAlert("Error", "No existing notifications.", "OK");
                    return;
                }

                if (notifications == null)
                {
                    await DisplayAlert("Error", errorMessage ?? "Could not find notifications.", "OK");
                    return;
                }
                else
                {
                    notifications_collection.ItemsSource = notifications;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
            }
            finally
            {
                notificationsLoaded_ai.IsRunning = false;
                notificationsLoaded_ai.IsVisible = false;
            }
        }

        private async Task DisplayLoginPage()
        {
            _loginPageDisplayed = true;
            await Navigation.PushAsync(new LoginPage(_apiService, _validator));
        }
    }
}