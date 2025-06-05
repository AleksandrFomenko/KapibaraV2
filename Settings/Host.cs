using System.Windows;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;
using Settings.Configuration;
using Settings.Views;
using Settings.ViewModels;

namespace Settings;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes
/// </summary>
public static class Host
{
    private static IServiceProvider _serviceProvider;

    public static void Start()
    {
        var services = new ServiceCollection();

        if (Context.ActiveDocument != null) services.AddSingleton(Context.ActiveDocument);
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<SettingsView>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<Config>();
            

        _serviceProvider = services.BuildServiceProvider();
    }
    
    public static T GetService<T>() where T : class
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}