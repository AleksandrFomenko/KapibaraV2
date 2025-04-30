using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using Insolation.ViewModels;
using Insolation.Views;

namespace Insolation.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new InsolationViewModel();
        var view = new InsolationView(viewModel);
        view.Show(UiApplication.MainWindowHandle);
    }
}