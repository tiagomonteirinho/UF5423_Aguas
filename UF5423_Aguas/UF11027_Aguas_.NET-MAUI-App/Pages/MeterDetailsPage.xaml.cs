using UF11027_Aguas_.NET_MAUI_App.Services;
using UF11027_Aguas_.NET_MAUI_App.Validations;

namespace UF11027_Aguas_.NET_MAUI_App.Pages
{
    public partial class MeterDetailsPage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private bool _loginPageDisplayed = false;
        private bool _isDataLoaded = false;

        public MeterDetailsPage(int id, ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;

            GetMeterDetails(id);
        }

        private async void GetMeterDetails(int id)
        {
            try
            {
                meterDetailsLoaded_ai.IsRunning = true;
                meterDetailsLoaded_ai.IsVisible = true;

                var (meterDetails, errorMessage) = await _apiService.GetMeterDetails(id);
                if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
                {
                    await DisplayLoginPage();
                    return;
                }

                if (meterDetails == null)
                {
                    await DisplayAlert("Error", errorMessage ?? "Could not find meter details.", "OK");
                    return;
                }

                BindingContext = meterDetails;
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Could not process request.", "OK");
            }
            finally
            {
                meterDetailsLoaded_ai.IsRunning = false;
                meterDetailsLoaded_ai.IsVisible = false;
            }
        }

        private async Task DisplayLoginPage()
        {
            _loginPageDisplayed = true;
            await Navigation.PushAsync(new LoginPage(_apiService, _validator));
        }
    }
}