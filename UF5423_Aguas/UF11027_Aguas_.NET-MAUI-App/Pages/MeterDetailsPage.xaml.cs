using UF11027_Aguas_.NET_MAUI_App.Models;
using UF11027_Aguas_.NET_MAUI_App.Services;
using UF11027_Aguas_.NET_MAUI_App.Validations;

namespace UF11027_Aguas_.NET_MAUI_App.Pages;

public partial class MeterDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isDataLoaded = false;
    private int _meterId;

    public MeterDetailsPage(int id, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _meterId = id;

        GetMeterDetails(_meterId);
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

            if (meterDetails.Consumptions == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not find consumptions.", "OK");
                return;
            }

            if (meterDetails.Consumptions.Count == 0)
            {
                noConsumptions_lbl.IsVisible = true;
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

    private void consumptions_collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Consumption;
        if (currentSelection == null) return;

        Navigation.PushAsync(new ConsumptionDetailsPage(currentSelection.Id, _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }

    private async void addConsumption_imgBtn_Clicked(object sender, EventArgs e)
    {
        if (DateTime.Now.Day > 5)
        {
            await DisplayAlert("Error", "Consumptions can only be added during the first 5 days of each month.", "OK");
        }
        else
        {
            await Navigation.PushAsync(new AddConsumptionPage(_meterId, _apiService, _validator));
        }
    }
}