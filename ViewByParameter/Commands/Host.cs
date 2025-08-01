using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;
using ViewByParameter.AddFilter.View;
using ViewByParameter.Models;
using ViewByParameter.ViewModels;
using ViewByParameter.Views;

namespace ViewByParameter.Commands;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes
/// </summary>
public static class Host
{

    public static void Start()
    {
        var services = new ServiceCollection();
        
        var doc = Context.ActiveDocument;
        if (doc != null)
        {
            services.AddSingleton(doc);
        }
        services.AddSingleton<Func<AddFilterView?>>(_ => AddFilter.Host.Host.Start);
        services.AddSingleton<IViewByParameterModel,ViewByParameterModel>();
        services.AddSingleton<ViewByParameterViewModel>();
        services.AddSingleton<ViewByParameterView>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        var tws = serviceProvider.GetService<ThemeWatcherService>();
        var view = serviceProvider.GetService<ViewByParameterView>();
        tws?.SetConfigTheme(view);
        view?.ShowDialog();
    }
    
    public static void StartTestUi()
    {
        var services = new ServiceCollection();
        services.AddSingleton<Func<AddFilterView?>>(_ => AddFilter.Host.Host.StartTestUi);
        services.AddSingleton<IViewByParameterModel,ViewByParameterModelTestUi>();
        services.AddSingleton<ViewByParameterViewModel>();
        services.AddSingleton<ViewByParameterView>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        var tws = serviceProvider.GetService<ThemeWatcherService>();
        var view = serviceProvider.GetService<ViewByParameterView>();
        tws?.SetConfigTheme(view);
        view?.ShowDialog();
    }
}