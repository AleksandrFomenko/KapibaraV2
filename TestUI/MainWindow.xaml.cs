


using KapibaraUI.Services.Appearance;
using VentilationInstallations;
using VentilationInstallations.Views;

namespace TestUI;

public partial class MainWindow
{
    public MainWindow()
    {
        Host.StartMock();
        var tws = Host.GetService<IThemeWatcherService>();
        tws.Initialize();
        
        var view = Host.GetService<VentilationInstallationsView>();
        view.ShowDialog();
    }
}