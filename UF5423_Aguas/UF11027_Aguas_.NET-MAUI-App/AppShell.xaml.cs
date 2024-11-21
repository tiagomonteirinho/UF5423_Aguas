using UF11027_Aguas_.NET_MAUI_App.Pages;
using UF11027_Aguas_.NET_MAUI_App.Services;
using UF11027_Aguas_.NET_MAUI_App.Validations;

namespace UF11027_Aguas_.NET_MAUI_App
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;

        public AppShell(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage();
            var notificationsPage = new NotificationsPage();
            var consumptionsPage = new ConsumptionsPage();
            var accountPage = new AccountPage();

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
}
