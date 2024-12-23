using WaterServices_MauiApp.Services;
using WaterServices_MauiApp.Validations;

namespace WaterServices_MauiApp.Pages;

public partial class InvoicePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isDataLoaded = false;
    private int _consumptionId;

    public InvoicePage(int consumptionId, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _consumptionId = consumptionId;

        GetInvoiceDetails(_consumptionId);
    }
    
    private async void GetInvoiceDetails(int consumptionId)
    {
        try
        {
            invoiceDetailsLoaded_ai.IsRunning = true;
            invoiceDetailsLoaded_ai.IsVisible = true;

            var (invoiceDetails, errorMessage) = await _apiService.GetInvoiceDetails(consumptionId);
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (invoiceDetails == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not find consumption details.", "OK");
                return;
            }

            BindingContext = invoiceDetails;

            if (invoiceDetails.Consumption?.Status == "Awaiting payment")
            {
                buyConsumption_btn.IsVisible = true;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not process request.", "OK");
        }
        finally
        {
            invoiceDetailsLoaded_ai.IsRunning = false;
            invoiceDetailsLoaded_ai.IsVisible = false;
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void buyConsumption_btn_Clicked(object sender, EventArgs e)
    {
        try
        {
            var errorMessage = await _apiService.BuyConsumption(_consumptionId);
            if (errorMessage == null)
            {
                await DisplayAlert("Success", "Consumption payment successfully confirmed.", "OK");
                buyConsumption_btn.IsVisible = false;
            }

            else
            {
                await DisplayAlert("Error", errorMessage ?? "Could not process request.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
        }
    }
}