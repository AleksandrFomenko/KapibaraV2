using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using RiserMate.Views;

namespace RiserMate.Commands;

/// <summary>
///     External command entry point
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var view = RiserMateHost.GetService<RiserMateView>();
        view.Show(UiApplication.MainWindowHandle);
    }
}