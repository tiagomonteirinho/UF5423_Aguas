using UF11027_Aguas_.NET_MAUI_App.Services;
using UF11027_Aguas_.NET_MAUI_App.Validations;

namespace UF11027_Aguas_.NET_MAUI_App.Pages;

public partial class RequestWaterMeterPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public RequestWaterMeterPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void login_tap_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void requestMeter_button_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(address_entry.Text))
        {
            await DisplayAlert("Error", "Address is required.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(postalCode_entry.Text))
        {
            await DisplayAlert("Error", "Postal code is required.", "OK");
            return;
        }

        if (await _validator.Validate(name_entry.Text, email_entry.Text, phoneNumber_entry.Text, serialNumber_entry.Text))
        {
            var response = await _apiService.RequestWaterMeter(name_entry.Text, email_entry.Text,
                phoneNumber_entry.Text, address_entry.Text, postalCode_entry.Text, serialNumber_entry.Text);
            if (!response.HasError)
            {
                await DisplayAlert("Success", "Water meter request successfully submitted.", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator));
            }
            else
            {
                await DisplayAlert("Error", "Could not submit water meter request.", "OK");
            }
        }
        else
        {
            string errorMessage = "";
            errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
            errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            errorMessage += _validator.PhoneNumberError != null ? $"\n- {_validator.PhoneNumberError}" : "";
            errorMessage += _validator.SerialNumberError != null ? $"\n- {_validator.SerialNumberError}" : "";
            await DisplayAlert("Error", errorMessage, "Ok");
        }
    }
}