using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using ClashDetective.ViewModels;
using ClashDetective.Views;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;

namespace ClashDetective.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var document = Context.ActiveDocument;
        var services = new ServiceCollection();
        
        services.AddSingleton(document);
        services.AddSingleton<ClashDetectiveViewModel>();
        services.AddSingleton<ClashDetectiveView>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var view = serviceProvider.GetRequiredService<ClashDetectiveView>(); 
        view.Show();
    }
}