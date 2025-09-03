using Autodesk.Revit.Attributes;
using KapibaraUI.Services.Appearance;
using Nice3point.Revit.Toolkit.External;
using Settings.Configuration;
using Settings.Models;
using Settings.ViewModels;
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
        var vm = Host.GetService<SettingsViewModel>();
        var tws = Host.GetService<IThemeWatcherService>();
        
        vm.SetSetting();
        view.SourceInitialized += (sender, args) => tws.SetConfigTheme();
        view.ShowDialog();
    }
}