using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;
using ViewByParameter.AddFilter.Models;
using ViewByParameter.AddFilter.View;
using ViewByParameter.AddFilter.ViewModels;
using ViewByParameter.Views;

namespace ViewByParameter.AddFilter.Host;

public static class Host
{
    public static AddFilterView? Start()
    {
        var services = new ServiceCollection();
        
        var doc = Context.ActiveDocument;
        if (doc != null)
        {
            services.AddSingleton(doc);
        }
        
        services.AddSingleton<IAddFilterModel, AddFilterModel>();
        services.AddSingleton<AddFilterViewModel>();
        services.AddSingleton<AddFilterView>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        var tws = serviceProvider.GetService<IThemeWatcherService>();
        var view = serviceProvider.GetService<AddFilterView>();
        return view;
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
        return view;
    }
}