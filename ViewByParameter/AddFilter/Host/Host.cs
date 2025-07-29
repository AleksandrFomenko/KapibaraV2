using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;
using ViewByParameter.AddFilter.Models;
using ViewByParameter.AddFilter.View;
using ViewByParameter.AddFilter.ViewModels;
using ViewByParameter.Views;

namespace ViewByParameter.AddFilter.Host;

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
        
        services.AddSingleton<IAddFilterModel, AddFilterModel>();
        services.AddSingleton<AddFilterModel>();
        services.AddSingleton<AddFilterView>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        var tws = serviceProvider.GetService<ThemeWatcherService>();
        var view = serviceProvider.GetService<AddFilterView>();
        tws?.SetConfigTheme(view);
        view?.ShowDialog();
    }
    
    public static AddFilterView? StartTestUi()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<IAddFilterModel, AddFilterModelTestUi>();
        services.AddSingleton<AddFilterViewModel>();
        services.AddSingleton<AddFilterView>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var tws = serviceProvider.GetService<ThemeWatcherService>();
        var view = serviceProvider.GetService<AddFilterView>();
        tws?.SetConfigTheme(view);
        return view;
    }
}