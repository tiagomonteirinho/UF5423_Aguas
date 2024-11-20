using UF11027_Aguas_.NET_MAUI_App.Services;

namespace UF11027_Aguas_.NET_MAUI_App.Pages;

public partial class RequestWaterMeterPage : ContentPage
{
    private readonly ApiService _apiService;

    public RequestWaterMeterPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }
}