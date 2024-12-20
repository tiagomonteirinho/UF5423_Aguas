using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Water_Services_.NET_MAUI_App.Services;
using Water_Services_.NET_MAUI_App.Validations;

namespace Water_Services_.NET_MAUI_App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<IValidator, Validator>();

        return builder.Build();
    }
}
