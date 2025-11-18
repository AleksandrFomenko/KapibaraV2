using EngineeringSystems.Commands;
using Nice3point.Revit.Extensions;
using RiserMate;
using RiserMate.Views;
using Wpf.Ui;
using HostAxes = Axes.Host.Host;


namespace TestUI;

public partial class MainWindow
{
    public MainWindow()
    {
        RiserMateHost.StartMockServices();
        var view = RiserMateHost.GetService<RiserMateView>();
        view.Show();
    }
}