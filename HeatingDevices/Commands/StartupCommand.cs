using Autodesk.Revit.Attributes;
using HeatingDevices.Views;
using KapibaraUI.Services.Appearance;
using Nice3point.Revit.Toolkit.External;
using Wpf.Ui.Appearance;

namespace HeatingDevices.Commands;

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
        var view = Host.GetService<SpaceHeaterView>();
        var tws = Host.GetService<IThemeWatcherService>();
        tws.SetConfigTheme();
        view.ShowDialog();
        
    }
}