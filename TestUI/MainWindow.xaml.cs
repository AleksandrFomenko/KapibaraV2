using HostViewByParameter = ViewByParameter.Commands.Host;
using HostLegendPlacer = LegendPlacer.Commands.Host;
using ExporterModelsHost = ExporterModels.Host;


namespace TestUI;

public partial class MainWindow
{
    public MainWindow()
    {
        //HostLegendPlacer.StartTestUi();
        //HostViewByParameter.StartTestUi();
        ExporterModelsHost .StartMock();
        
        
    }
}