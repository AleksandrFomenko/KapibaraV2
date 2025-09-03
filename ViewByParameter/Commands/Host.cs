using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;
using ViewByParameter.AddFilter.View;
using ViewByParameter.Models;
using ViewByParameter.services;
using ViewByParameter.ViewModels;
using ViewByParameter.Views;

namespace ViewByParameter.Commands;

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
        services.AddSingleton<FilterCreationService>();
        services.AddSingleton<ViewCreationService>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        var tws = serviceProvider.GetService<IThemeWatcherService>();
        var view = serviceProvider.GetService<ViewByParameterView>();
        tws?.Initialize();
        view.SourceInitialized += (sender, args) => tws.SetConfigTheme();
        
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
        
        var tws = serviceProvider.GetService<IThemeWatcherService>();
        var view = serviceProvider.GetService<ViewByParameterView>();
        tws?.SetConfigTheme(view);
        view?.ShowDialog();
    }
}