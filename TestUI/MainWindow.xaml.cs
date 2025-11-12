using EngineeringSystems.Commands;
using HostAxes = Axes.Host.Host;


namespace TestUI;

public partial class MainWindow
{
    public MainWindow()
    {
        GroupSystems.StartMock();
        GroupSystems.Start();
    }
}