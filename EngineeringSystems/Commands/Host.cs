using EngineeringSystems.Configuration;
using EngineeringSystems.Model;
using EngineeringSystems.ViewModels;
using EngineeringSystems.Views;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;

namespace EngineeringSystems.Commands;

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
        
        services.AddSingleton<IData, Data>();
        services.AddSingleton<IEngineeringSystemsModel, EngineeringSystemsModel>();
        services.AddSingleton<Config>();
        services.AddSingleton<EngineeringSystemsViewModel>();
        services.AddSingleton<EngineeringSystemsView>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var view = serviceProvider.GetRequiredService<EngineeringSystemsView>();
        var tws = serviceProvider.GetRequiredService<IThemeWatcherService>();
        tws.SetConfigTheme(view);
        view.ShowDialog();
    }
}