using System.Windows;
using LegendPlacer.Commands;

namespace TestUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        Host.StartTestUi();
    }
}