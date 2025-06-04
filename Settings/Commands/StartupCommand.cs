using Autodesk.Revit.Attributes;
using KapibaraUI.Services.Appearance;
using Nice3point.Revit.Toolkit.External;
using Settings.Views;
using Wpf.Ui.Appearance;

namespace Settings.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        Host.Start();
        var view = Host.GetService<SettingsView>();
        var tws = Host.GetService<IThemeWatcherService>();
        ThemeWatcherService.Initialize();
        
        tws.SetTheme(ApplicationTheme.Light,view);
        view.ShowDialog();
    }
}