using Autodesk.Revit.Attributes;
using ImportExcelByParameter.Configuration;
using ImportExcelByParameter.Models;
using Nice3point.Revit.Toolkit.External;
using ImportExcelByParameter.ViewModels;
using ImportExcelByParameter.Views;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;
using Nice3point.Revit.Extensions.UI;


namespace ImportExcelByParameter.Commands;


[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        Handlers.Handlers.RegisterHandlers();
        var services = new ServiceCollection();
        var doc = RevitContext.ActiveDocument;
        if (doc != null) services.AddSingleton(doc);

        services.AddSingleton<Config>();
        services.AddSingleton<ExcelByParameterModel>();
        services.AddSingleton<ImportExcelByParameterViewModel>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<ImportExcelByParameterView>();

        var sp = services.BuildServiceProvider();
        var view = sp.GetRequiredService<ImportExcelByParameterView>();
        var tws = sp.GetRequiredService<IThemeWatcherService>();
        tws.SetConfigTheme();
        view.SourceInitialized += (_, _) => tws.SetConfigTheme();
        view.Show(RevitContext.UiApplication.MainWindowHandle);
    }
}