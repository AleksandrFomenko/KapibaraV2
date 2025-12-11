using System.Windows;
using KapibaraUI.Services.Appearance;
using KapibaraUI.Services.Navigation;
using Microsoft.Extensions.DependencyInjection;
using RiserMate.Abstractions;
using RiserMate.Implementation;
using RiserMate.Models.Design;
using RiserMate.Models.Revit;
using RiserMate.Views;
using RiserMate.ViewModels;
using Wpf.Ui;
using Wpf.Ui.Abstractions;

namespace RiserMate;

public static class RiserMateHost
{
    private static IServiceProvider? _serviceProvider;
    public static Window? Window { get; private set; }
    
    public static void StartMockServices()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<RiserMateViewModel>();
        services.AddSingleton<RiserMateView>();
        
        services.AddSingleton<RiserCreator>();
        services.AddSingleton<RizerCreatorViewModel>();
        services.AddSingleton<IModelRiserCreator, ModelRiserCreatorMock>();
        
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<INavigationViewPageProvider, PageService>();
        services.AddSingleton<IConfigRiserMate, ConfigRiserMateService>();

        _serviceProvider = services.BuildServiceProvider();
        
        var theme = _serviceProvider.GetService<IThemeWatcherService>();
        theme?.Initialize();
    }
    
    public static void StartServices()
    {
        
        Handlers.Handlers.RegisterHandlers();
        var services = new ServiceCollection();
        
        services.AddSingleton<RiserMateViewModel>();
        services.AddSingleton<RiserMateView>();
        
        services.AddSingleton<RiserCreator>();
        services.AddSingleton<RizerCreatorViewModel>();
        services.AddSingleton<IModelRiserCreator, ModelRiserMateCreator>();
        
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<INavigationViewPageProvider, PageService>();
        
        services.AddSingleton<IViewCreationService, ViewCreationService>();
        services.AddSingleton<IFilterCreationService, FilterCreationService>();
        
        services.AddSingleton<IConfigRiserMate, ConfigRiserMateService>();
        

        _serviceProvider = services.BuildServiceProvider();
        
        var theme = _serviceProvider.GetService<IThemeWatcherService>();
        Window = _serviceProvider.GetService<RiserMateView>();
        
        theme?.SetConfigTheme();
        if (Window != null) Window.SourceInitialized += (sender, args) => theme?.SetConfigTheme();
    }

    public static void Show() => Window?.Show(Context.UiApplication.MainWindowHandle);

    public static T GetService<T>() where T : class
    {
        return _serviceProvider!.GetRequiredService<T>();
    }
}