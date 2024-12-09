using UF11027_Aguas_.NET_MAUI_App.Models;
using UF11027_Aguas_.NET_MAUI_App.Services;
using UF11027_Aguas_.NET_MAUI_App.Validations;

namespace UF11027_Aguas_.NET_MAUI_App.Pages;

public partial class ConsumptionsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public ConsumptionsPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetConsumptions();
    }

    private async Task GetConsumptions()
    {
        try
        {
            consumptionsLoaded_ai.IsRunning = true;
            consumptionsLoaded_ai.IsVisible = true;

            var (consumptions, errorMessage) = await _apiService.GetConsumptions();
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Error", "Could not find consumptions.", "OK");
                return;
            }

            if (consumptions == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not find consumptions.", "OK");
                return;
            }

            if (consumptions.Count == 0)
            {
                noConsumptions_lbl.IsVisible = true;
                return;
            }
            else
            {
                consumptions_collection.ItemsSource = consumptions;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not process request: {ex.Message}", "OK");
        }
        finally
        {
            consumptionsLoaded_ai.IsRunning = false;
            consumptionsLoaded_ai.IsVisible = false;
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
}