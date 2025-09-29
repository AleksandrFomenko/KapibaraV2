using HostProjectAxes = ProjectAxes.Commands.Host;
using HostAxes = Axes.Host.Host;


namespace TestUI;

public partial class MainWindow
{
    public MainWindow()
    {
        HostAxes.StartMock();
    }
}