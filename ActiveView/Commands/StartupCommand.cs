using ActiveView.Models;
using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using ActiveView.ViewModels;
using ActiveView.Views;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;


namespace ActiveView.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
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

        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<IModelActiveView, ActiveViewModel>();
        services.AddSingleton<ActiveViewViewModel>();
        services.AddSingleton<ActiveViewView>();

        var serviceProvider = services.BuildServiceProvider();

        var view = serviceProvider.GetRequiredService<ActiveViewView>();
        var theme = serviceProvider.GetRequiredService<IThemeWatcherService>();
        theme.SetConfigTheme(view);

        view.ShowDialog();
    }
}
