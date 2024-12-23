using WaterServices_MauiApp.Models;
using WaterServices_MauiApp.Services;
using WaterServices_MauiApp.Validations;

namespace WaterServices_MauiApp.Pages;

public partial class ConsumptionDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isDataLoaded = false;

    public ConsumptionDetailsPage(int id, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;

        GetConsumptionDetails(id);
    }

    private async void GetConsumptionDetails(int id)
    {
        try
        {
            consumptionDetailsLoaded_ai.IsRunning = true;
            consumptionDetailsLoaded_ai.IsVisible = true;

            var (consumptionDetails, errorMessage) = await _apiService.GetConsumptionDetails(id);
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (consumptionDetails == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not find consumption details.", "OK");
                return;
            }

            BindingContext = consumptionDetails;

            if (consumptionDetails.Status == "Payment confirmed")
            {
                invoice_btn.IsVisible = true;
                invoice_btn.Text = "Invoice";
            }
            else if (consumptionDetails.Status == "Awaiting payment")
            {
                invoice_btn.IsVisible = true;
                invoice_btn.Text = "Buy consumption";
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not process request.", "OK");
        }
        finally
        {
            consumptionDetailsLoaded_ai.IsRunning = false;
            consumptionDetailsLoaded_ai.IsVisible = false;
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void invoice_btn_Clicked(object sender, EventArgs e)
    {
        var consumption = BindingContext as Consumption;
        if (consumption == null) return;

        await Navigation.PushAsync(new InvoicePage(consumption.Id, _apiService, _validator));
    }
}