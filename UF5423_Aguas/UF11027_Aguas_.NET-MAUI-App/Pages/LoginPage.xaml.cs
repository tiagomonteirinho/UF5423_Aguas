using UF11027_Aguas_.NET_MAUI_App.Services;
using UF11027_Aguas_.NET_MAUI_App.Validations;

namespace UF11027_Aguas_.NET_MAUI_App.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;

        public LoginPage(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
        }

        private async void login_button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(email_entry.Text))
            {
                await DisplayAlert("Error", "Email is required.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(password_entry.Text))
            {
                await DisplayAlert("Error", "Password is required.", "OK");
                return;
            }

            var response = await _apiService.Login(email_entry.Text, password_entry.Text);
            if (!response.HasError)
            {
                Application.Current!.MainPage = new AppShell(_apiService, _validator);
            }
            else
            {
                await DisplayAlert("Error", "Could not sign in.", "OK");
            }
        }

        private async void requestWaterMeter_tap_Tapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new RequestWaterMeterPage(_apiService, _validator));
        }

        private async void recoverPassword_tap_Tapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new RecoverPassword(_apiService, _validator));
        }
    }
}