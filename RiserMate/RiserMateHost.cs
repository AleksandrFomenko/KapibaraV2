using KapibaraUI.Services.Appearance;
using KapibaraUI.Services.Navigation;
using Microsoft.Extensions.DependencyInjection;
using RiserMate.Views;
using RiserMate.ViewModels;
using Wpf.Ui;
using Wpf.Ui.Abstractions;

namespace RiserMate;

public static class RiserMateHost
{
    private static IServiceProvider? _serviceProvider;
    
    public static void StartMockServices()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<RiserMateViewModel>();
        services.AddSingleton<RiserMateView>();
        
        services.AddSingleton<RiserCreator>();
        
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<INavigationViewPageProvider, PageService>();

        _serviceProvider = services.BuildServiceProvider();
    }
    public static T GetService<T>() where T : class
    {
        return _serviceProvider!.GetRequiredService<T>();
    }
}