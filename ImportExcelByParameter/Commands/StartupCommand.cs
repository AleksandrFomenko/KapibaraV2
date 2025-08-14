using Autodesk.Revit.Attributes;
using ImportExcelByParameter.Configuration;
using Nice3point.Revit.Toolkit.External;
using ImportExcelByParameter.ViewModels;
using ImportExcelByParameter.Views;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;


namespace ImportExcelByParameter.Commands;


[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var services = new ServiceCollection();
        var doc = Context.ActiveDocument;
        if (doc != null)
        {
            services.AddSingleton(doc);
        }

        services.AddSingleton<Config>();
        services.AddSingleton<ImportExcelByParameterViewModel>();
        services.AddSingleton<ImportExcelByParameterView>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();

        var serviceBuilder = services.BuildServiceProvider();
        var view = serviceBuilder.GetRequiredService<ImportExcelByParameterView>();
        var themeWatcherService = serviceBuilder.GetRequiredService<IThemeWatcherService>();
        
        themeWatcherService.SetConfigTheme(view);
        view.ShowDialog();
    }
}