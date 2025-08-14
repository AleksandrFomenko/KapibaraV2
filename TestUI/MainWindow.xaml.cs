using HostViewByParameter = ViewByParameter.Commands.Host;
using HostLegendPlacer = LegendPlacer.Commands.Host;


namespace TestUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        //HostLegendPlacer.StartTestUi();
        HostViewByParameter.StartTestUi();
    }
}