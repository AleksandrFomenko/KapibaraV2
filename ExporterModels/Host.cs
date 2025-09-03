using ExporterModels.Abstractions;
using ExporterModels.services;
using ExporterModels.ViewModels;
using ExporterModels.Views;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;

namespace ExporterModels;

public static class Host
{
    private static IServiceProvider? _serviceProvider;

    public static void Start()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<IWindowOwnerProvider, WindowOwnerProvider>();
        services.AddSingleton<ConfigurationService>();
        services.AddSingleton<ExporterModelsViewModel>();
        services.AddSingleton<ExporterModelsView>();

        _serviceProvider = services.BuildServiceProvider();

        var view = GetService<ExporterModelsView>();
        var windowOwnerProvider = GetService<IWindowOwnerProvider>();
        windowOwnerProvider.SetOwner(view);
        var tws = GetService<IThemeWatcherService>();
        view.SourceInitialized += (sender, args) => tws.SetConfigTheme();
        view.Show(Context.UiApplication.MainWindowHandle);
        
    }
    
    public static T GetService<T>() where T : class
    {
        return _serviceProvider!.GetRequiredService<T>();
    }
}