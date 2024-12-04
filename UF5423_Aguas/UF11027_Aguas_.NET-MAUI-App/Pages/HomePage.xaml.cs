using UF11027_Aguas_.NET_MAUI_App.Models;
using UF11027_Aguas_.NET_MAUI_App.Services;
using UF11027_Aguas_.NET_MAUI_App.Validations;

namespace UF11027_Aguas_.NET_MAUI_App.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private bool _loginPageDisplayed = false;
        private bool _isDataLoaded = false;

        public HomePage(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;
            greetings_label.Text = "Hello, " + Preferences.Get("username", string.Empty);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await GetMeters();
        }

        private async Task<IEnumerable<Meter>> GetMeters()
        {
            try
            {
                var (meters, errorMessage) = await _apiService.GetMeters();
                if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
                {
                    await DisplayLoginPage();
                    return Enumerable.Empty<Meter>();
                }

                if (meters == null)
                {
                    await DisplayAlert("Error", errorMessage ?? "Could not find water meters.", "OK");
                    return Enumerable.Empty<Meter>();
                }

                meters_collection.ItemsSource = meters;
                return meters;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
                return Enumerable.Empty<Meter>();
            }
        }

        private async Task DisplayLoginPage()
        {
            _loginPageDisplayed = true;
            await Navigation.PushAsync(new LoginPage(_apiService, _validator));
        }

        private async void meters_collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentSelection = e.CurrentSelection.FirstOrDefault() as Meter;
            if (currentSelection == null) return;

            // Include meter consumptions.
            var (meterDetails, errorMessage) = await _apiService.GetMeterDetails(currentSelection.Id);
            if (meterDetails == null) return;

            await Navigation.PushAsync(new MeterDetailsPage(currentSelection.Id, _apiService, _validator));

            ((CollectionView)sender).SelectedItem = null;
        }
    }
}
