using UF11027_Aguas_.NET_MAUI_App.Services;
using UF11027_Aguas_.NET_MAUI_App.Validations;

namespace UF11027_Aguas_.NET_MAUI_App.Pages
{
	public partial class AddConsumptionPage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private bool _loginPageDisplayed = false;
        private bool _isDataLoaded = false;
        private int _meterId;

        public AddConsumptionPage(int meterId, ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
            _meterId = meterId;
        }

        private async void addConsumption_btn_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(volume_entry.Text))
            {
                await DisplayAlert("Error", "Volume is required.", "OK");
                return;
            }

            var response = await _apiService.AddConsumption(_meterId, volume_entry.Text);
            if (!response.HasError)
            {
                await DisplayAlert("Success", "Consumption successfully added.", "OK");
                volume_entry.Text = null;
            }
            else
            {
                await DisplayAlert("Error", "Could not add consumption.", "OK");
            }
        }
    }
}