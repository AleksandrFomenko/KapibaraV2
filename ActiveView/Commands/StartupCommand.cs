using ActiveView.Models;
using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using ActiveView.ViewModels;
using ActiveView.Views;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;


namespace ActiveView.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        
        var services = new ServiceCollection();

        var document = Context.ActiveDocument;
        if (document != null)
        {
            services.AddSingleton(document); 
        }

        services.AddScoped<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<IModelActiveView, ActiveViewModel>();
        services.AddSingleton<ActiveViewViewModel>();
        services.AddSingleton<ActiveViewView>();

        var serviceProvider = services.BuildServiceProvider();
        var themeService = serviceProvider.GetRequiredService<IThemeWatcherService>();
        themeService.Initialize();
        var view = serviceProvider.GetRequiredService<ActiveViewView>();
        themeService.SetConfigTheme();
        view.SourceInitialized += (sender, args) => themeService.SetConfigTheme();
        view.ShowDialog();
    }
}
