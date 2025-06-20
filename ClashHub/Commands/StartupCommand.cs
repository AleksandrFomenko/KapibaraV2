using Autodesk.Revit.Attributes;
using ClashHub.Views;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;
using Nice3point.Revit.Toolkit.External;
using ClashDetectiveViewModel = ClashHub.ViewModels.ClashDetectiveViewModel;

namespace ClashHub.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var document = Context.ActiveDocument;
        var services = new ServiceCollection();

        if (document != null) services.AddSingleton(document);
        services.AddSingleton<ClashDetectiveViewModel>();
        services.AddSingleton<ClashDetectiveView>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var view = serviceProvider.GetRequiredService<ClashDetectiveView>();
        var tws = serviceProvider.GetRequiredService<IThemeWatcherService>();
        tws.SetConfigTheme(view);
        view.Show();
    }
}