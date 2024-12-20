using Android.Net;
using Water_Services_.NET_MAUI_App.Pages;
using Water_Services_.NET_MAUI_App.Services;
using Water_Services_.NET_MAUI_App.Validations;

namespace Water_Services_.NET_MAUI_App;

public partial class AppShell : Shell
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public AppShell(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
        ConfigureShell();
    }

    private void ConfigureShell()
    {
        var homePage = new HomePage(_apiService, _validator);
        var notificationsPage = new NotificationsPage(_apiService, _validator);
        var consumptionsPage = new ConsumptionsPage(_apiService, _validator);
        var accountPage = new AccountPage(_apiService, _validator);

        Items.Add(new TabBar
        {
            Items =
            {
                new ShellContent { Title = "Home", Icon = "home", Content = homePage },
                new ShellContent { Title = "Notifications", Icon = "bell", Content = notificationsPage },
                new ShellContent { Title = "Consumptions", Icon = "consumption", Content = consumptionsPage },
                new ShellContent { Title = "Account", Icon = "account", Content = accountPage }
            }
        });
    }
}
